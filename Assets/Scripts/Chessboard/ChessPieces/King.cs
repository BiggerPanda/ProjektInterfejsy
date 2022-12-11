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

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] _board, ref List<Vector2Int[]> _moveList, ref List<Vector2Int> _avaliableMoves)
    {
        Vector2Int[] kingMove = _moveList.Find(m => m[0].x == 4 && m[0].y == ((team == TeamColor.White) ? 0 : 7));
        Vector2Int[] leftRook = _moveList.Find(m => m[0].x == 0 && m[0].y == ((team == TeamColor.White) ? 0 : 7));
        Vector2Int[] rightRook = _moveList.Find(m => m[0].x == 7 && m[0].y == ((team == TeamColor.White) ? 0 : 7));

        if (kingMove == null && position.x == 4)
        {
            if (team == TeamColor.White)
            {
                if (leftRook == null)
                {
                    if (_board[1, 0] == null && _board[2, 0] == null && _board[3, 0] == null)
                    {
                        if (_board[0, 0].team == TeamColor.White && _board[0, 0].type == ChessPieceType.Rook)
                        {
                            _avaliableMoves.Add(new Vector2Int(2, 0));
                            return SpecialMove.Castling;
                        }
                    }
                }

                if (rightRook == null)
                {
                    if (_board[5, 0] == null && _board[6, 0] == null)
                    {
                        if (_board[7, 0].team == TeamColor.White && _board[7, 0].type == ChessPieceType.Rook)
                        {
                            _avaliableMoves.Add(new Vector2Int(6, 0));
                            return SpecialMove.Castling;
                        }
                    }
                }
            }
        }
        else
        {
            if (leftRook == null)
            {
                if (_board[1, 7] == null && _board[2, 7] == null && _board[3, 7] == null)
                {
                    if (_board[0, 7].team == TeamColor.Black && _board[0, 7].type == ChessPieceType.Rook)
                    {
                        _avaliableMoves.Add(new Vector2Int(2, 7));
                        return SpecialMove.Castling;
                    }
                }
            }

            if (rightRook == null)
            {
                if (_board[5, 7] == null && _board[6, 7] == null)
                {
                    if (_board[7, 7].team == TeamColor.White && _board[7, 7].type == ChessPieceType.Rook)
                    {
                        _avaliableMoves.Add(new Vector2Int(6, 7));
                        return SpecialMove.Castling;
                    }
                }
            }
        }

        return SpecialMove.None;
    }
}
