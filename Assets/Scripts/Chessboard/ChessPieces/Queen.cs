using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvaliableMoves(ref ChessPiece[,] _board, int _xCount, int _yCount)
    {
        avaliableMoves = new List<Vector2Int>();

        int direction = (team == TeamColor.White) ? 1 : -1;

        for (int x = position.x + 1, y = position.y + 1; x < _xCount && y < _yCount; x++, y++)
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
                break;
            }
        }

        for (int x = position.x + 1, y = position.y - 1; x < _xCount && y >= 0; x++, y--)
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
                break;
            }
        }

        for (int x = position.x - 1, y = position.y + 1; x >= 0 && y < _yCount; x--, y++)
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
                break;
            }
        }

        for (int x = position.x - 1, y = position.y - 1; x >= 0 && y >= 0; x--, y--)
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
                break;
            }
        }

        for (int y = position.y - 1; y >= 0; y--)
        {
            if (_board[position.x, y] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x, y));
            }
            else
            {
                if (_board[position.x, y].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x, y));
                }
                break;
            }
        }

        for (int y = position.y + 1; y < _yCount; y++)
        {
            if (_board[position.x, y] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x, y));
            }
            else
            {
                if (_board[position.x, y].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x, y));
                }
                break;
            }
        }

        for (int x = position.x - 1; x >= 0; x--)
        {
            if (_board[x, position.y] == null)
            {
                avaliableMoves.Add(new Vector2Int(x, position.y));
            }
            else
            {
                if (_board[x, position.y].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(x, position.y));
                }
                break;
            }
        }

        for (int x = position.x + 1; x < _xCount; x++)
        {
            if (_board[x, position.y] == null)
            {
                avaliableMoves.Add(new Vector2Int(x, position.y));
            }
            else
            {
                if (_board[x, position.y].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(x, position.y));
                }
                break;
            }
        }



        return avaliableMoves;
    }
}
