using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvaliableMoves(ref ChessPiece[,] _board, int _xCount, int _yCount)
    {
        avaliableMoves = new List<Vector2Int>();

        int direction = (team == TeamColor.White) ? 1 : -1;

        if (_board[position.x, position.y + direction] == null)
        {
            avaliableMoves.Add(new Vector2Int(position.x, position.y + direction));
        }

        if (_board[position.x, position.y + direction] == null)
        {
            if ((position.y == 1 || position.y == 6) && _board[position.x, position.y + direction * 2] == null)
            {
                avaliableMoves.Add(new Vector2Int(position.x, position.y + (direction * 2)));
            }
        }

        //kill
        if (position.x + 1 < _xCount && _board[position.x + 1, position.y + direction] != null)
        {
            if (_board[position.x + 1, position.y + direction].team != team)
            {
                avaliableMoves.Add(new Vector2Int(position.x + 1, position.y + direction));
            }
        }

        if (position.x - 1 >= 0 && _board[position.x - 1, position.y + direction] != null)
        {
            if (_board[position.x - 1, position.y + direction].team != team)
            {
                avaliableMoves.Add(new Vector2Int(position.x - 1, position.y + direction));
            }
        }

        return avaliableMoves;
    }
}
