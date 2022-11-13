using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvaliableMoves(ref ChessPiece[,] _board, int _xCount, int _yCount)
    {
        avaliableMoves = new List<Vector2Int>();

        int direction = (team == TeamColor.White) ? 1 : -1;

        for (int x = position.x - 1; x <= position.x + 1; x++)
        {
            for (int y = position.y - 1; y <= position.y + 1; y++)
            {
                if (x >= 0 && x < _xCount && y >= 0 && y < _yCount)
                {
                    if (_board[x, y] == null)
                    {
                        avaliableMoves.Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        if (_board[x, y].team != team)
                        {
                            avaliableMoves.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }


        return avaliableMoves;
    }
}
