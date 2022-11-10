using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileRenderer = null;
    private Vector2Int tilePosition = Vector2Int.zero;
    private Color tileColor = new Color(1, 1, 1, 0.1f);
    private Color tileHighlightColor = new Color(1, 0, 1, 0.5f);

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
    }

    private void OnMouseEnter()
    {
        Highlight();
    }

    private void OnMouseExit()
    {
        Unhighlight();
    }

    public Tile SetPosition(int _x, int _y)
    {
        tilePosition = new Vector2Int(_x, _y);
        return this;
    }

    public void DrawTile(int _x, int _y)
    {
        tileRenderer.material.color = tileColor;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, gameObject.transform.lossyScale);
    }

    public void Highlight()
    {
        tileRenderer.material.color = tileHighlightColor;
    }

    public void Unhighlight()
    {
        DrawTile((int)tilePosition.x, (int)tilePosition.y);
    }

    public Vector2Int GetTilePosition()
    {
        return tilePosition;
    }
}
