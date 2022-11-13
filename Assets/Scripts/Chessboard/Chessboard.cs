using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private float tileSize = 1.3f;
    [SerializeField] private GameObject tileGameObject;
    [SerializeField] private ChessModelData chessData;

    private Tile[,] tiles = new Tile[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
    private ChessPiece[,] chessPieces = new ChessPiece[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];
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
                    curentlyDragged = chessPieces[tile.X, tile.Y];
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = curentlyDragged.position;
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(cameraRay, out hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile == null || chessPieces[tile.X, tile.Y] == null)
                {
                    return;
                }

                bool validMove = MoveTo(curentlyDragged, tile.X, tile.Y);

                if (validMove == false)
                {
                    curentlyDragged.transform.position = tiles[curentlyDragged.position.x, curentlyDragged.position.y].transform.position;
                    curentlyDragged = null;
                }

            }
        }
    }

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

    private void PositionSinglePiece(int _x, int _y, bool force = false)
    {
        if (chessPieces[_x, _y] == null)
        {
            return;
        }

        chessPieces[_x, _y].transform.position = tiles[_x, _y].transform.position;
        tiles[_x, _y].ToggleTaken();
    }


    private ChessPiece SpanwSinglePiece(ChessPieceType _pieceType, TeamColor _team)
    {
        ChessPiece piece = Instantiate(chessData.GetModel(_pieceType), transform).GetComponent<ChessPiece>();
        piece.SetTeam(_team);
        piece.SetType(_pieceType);
        return piece;
    }



    #endregion
}
