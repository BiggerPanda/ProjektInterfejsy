using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjektInterfejsy/ChessModelData")]
public class ChessModelData : ScriptableObject
{
    [SerializeField] public List<ChessPieceData> ChessModelTypes;

    public GameObject GetModel(ChessPieceType _type)
    {
        foreach (ChessPieceData model in ChessModelTypes)
        {
            if (model.PieceType == _type)
            {
                return model.PieceModel;
            }
        }
        return null;
    }
}

