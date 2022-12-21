using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CheesPlayer : MonoBehaviour
{
    public TeamColor playerColor;
    public bool isBot = false;
    protected List<ChessPiece> playerChessPieces;
    protected Chessboard chessboard => Chessboard.Instance;

    public void SetupPlayer()
    {
        playerColor = TeamColor.Black;
        playerChessPieces = new List<ChessPiece>();
        ChessPiece[,] chessPieces = chessboard.ChessPieces;
        for (int x = 0; x < Chessboard.CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < Chessboard.CHESSBOARD_SIZE_Y; y++)
            {
                if (chessPieces[x, y] == null)
                {
                    continue;
                }

                if (chessPieces[x, y].team != playerColor)
                {
                    continue;
                }

                playerChessPieces.Add(chessPieces[x, y]);
            }
        }
    }

    [Button("RandomMove")]
    public IEnumerator MakeRandomMove()
    {
        if (playerChessPieces.Count == 0)
        {
            yield return null;
        }

        int randomIndex = Random.Range(0, playerChessPieces.Count);
        ChessPiece randomPiece = playerChessPieces[randomIndex];
        if (randomPiece == null)
        {
            yield return null;

        }
        ChessPiece[,] _board = chessboard.ChessPieces;
        List<Vector2Int> possibleMoves = randomPiece.GetAvaliableMoves(ref _board, Chessboard.CHESSBOARD_SIZE_X, Chessboard.CHESSBOARD_SIZE_Y);
        if (possibleMoves.Count == 0)
        {
            yield return null;

        }

        int randomMoveIndex = Random.Range(0, possibleMoves.Count);
        Vector2Int randomMove = possibleMoves[randomMoveIndex];
        if (randomMove == null)
        {
            yield return null;
        }

        chessboard.PlayerMovePiece(ref randomPiece, randomMove.x, randomMove.y);
    }
}
