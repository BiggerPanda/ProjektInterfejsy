using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NotationWriter
{
    public string Notation;
    //pgn variables
    public string PGN = "";
    private string pgnMove = "";
    private int move = 1;

    #region  translateToNotation
    private string TranslateToNotation(ref ChessPiece[,] _board)
    {
        Notation = "";
        int emptySpots;
        for (int x = 0; x < Chessboard.CHESSBOARD_SIZE_X; x++)
        {
            emptySpots = 0;
            for (int y = 0; y < Chessboard.CHESSBOARD_SIZE_Y; y++)
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

                    if (y == Chessboard.CHESSBOARD_SIZE_Y - 1)
                    {
                        Notation += "/";

                    }
                }
                else
                {
                    emptySpots += 1;
                    if (emptySpots == Chessboard.CHESSBOARD_SIZE_X)
                    {
                        Notation += emptySpots.ToString();
                        emptySpots = 0;
                    }

                    if (y == Chessboard.CHESSBOARD_SIZE_Y - 1)
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
    private void CheckNotatnio()
    {
        ChessPiece[,] _chessPieces = Chessboard.Instance.ChessPieces;
        TranslateToNotation(ref _chessPieces);
    }

    private void CreateFromNotation()
    {
        string testNotatnion = "RNBQKBNR/PPP1PPPP/8/3P4/8/8/pppppppp/rnbqkbnr/";
        ChessPiece[,] _board = TranslateFromNotation(testNotatnion);
    }

    private ChessPiece[,] TranslateFromNotation(string _notation)
    {
        ChessPiece[,] _board = new ChessPiece[Chessboard.CHESSBOARD_SIZE_X, Chessboard.CHESSBOARD_SIZE_Y];

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
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.White);
                break;
            case "R":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Rook, TeamColor.White);
                break;
            case "N":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Knight, TeamColor.White);
                break;
            case "B":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.White);
                break;
            case "Q":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Queen, TeamColor.White);
                break;
            case "K":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.King, TeamColor.White);
                break;
            case "p":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Pawn, TeamColor.Black);
                break;
            case "r":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Rook, TeamColor.Black);
                break;
            case "n":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Knight, TeamColor.Black);
                break;
            case "b":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Bishop, TeamColor.Black);
                break;
            case "q":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.Queen, TeamColor.Black);
                break;
            case "k":
                piece = Chessboard.Instance.SpanwSinglePiece(ChessPieceType.King, TeamColor.Black);
                break;
            default:
                break;
        }
        return piece;
    }

    public static string placeOnBoardToLetter(int place)
    {
        string letter = "";
        switch (place)
        {
            case 0:
                letter = "a";
                break;
            case 1:
                letter = "b";
                break;
            case 2:
                letter = "c";
                break;
            case 3:
                letter = "d";
                break;
            case 4:
                letter = "e";
                break;
            case 5:
                letter = "f";
                break;
            case 6:
                letter = "g";
                break;
            case 7:
                letter = "h";
                break;
            default:
                break;
        }
        return letter;
    }
    public void writePGNNotation(int _x, int _y, ChessPiece _piece)
    {
        string letterX = NotationWriter.placeOnBoardToLetter(_x);
        string letterY = (_y + 1).ToString();
        if (_piece.type != ChessPieceType.Pawn)
        {
            pgnMove += CheckType(_piece);
        }
        pgnMove += letterX + letterY + " ";
        if (Chessboard.Instance.IsWhiteTurn == false)
        {
            PGN += move.ToString() + ". " + pgnMove;
            move++;
            pgnMove = "";
        }
    }
    #endregion
}

