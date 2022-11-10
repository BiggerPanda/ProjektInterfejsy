using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    Pawn = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5
}

public class ChessPiece : MonoBehaviour
{
    public Vector2Int position = Vector2Int.zero;
    public ChessPieceType type = ChessPieceType.Pawn;

    private MeshRenderer pieceRenderer;
    private Material pieceMaterial;


    private void Awake()
    {
        pieceRenderer = GetComponent<MeshRenderer>();
        pieceMaterial = pieceRenderer.material;
    }

    public void SetPosition(int _x, int _y)
    {
        position = new Vector2Int(_x, _y);
    }

}
