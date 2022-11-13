using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvaliableMoves(ref ChessPiece[,] _board, int _xCount, int _yCount)
    {
        avaliableMoves = new List<Vector2Int>();

        int direction = (team == TeamColor.White) ? 1 : -1;

        if (position.x + 1 < _xCount && position.y + 2 < _yCount)
        {
            if (_board[position.x + 1, position.y + 2] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x + 1, position.y + 2));
            }
            else
            {
                if (_board[position.x + 1, position.y + 2].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x + 1, position.y + 2));
                }
            }
        }

        if (position.x + 1 < _xCount && position.y - 2 >= 0)
        {
            if (_board[position.x + 1, position.y - 2] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x + 1, position.y - 2));
            }
            else
            {
                if (_board[position.x + 1, position.y - 2].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x + 1, position.y - 2));
                }
            }
        }

        if (position.x - 1 >= 0 && position.y + 2 < _yCount)
        {
            if (_board[position.x - 1, position.y + 2] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x - 1, position.y + 2));
            }
            else
            {
                if (_board[position.x - 1, position.y + 2].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x - 1, position.y + 2));
                }
            }
        }

        if (position.x - 1 >= 0 && position.y - 2 >= 0)
        {
            if (_board[position.x - 1, position.y - 2] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x - 1, position.y - 2));
            }
            else
            {
                if (_board[position.x - 1, position.y - 2].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x - 1, position.y - 2));
                }
            }
        }

        if (position.x + 2 < _xCount && position.y + 1 < _yCount)
        {
            if (_board[position.x + 2, position.y + 1] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x + 2, position.y + 1));
            }
            else
            {
                if (_board[position.x + 2, position.y + 1].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x + 2, position.y + 1));
                }
            }
        }

        if (position.x + 2 < _xCount && position.y - 1 >= 0)
        {
            if (_board[position.x + 2, position.y - 1] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x + 2, position.y - 1));
            }
            else
            {
                if (_board[position.x + 2, position.y - 1].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x + 2, position.y - 1));
                }
            }
        }

        if (position.x - 2 >= 0 && position.y + 1 < _yCount)
        {
            if (_board[position.x - 2, position.y + 1] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x - 2, position.y + 1));
            }
            else
            {
                if (_board[position.x - 2, position.y + 1].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x - 2, position.y + 1));
                }
            }
        }

        if (position.x - 2 >= 0 && position.y - 1 >= 0)
        {
            if (_board[position.x - 2, position.y - 1] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x - 2, position.y - 1));
            }
            else
            {
                if (_board[position.x - 2, position.y - 1].team != team)
                {
                    avaliableMoves.Add(new Vector2Int(position.x - 2, position.y - 1));
                }
            }
        }

        return avaliableMoves;
    }
}
