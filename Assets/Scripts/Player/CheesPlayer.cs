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

    public void SetupPlayer(TeamColor team)
    {
        playerColor = team;
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
        Debug.Log("MakeRandomMove");
        if (playerChessPieces.Count == 0)
        {
            yield break;
        }

        int randomIndex = Random.Range(0, playerChessPieces.Count);
        ChessPiece randomPiece = playerChessPieces[randomIndex];
        Debug.Log("RandomPiece: " + randomPiece.position);
        if (randomPiece == null)
        {
            yield break;

        }
        ChessPiece[,] _board = chessboard.ChessPieces;
        List<Vector2Int> possibleMoves = randomPiece.GetAvaliableMoves(ref _board, Chessboard.CHESSBOARD_SIZE_X, Chessboard.CHESSBOARD_SIZE_Y);
        Debug.Log("possibleMoves.Count" + possibleMoves.Count);
        if (possibleMoves.Count == 0)
        {
            yield break;

        }

        int randomMoveIndex = Random.Range(0, possibleMoves.Count);
        Vector2Int randomMove = possibleMoves[randomMoveIndex];
        if (randomMove == null)
        {
            yield break;
        }
        Debug.Log("RandomPiece: " + randomPiece.position);
        Debug.Log("RandomMove: " + randomMove);
        chessboard.PlayerMovePiece(ref randomPiece, randomMove.x, randomMove.y);
    }

    public void UpdateChesspieces()
    {
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
}
