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
            if (((team == TeamColor.White && position.y == 1) || (team == TeamColor.Black && position.y == 6)) && _board[position.x, position.y + (direction * 2)] == null)
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

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] _board, ref List<Vector2Int[]> _moveList, ref List<Vector2Int> _avaliableMoves)
    {

        if ((team == TeamColor.White && position.y == 6) || team == TeamColor.Black && position.y == 1)
            return SpecialMove.Promotion;

        if (_moveList.Count > 0)
        {
            Vector2Int[] _lastMove = _moveList[_moveList.Count - 1];

            if (_board[_lastMove[1].x, _lastMove[1].y].type != ChessPieceType.Pawn)
            {
                return SpecialMove.None;
            }

            if (_board[_lastMove[1].x, _lastMove[1].y].team != team)
            {
                if (Mathf.Abs(_lastMove[0].y - _lastMove[1].y) == 2)
                {
                    if (_lastMove[1].y == position.y)
                    {
                        if (_lastMove[1].x == position.x + 1)
                        {
                            avaliableMoves.Add(new Vector2Int(position.x + 1, position.y + ((team == TeamColor.White) ? 1 : -1)));
                            return SpecialMove.EnPassant;
                        }

                        if (_lastMove[1].x == position.x - 1)
                        {
                            avaliableMoves.Add(new Vector2Int(position.x - 1, position.y + ((team == TeamColor.White) ? 1 : -1)));
                            return SpecialMove.EnPassant;
                        }
                    }
                }
            }
        }
        return SpecialMove.None;
    }
}
