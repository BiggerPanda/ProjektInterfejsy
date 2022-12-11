#define Testing 

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
    public bool IsWhiteTurn = false;

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
    public ChessPiece[,] ChessPieces => chessPieces;


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
        IsWhiteTurn = true;
    }

    private void Start()
    {
        DrawBoard();
        SpawnAllPieces();
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

                if (true) // turn check
                {
                    Tile.IsPieceDraged = true;
                    curentlyDragged = chessPieces[tile.X, tile.Y];
                    avaliableMoves = curentlyDragged.GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
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

        Debug.Log("PieceGrabed " + _piece.position);

        if ((_piece.team == TeamColor.White && isWhiteTurn == true) || (_piece.team == TeamColor.Black && isWhiteTurn == false)) // turn check
        {
            Tile.IsPieceDraged = true;
            curentlyDragged = chessPieces[_piece.position.x, _piece.position.y];

            avaliableMoves = curentlyDragged.GetAvaliableMoves(ref chessPieces, CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y);
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


    #region SpawnPieces

    [Button("Spawn Pieces")]
    private void SpawnAllPieces()
    {
        chessPieces[0, 0] = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.White);
        chessPieces[1, 0] = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.White);
        chessPieces[2, 0] = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.White);
        chessPieces[3, 0] = SpanwSinglePiece(ChessPieceType.Queen, TeamColor.White);
        chessPieces[4, 0] = SpanwSinglePiece(ChessPieceType.King, TeamColor.White);
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

            if (enemyChessPiece.team == TeamColor.White)
            {
                if (enemyChessPiece.type == ChessPieceType.King)
                {
                    CheckMate(TeamColor.Black);
                }

                whiteDead.Add(enemyChessPiece);
                enemyChessPiece.SetScale(Vector3.one * deadSize);
                enemyChessPiece.SetPosition(deathPlaceWhite.transform.position + new Vector3(0, 0, whiteDead.Count * tileSize / 2));
            }
            else
            {
                if (enemyChessPiece.type == ChessPieceType.King)
                {
                    CheckMate(TeamColor.White);
                }

                blackDead.Add(enemyChessPiece);
                enemyChessPiece.SetScale(Vector3.one * deadSize);
                enemyChessPiece.SetPosition(deathPlaceBlack.transform.position + new Vector3(0, 0, blackDead.Count * tileSize / 2));

            }
        }
        chessPieces[_x, _y] = _piece;
        chessPieces[_piece.position.x, _piece.position.y] = null;
        notationWriter.writePGNNotation(_x, _y, _piece);
        IsWhiteTurn = !IsWhiteTurn;

        PositionSinglePiece(_x, _y);

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(_x, _y) });

        return true;
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
        if (_team == TeamColor.White)
        {
            Debug.Log("Black Wins");
        }
        else
        {
            Debug.Log("White Wins");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

}
