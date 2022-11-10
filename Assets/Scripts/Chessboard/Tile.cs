using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileRenderer = null;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();

    }

    public void DrawTile(int x, int y)
    {
        if ((x + y) % 2 == 0)
        {
            tileRenderer.material.color = Color.white;
        }
        else
        {
            tileRenderer.material.color = Color.black;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, gameObject.transform.lossyScale);
    }
}
