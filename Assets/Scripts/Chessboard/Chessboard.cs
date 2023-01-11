//#define Testing 

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public enum TeamColor
{
    White,
    Black
}

public enum SpecialMove
{
    None,
    EnPassant,
    Castling,
    Promotion
}

public class Chessboard : MonoBehaviour
{
    public const int CHESSBOARD_SIZE_X = 8;
    public const int CHESSBOARD_SIZE_Y = 8;

    public static Chessboard Instance { get; private set; }
    public event Action onPieceMove;
    public event Action<TeamColor> onCheckMate;
    public bool IsWhiteTurn => isWhiteTurn;
    public bool GameOver = false;

    [SerializeField] private float tileSize = 1.3f;
    [SerializeField] private GameObject tileGameObject;
    [SerializeField] private ChessModelData chessData;
    [SerializeField] private float deadSize = 0.25f;
    [SerializeField] private GameObject deathPlaceWhite = null;
    [SerializeField] private GameObject deathPlaceBlack = null;
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private XRDirectInteractor leftDirectInteractor;

    private Tile[,] tiles = new Tile[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
    private ChessPiece[,] chessPieces = new ChessPiece[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
    private List<ChessPiece> whiteDead = new List<ChessPiece>();
    private List<ChessPiece> blackDead = new List<ChessPiece>();
    private List<Vector2Int> avaliableMoves = new List<Vector2Int>();
    private ChessPiece curentlyDragged = null;
    private Vector2Int previousPosition = Vector2Int.zero;
    private RaycastHit hit;
    private Ray cameraRay;
    public InputActionReference ActivationRayReference;
    public InputActionReference ActivationDirectReference;
    private bool isGrabbed = false;
    private SpecialMove specialMove = SpecialMove.None;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private bool isWhiteTurn = false;
    private NotationWriter notationWriter = null;
    private ChessPiece WhiteKingRef = null;
    private ChessPiece BlackKingRef = null;
    private List<Vector2Int> movesToRemove = new List<Vector2Int>();
    private CheesPlayer botPlayer = null;
    private CheesPlayer player = null;
    public ChessPiece[,] ChessPieces => chessPieces;
    public int BlackDeadAmount => blackDead.Count;
    public int WhiteDeadAmount => whiteDead.Count;
    public CheesPlayer Player => player;
    public CheesPlayer BotPlayer => botPlayer;

    private void Awake()
    {
        notationWriter = new NotationWriter();
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                GameObject tile = Instantiate(tileGameObject, new Vector3(tileSize * x, 0, tileSize * y), Quaternion.identity);
                tile.transform.parent = transform;
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                tile.name = $"Tile {x}{y}";
                tiles[x, y] = tile.GetComponent<Tile>().SetPosition(x, y);
            }
        }

        Instance = this;
        isWhiteTurn = true;
    }

    private void Start()
    {
        DrawBoard();
        SpawnAllPieces();
        CreatePlayer();
        CreateBotPlayer();
#if !Testing
        ActivationRayReference.action.performed += GrabPieceByRay;
        ActivationRayReference.action.canceled += LeftPieceByRay;
#endif
    }

    private void Update()
    {
#if Testing
        if (Input.GetMouseButtonDown(0))
        {
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile == null || chessPieces[tile.X, tile.Y] == null)
                {
                    return;
                }

                if ((chessPieces[tile.X, tile.Y].team == TeamColor.White && isWhiteTurn == true) || (chessPieces[tile.X, tile.Y].team == TeamColor.Black && isWhiteTurn == false)) // turn check
                {
                    Tile.IsPieceDraged = true;
                    curentlyDragged = chessPieces[tile.X, tile.Y];
                    avaliableMoves = curentlyDragged.GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
                    specialMove = curentlyDragged.GetSpecialMoves(ref chessPieces, ref moveList, ref avaliableMoves);
                    PreventCheck();
                    HighlightAvaliableTiles();
                    tile.SetChessPiece(null);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (curentlyDragged == null)
            {
                return;
            }

            previousPosition = curentlyDragged.position;
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cameraRay, out hit))
            {
                if (hit.collider.TryGetComponent<Tile>(out Tile tile) == false)
                {
                    curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
                    curentlyDragged = null;
                }

                bool validMove = MoveTo(ref curentlyDragged, tile.X, tile.Y);
                RemoveHighlightedTiles();
                tile.SetChessPiece(curentlyDragged);
                if (validMove == false)
                {
                    curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
                    curentlyDragged = null;
                    Tile.IsPieceDraged = false;

                }
                else
                {
                    curentlyDragged = null;
                    Tile.IsPieceDraged = false;
                }
            }
            else
            {
                curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
                curentlyDragged = null;
            }
        }

        if (curentlyDragged != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            curentlyDragged.SetPosition(Camera.main.ScreenToWorldPoint(mousePosition));
        }
#endif

        if (botPlayer == null)
        {
            return;
        }

        if (IsWhiteTurn == (botPlayer.playerColor == TeamColor.White))
        {
            if(GameOver == true)
            {
                return;
            }

           StartCoroutine(botPlayer.MakeRandomMove());
        }
    }

    public void WinGame(TeamColor team)
    {
        CheckMate(team);
    }

    #region  VRIntegration

    public void GrabPieceByRay(InputAction.CallbackContext obj)
    {
#if !Testing
        if (isGrabbed == true)
        {
            return;
        }

        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile == null || chessPieces[tile.X, tile.Y] == null)
            {
                return;
            }

            if ((chessPieces[tile.X, tile.Y].team == TeamColor.White && isWhiteTurn == true) || (chessPieces[tile.X, tile.Y].team == TeamColor.Black && isWhiteTurn == false)) // turn check
            {
                Tile.IsPieceDraged = true;
                curentlyDragged = chessPieces[tile.X, tile.Y];
                avaliableMoves = curentlyDragged.GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
                specialMove = curentlyDragged.GetSpecialMoves(ref chessPieces, ref moveList, ref avaliableMoves);
                PreventCheck();
                HighlightAvaliableTiles();
                tile.SetChessPiece(null);
            }
        }
#endif
    }

    public void LeftPieceByRay(InputAction.CallbackContext obj)
    {
#if !Testing
        if (isGrabbed == true)
        {
            RemoveHighlightedTiles();
            return;
        }

        if (curentlyDragged == null)
        {
            RemoveHighlightedTiles();
            return;
        }

        previousPosition = curentlyDragged.position;


        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            if (hit.collider.TryGetComponent<Tile>(out Tile tile) == false)
            {
                curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
                curentlyDragged = null;
                return;
            }

            bool validMove = MoveTo(ref curentlyDragged, tile.X, tile.Y);
            RemoveHighlightedTiles();
            tile.SetChessPiece(curentlyDragged);
            if (validMove == false)
            {
                curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
                curentlyDragged = null;
                Tile.IsPieceDraged = false;

            }
            else
            {
                curentlyDragged = null;
                Tile.IsPieceDraged = false;
            }
        }
        else
        {
            curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
            curentlyDragged = null;
        }
#endif
    }

    public void PieceGrabed(ChessPiece _piece)
    {
#if !Testing
        isGrabbed = true;
        if (_piece == null)
        {
            return;
        }

        if ((_piece.team == TeamColor.White && isWhiteTurn == true) || (_piece.team == TeamColor.Black && isWhiteTurn == false)) // turn check
        {
            Tile.IsPieceDraged = true;
            curentlyDragged = chessPieces[_piece.position.x, _piece.position.y];

            avaliableMoves = curentlyDragged.GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
            specialMove = curentlyDragged.GetSpecialMoves(ref chessPieces, ref moveList, ref avaliableMoves);
            PreventCheck();
            HighlightAvaliableTiles();
            tiles[curentlyDragged.position.x, curentlyDragged.position.y].SetChessPiece(null);
        }
#endif
    }

    public void PieceLeft(Tile _tile)
    {
#if !Testing
        if (curentlyDragged == null)
        {
            Debug.Log("PieceLeft curentlyDragged == null");
            RemoveHighlightedTiles();
            return;
        }

        if (_tile == null)
        {
            curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
            curentlyDragged = null;
            return;
        }

        bool validMove = MoveTo(ref curentlyDragged, _tile.X, _tile.Y);
        RemoveHighlightedTiles();
        _tile.SetChessPiece(curentlyDragged);
        if (validMove == false)
        {
            curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
            curentlyDragged = null;
            Tile.IsPieceDraged = false;

        }
        else
        {
            curentlyDragged = null;
            Tile.IsPieceDraged = false;
        }

        isGrabbed = false;
#endif
    }

    #endregion

    #region SpawnPieces

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= CHESSBOARD_SIZE_X || y < 0 || y >= CHESSBOARD_SIZE_Y)
        {
            return null;
        }

        return tiles[x, y];
    }

    [Button("Generate chessboard")]
    private void DrawBoard()
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                tiles[x, y].DrawTile(x, y);
            }
        }
    }

    [Button("Spawn Pieces")]
    private void SpawnAllPieces()
    {
        chessPieces[0, 0] = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.White);
        chessPieces[1, 0] = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.White);
        chessPieces[2, 0] = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.White);
        chessPieces[3, 0] = SpanwSinglePiece(ChessPieceType.Queen, TeamColor.White);
        chessPieces[4, 0] = SpanwSinglePiece(ChessPieceType.King, TeamColor.White);
        WhiteKingRef = chessPieces[4, 0];
        chessPieces[5, 0] = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.White);
        chessPieces[6, 0] = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.White);
        chessPieces[7, 0] = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.White);

        for (int i = 0; i < CHESSBOARD_SIZE_X; i++)
        {
            chessPieces[i, 1] = SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.White);
        }

        chessPieces[0, 7] = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.Black);
        chessPieces[1, 7] = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.Black);
        chessPieces[2, 7] = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.Black);
        chessPieces[3, 7] = SpanwSinglePiece(ChessPieceType.Queen, TeamColor.Black);
        chessPieces[4, 7] = SpanwSinglePiece(ChessPieceType.King, TeamColor.Black);
        BlackKingRef = chessPieces[4, 7];
        chessPieces[5, 7] = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.Black);
        chessPieces[6, 7] = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.Black);
        chessPieces[7, 7] = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.Black);

        for (int i = 0; i < CHESSBOARD_SIZE_X; i++)
        {
            chessPieces[i, 6] = SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.Black);
        }

        PositionAllPieces();
    }

    public ChessPiece SpanwSinglePiece(ChessPieceType _pieceType, TeamColor _team)
    {
        ChessPiece piece = Instantiate(chessData.GetModel(_pieceType), transform).GetComponent<ChessPiece>();
        piece.SetTeam(_team);
        piece.SetType(_pieceType);
        return piece;
    }

    private void PositionAllPieces()
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                PositionSinglePiece(x, y, true);
            }
        }
    }

    private void PrintBoard(ref ChessPiece[,] _board)
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                Debug.Log(_board[x, y]);
            }
        }
    }

    private void PositionSinglePiece(int _x, int _y, bool _force = false)
    {
        if (chessPieces[_x, _y] == null)
        {
            return;
        }

        chessPieces[_x, _y].SetPosition(tiles[_x, _y].transform.position, _force);
        chessPieces[_x, _y].SetPosition(_x, _y);
        tiles[_x, _y].ToggleTaken();
    }
    #endregion

    #region MovePiece
    private bool ContainsValidMove(ref List<Vector2Int> _moves, Vector2 move)
    {
        for (int i = 0; i < _moves.Count; i++)
        {
            if (_moves[i].x == move.x && _moves[i].y == move.y)
            {
                return true;
            }
        }

        return false;
    }

    private bool MoveTo(ref ChessPiece _piece, int _x, int _y)
    {
        if (ContainsValidMove(ref avaliableMoves, new Vector2(_x, _y)) == false)
        {
            return false;
        }

        if (chessPieces[_x, _y] != null)
        {
            if (chessPieces[_x, _y].team == _piece.team)
            {
                return false;
            }

            ChessPiece enemyChessPiece = chessPieces[_x, _y];

            KillPiece(enemyChessPiece);
        }
        chessPieces[_x, _y] = _piece;
        chessPieces[_piece.position.x, _piece.position.y] = null;
        notationWriter.writePGNNotation(_x, _y, _piece);
        isWhiteTurn = !isWhiteTurn;

        PositionSinglePiece(_x, _y);

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(_x, _y) });

        ProcessSpecialMove();

        onPieceMove?.Invoke();
        
        player.UpdateChesspieces();
        botPlayer.UpdateChesspieces();

        if (CheckForCheckmate())
        {
            CheckMate(_piece.team);
        }

        return true;
    }

    public void PlayerMovePiece(ref ChessPiece _piece, int _x, int _y)
    {
        curentlyDragged = _piece;
        avaliableMoves = _piece.GetAvaliableMoves(ref chessPieces , CHESSBOARD_SIZE_X,CHESSBOARD_SIZE_Y);
        specialMove = _piece.GetSpecialMoves(ref chessPieces, ref moveList, ref avaliableMoves);
        PreventCheck();

        Tile _tile = tiles[_x, _y];
        bool validMove = MoveTo(ref _piece, _x, _y);
        Debug.Log(validMove);
        RemoveHighlightedTiles();
        
        if (validMove == false)
        {
            curentlyDragged.SetPosition(tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position);
            curentlyDragged = null;
            Tile.IsPieceDraged = false;

        }
        else
        {
            curentlyDragged = null;
            Tile.IsPieceDraged = false;
            _tile.SetChessPiece(curentlyDragged);
        }
    }
    #endregion

    #region SpecialMoves
    private void ProcessSpecialMove()
    {
        if (specialMove == SpecialMove.None)
        {
            return;
        }

        if (specialMove == SpecialMove.EnPassant)
        {
            Vector2Int[] _newMove = moveList[moveList.Count - 1];
            Vector2Int[] _targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece _myPawn = chessPieces[_newMove[1].x, _newMove[1].y];
            ChessPiece _enemyPawn = chessPieces[_targetPawnPosition[1].x, _targetPawnPosition[1].y];

            if (_myPawn.position.x == _enemyPawn.position.x)
            {
                if (_myPawn.position.y == _enemyPawn.position.y - 1 || _myPawn.position.y == _enemyPawn.position.y + 1)
                {
                    KillPiece(_enemyPawn);
                    chessPieces[_enemyPawn.position.x, _enemyPawn.position.y] = null;
                    specialMove = SpecialMove.None;
                }
            }
        }

        if (specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            if (lastMove[1].y == 0)
            {
                if (lastMove[1].x == 6)
                {
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    chessPieces[7, 0] = null;
                    PositionSinglePiece(5, 0);
                }
                else if (lastMove[1].x == 2)
                {
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    chessPieces[0, 0] = null;
                    PositionSinglePiece(3, 0);
                }
            }
            else if (lastMove[1].y == 7)
            {
                if (lastMove[1].x == 6)
                {
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    chessPieces[7, 7] = null;
                    PositionSinglePiece(5, 7);
                }
                else if (lastMove[1].x == 2)
                {
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    chessPieces[0, 7] = null;
                    PositionSinglePiece(3, 7);
                }
            }
        }

        if (specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece pawn = chessPieces[lastMove[1].x, lastMove[1].y];
            if (pawn.position.y == 0 || pawn.position.y == 7)
            {
                ChessPiece newPiece = SpanwSinglePiece(ChessPieceType.Queen, pawn.team);
                Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                chessPieces[pawn.position.x, pawn.position.y] = newPiece;
                PositionSinglePiece(lastMove[1].x, lastMove[1].y);
            }
        }
    }

    private void PreventCheck()
    {
        if (curentlyDragged != null)
        {
            if (curentlyDragged.team == TeamColor.White)
            {
                SimulateMoveForSinglePiece(curentlyDragged, ref avaliableMoves, WhiteKingRef);
            }
            else
            {
                SimulateMoveForSinglePiece(curentlyDragged, ref avaliableMoves, BlackKingRef);
            }
        }
    }

    private void SimulateMoveForSinglePiece(ChessPiece dragged, ref List<Vector2Int> moves, ChessPiece king)
    {
        movesToRemove.Clear();
        Vector2Int dragedRealPosition = dragged.position;
        Vector2Int kingSimPosition = king.position;
        ChessPiece[,] tempChessPieces = new ChessPiece[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
        List<ChessPiece> tempAttackChessPiece = new List<ChessPiece>();
        for (int i = 0; i < moves.Count; i++)
        {
            if (dragged.type == ChessPieceType.King)
            {
                kingSimPosition = moves[i];
            }

            for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
            {
                for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        tempChessPieces[x, y] = chessPieces[x, y];
                        if (tempChessPieces[x, y].team != dragged.team)
                        {
                            tempAttackChessPiece.Add(tempChessPieces[x, y]);
                        }
                    }
                }
            }

            tempChessPieces[dragged.position.x, dragged.position.y] = null;
            dragged.position = moves[i];
            tempChessPieces[moves[i].x, moves[i].y] = dragged;
            Vector2Int simulatedPosition = moves[i];

            ChessPiece dead = tempAttackChessPiece.Find(x => x.position == simulatedPosition);

            if (dead != null)
            {
                tempAttackChessPiece.Remove(dead);
            }

            List<Vector2Int> simulatedMoves = new List<Vector2Int>();
            for (int x = 0; x < tempAttackChessPiece.Count; x++)
            {
                List<Vector2Int> pieceMoves = tempAttackChessPiece[x].GetAvaliableMoves(ref tempChessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
                for (int y = 0; y < pieceMoves.Count; y++)
                {
                    simulatedMoves.Add(pieceMoves[y]);
                }
            }

            if (ContainsValidMove(ref simulatedMoves, king.position))
            {
                movesToRemove.Add(moves[i]);
            }

            dragged.position = dragedRealPosition;
        }

        for (int i = 0; i < movesToRemove.Count; i++)
        {
            moves.Remove(movesToRemove[i]);
        }
    }

    private bool CheckForCheckmate()
    {
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        TeamColor targetTeam = chessPieces[lastMove[1].x, lastMove[1].y].team == TeamColor.White ? TeamColor.Black : TeamColor.White;

        List<ChessPiece> attackPieces = new List<ChessPiece>();
        List<ChessPiece> defendPieces = new List<ChessPiece>();
        ChessPiece king = null;

        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].team == targetTeam)
                    {
                        defendPieces.Add(chessPieces[x, y]);

                        if (chessPieces[x, y].type == ChessPieceType.King)
                        {
                            king = chessPieces[x, y];
                        }
                    }
                    else
                    {
                        attackPieces.Add(chessPieces[x, y]);
                    }
                }
            }
        }

        List<Vector2Int> currentAvaliableMoves = new List<Vector2Int>();
        for (int x = 0; x < attackPieces.Count; x++)
        {
            List<Vector2Int> pieceMoves = attackPieces[x].GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
            for (int y = 0; y < pieceMoves.Count; y++)
            {
                currentAvaliableMoves.Add(pieceMoves[y]);
            }
        }

        Debug.Log(currentAvaliableMoves.Count);
        Debug.Log(king.position);
        Debug.Log(king.team);

        if (ContainsValidMove(ref currentAvaliableMoves, king.position))
        {
            for (int x = 0; x < defendPieces.Count; x++)
            {
                List<Vector2Int> defendingMoves = defendPieces[x].GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
                SimulateMoveForSinglePiece(defendPieces[x], ref defendingMoves, king);
                if (defendingMoves.Count != 0)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            Debug.Log("sth fucked up");
        }

        return false;
    }

    #endregion
    private void KillPiece(ChessPiece _piece)
    {
        if (_piece.team == TeamColor.White)
        {
            whiteDead.Add(_piece);
            _piece.SetScale(Vector3.one * deadSize);
            _piece.SetPosition(deathPlaceWhite.transform.position + new Vector3(0, 0, whiteDead.Count * tileSize / 2));
            _piece.GetComponent<Collider>().enabled = false;
        }
        else
        {
            blackDead.Add(_piece);
            _piece.SetScale(Vector3.one * deadSize);
            _piece.SetPosition(deathPlaceBlack.transform.position + new Vector3(0, 0, blackDead.Count * tileSize / 2));
            _piece.GetComponent<Collider>().enabled = false;
        }

        player.UpdateChesspieces();
        botPlayer.UpdateChesspieces();
    }

    private void HighlightAvaliableTiles()
    {
        if (curentlyDragged == null)
        {
            return;
        }

        foreach (Vector2Int move in avaliableMoves)
        {
            tiles[move.x, move.y].MakeAvaliable();
        }
    }

    private void RemoveHighlightedTiles()
    {
        if (curentlyDragged == null)
        {
            return;
        }

        foreach (Vector2Int move in avaliableMoves)
        {
            tiles[move.x, move.y].MakeUnavaliable();
        }
    }

    private void CheckMate(TeamColor _team)
    {
        GameOver = true;
        if (_team == TeamColor.White)
        {
            Debug.Log("Black Wins");
        }
        else
        {
            Debug.Log("White Wins");
        }

        onCheckMate?.Invoke(_team);
    }

    private void CreateBotPlayer()
    {
        botPlayer = new GameObject("botPlayer").AddComponent<CheesPlayer>();
        botPlayer.SetupPlayer(TeamColor.Black);
        botPlayer.isBot = true;
    }

    private void CreatePlayer()
    {
        player = new GameObject("Player").AddComponent<CheesPlayer>();
        player.SetupPlayer(TeamColor.White);
    }
}
