using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChessPieceData
{
    public ChessPieceType PieceType = ChessPieceType.Pawn;
    public GameObject PieceModel = null;

    public GameObject GetModel()
    {
        return PieceModel;
    }
}

