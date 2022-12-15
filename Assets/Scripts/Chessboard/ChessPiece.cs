//#define Testing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    public bool IsGrabed = false;

    [SerializeField] protected Material whiteMaterial;
    [SerializeField] protected Material blackMaterial;

    private MeshRenderer pieceRenderer;
    private Material pieceMaterial;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    private Collider objectCollider = null;
    protected List<Vector2Int> avaliableMoves;

    private void OnValidate()
    {
        if (pieceRenderer == null)
        {
            pieceRenderer = GetComponentInChildren<MeshRenderer>();
            pieceMaterial = pieceRenderer.sharedMaterial;
        }
    }

    private void Awake()
    {
        pieceRenderer = GetComponentInChildren<MeshRenderer>();
        pieceMaterial = pieceRenderer.material;
        objectCollider = GetComponent<Collider>();
#if Testing
        objectCollider.enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
#endif
    }

    private void Update()
    {
        if (IsGrabed == true)
        {
            return;
        }

        if (transform.localPosition != desiredPosition)
        {
            transform.localPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
        }

        if (transform.localScale != desiredScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10f);
        }
    }

    public virtual List<Vector2Int> GetAvaliableMoves(ref ChessPiece[,] _board, int _xCount, int _yCount)
    {
        avaliableMoves = new List<Vector2Int>();

        avaliableMoves.Add(new Vector2Int(2, 2));
        avaliableMoves.Add(new Vector2Int(2, 3));
        avaliableMoves.Add(new Vector2Int(3, 2));
        avaliableMoves.Add(new Vector2Int(2, 4));


        return avaliableMoves;
    }

    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] _board, ref List<Vector2Int[]> _moveList, ref List<Vector2Int> _avaliableMoves)
    {
        return SpecialMove.None;
    }

    public virtual void SetTeam(TeamColor _team)
    {
        team = _team;
        pieceMaterial = team == TeamColor.White ? whiteMaterial : blackMaterial;
        pieceRenderer.sharedMaterial = pieceMaterial;
    }

    public virtual void SetType(ChessPieceType _type)
    {
        type = _type;
    }

    public virtual void SetPosition(int _x, int _y)
    {
        position = new Vector2Int(_x, _y);
    }

    public virtual void SetPosition(Vector3 _position, bool force = false)
    {
        desiredPosition = _position;
        if (force)
        {
            transform.localPosition = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 _scale, bool force = false)
    {
        desiredScale = _scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }

    public virtual void OnGrab()
    {
#if !Testing
        IsGrabed = true;
        objectCollider.enabled = false;
        Chessboard.Instance.PieceGrabed(gameObject.GetComponent<ChessPiece>());
#endif
    }

    public virtual void OnRelease()
    {
#if !Testing
        objectCollider.enabled = true;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 5f);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit _ray, 5f))
        {
            if (_ray.collider.TryGetComponent<Tile>(out Tile _tile))
            {
                Chessboard.Instance.PieceLeft(_tile);
            }
            else
            {
                Chessboard.Instance.PieceLeft(null);
            }
        }
        IsGrabed = false;
#endif
    }
}
