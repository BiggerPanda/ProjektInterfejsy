using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using NaughtyAttributes;

public enum TeamColor
{
    White,
    Black
}

public class Chessboard : MonoBehaviour
{
    public const int CHESSBOARD_SIZE_X = 8;
    public const int CHESSBOARD_SIZE_Y = 8;

    [ReadOnly] public string Notation;
    [SerializeField] private float tileSize = 1.3f;
    [SerializeField] private GameObject tileGameObject;
    [SerializeField] private ChessModelData chessData;
    [SerializeField] private float deadSize = 0.25f;
    [SerializeField] private GameObject deathPlaceWhite = null;
    [SerializeField] private GameObject deathPlaceBlack = null;

    private Tile[,] tiles = new Tile[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
    private ChessPiece[,] chessPieces = new ChessPiece[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
    private List<ChessPiece> whiteDead = new List<ChessPiece>();
    private List<ChessPiece> blackDead = new List<ChessPiece>();
    private List<Vector2Int> avaliableMoves = new List<Vector2Int>();
    private ChessPiece curentlyDragged = null;
    private Vector2Int previousPosition = Vector2Int.zero;
    private RaycastHit hit;
    private Ray cameraRay;

    private void Awake()
    {
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
    }

    private void Start()
    {
        DrawBoard();
        SpawnAllPieces();
    }

    private void Update()
    {
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
    }

    #region  VRIntegration

    public void GrabPiece()
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

    public void LeftPiece()
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

    private ChessPiece SpanwSinglePiece(ChessPieceType _pieceType, TeamColor _team)
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
                whiteDead.Add(enemyChessPiece);
                enemyChessPiece.SetScale(Vector3.one * deadSize);
                enemyChessPiece.SetPosition(deathPlaceWhite.transform.position + new Vector3(0, 0, whiteDead.Count * tileSize / 2));
            }
            else
            {
                blackDead.Add(enemyChessPiece);
                enemyChessPiece.SetScale(Vector3.one * deadSize);
                enemyChessPiece.SetPosition(deathPlaceBlack.transform.position + new Vector3(0, 0, blackDead.Count * tileSize / 2));

            }
        }
        chessPieces[_x, _y] = _piece;
        chessPieces[_piece.position.x, _piece.position.y] = null;

        PositionSinglePiece(_x, _y);

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
    #endregion

    #region  translateToNotation

    private string TranslateToNotation(ref ChessPiece[,] _board)
    {
        Notation = "";
        int emptySpots;
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            emptySpots = 0;
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                if (_board[y, x] != null)
                {
                    if (emptySpots > 0)
                    {
                        Notation += emptySpots.ToString();
                        emptySpots = 0;
                    }
                    if (_board[y, x].team == TeamColor.White)
                    {
                        Notation += CheckType(_board[y, x]).ToUpper();
                    }
                    else
                    {
                        Notation += CheckType(_board[y, x]).ToLower();
                    }

                    if (y == CHESSBOARD_SIZE_Y - 1)
                    {
                        Notation += "/";

                    }
                }
                else
                {
                    emptySpots += 1;
                    if (emptySpots == CHESSBOARD_SIZE_X)
                    {
                        Notation += emptySpots.ToString();
                        emptySpots = 0;
                    }

                    if (y == CHESSBOARD_SIZE_Y - 1)
                    {
                        if (emptySpots > 0)
                        {
                            Notation += emptySpots.ToString();
                        }
                        Notation += "/";

                    }
                }
            }
        }
        Debug.Log(Notation);
        return null;
    }

    [Button("Check Notation")]
    private void CheckNotatnio()
    {
        TranslateToNotation(ref chessPieces);
    }

    [Button("Check Board")]
    private void CreateFromNotation()
    {
        string testNotatnion = "RNBQKBNR/PPP1PPPP/8/3P4/8/8/pppppppp/rnbqkbnr/";
        ChessPiece[,] _board = TranslateFromNotation(testNotatnion);
        PrintBoard(ref _board);
    }

    private ChessPiece[,] TranslateFromNotation(string _notation)
    {
        ChessPiece[,] _board = new ChessPiece[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];

        int row = 0, collumn = 0;

        foreach (char x in _notation)
        {
            Debug.LogWarning(x);
            if (x == '/')
            {
                row++;
                collumn = 0;
            }
            else
            {
                if (Char.IsDigit(x))
                {
                    collumn += int.Parse(x.ToString()) - 1;
                }
                else
                {
                    Debug.Log(row + " " + collumn);
                    _board[row, collumn] = CheckType(x.ToString());
                    _board[row, collumn].SetPosition(row, collumn);
                    collumn++;
                }
            }
        }
        return _board;
    }

    private string CheckType(ChessPiece _piece)
    {
        string answer = "";
        switch (_piece.type)
        {
            case ChessPieceType.Pawn:
                answer = "P";
                break;
            case ChessPieceType.Rook:
                answer = "R";
                break;
            case ChessPieceType.Knight:
                answer = "N";
                break;
            case ChessPieceType.Bishop:
                answer = "B";
                break;
            case ChessPieceType.Queen:
                answer = "Q";
                break;
            case ChessPieceType.King:
                answer = "K";
                break;
            default:
                break;
        }
        return answer;
    }

    private ChessPiece CheckType(string _type)
    {
        ChessPiece piece = null;
        switch (_type)
        {
            case "P":
                piece = SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.White);
                break;
            case "R":
                piece = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.White);
                break;
            case "N":
                piece = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.White);
                break;
            case "B":
                piece = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.White);
                break;
            case "Q":
                piece = SpanwSinglePiece(ChessPieceType.Queen, TeamColor.White);
                break;
            case "K":
                piece = SpanwSinglePiece(ChessPieceType.King, TeamColor.White);
                break;
            case "p":
                piece = SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.Black);
                break;
            case "r":
                piece = SpanwSinglePiece(ChessPieceType.Rook, TeamColor.Black);
                break;
            case "n":
                piece = SpanwSinglePiece(ChessPieceType.Knight, TeamColor.Black);
                break;
            case "b":
                piece = SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.Black);
                break;
            case "q":
                piece = SpanwSinglePiece(ChessPieceType.Queen, TeamColor.Black);
                break;
            case "k":
                piece = SpanwSinglePiece(ChessPieceType.King, TeamColor.Black);
                break;
            default:
                break;
        }
        return piece;
    }
    #endregion
}
