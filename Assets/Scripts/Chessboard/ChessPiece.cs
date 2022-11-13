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
    public TeamColor team = TeamColor.White;

    [SerializeField] protected Material whiteMaterial;
    [SerializeField] protected Material blackMaterial;
    private MeshRenderer pieceRenderer;
    private Material pieceMaterial;
    private Vector3 desiredPosition;
    private Vector3 desiredScale;


    private void Awake()
    {
        pieceRenderer = GetComponentInChildren<MeshRenderer>();
        pieceMaterial = pieceRenderer.material;
    }

    public void SetTeam(TeamColor _team)
    {
        team = _team;
        pieceMaterial = team == TeamColor.White ? whiteMaterial : blackMaterial;
        pieceRenderer.material = pieceMaterial;
    }

    public void SetType(ChessPieceType _type)
    {
        type = _type;
    }

    public void SetPosition(int _x, int _y)
    {
        position = new Vector2Int(_x, _y);
    }
}
