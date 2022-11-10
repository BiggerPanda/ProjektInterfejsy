using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class Chessboard : MonoBehaviour
{
    public const int CHESSBOARD_SIZE_X = 8;
    public const int CHESSBOARD_SIZE_Y = 8;

    [SerializeField] private float tileSize = 1.3f;
    [SerializeField] private GameObject tileGameObject;

    private Tile[,] tiles = new Tile[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];

    private void Awake()
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                GameObject tile = Instantiate(tileGameObject, new Vector3(tileSize * x, 0, tileSize * y), Quaternion.identity);
                tile.transform.parent = transform;
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                tile.name = $"Tile {x}{y}";
                tiles[x, y] = tile.GetComponent<Tile>().SetPosition(x, y);
            }
        }
    }

    private void Start()
    {
        DrawBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    Debug.Log(tile.GetTilePosition());
                }
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= CHESSBOARD_SIZE_X || y < 0 || y >= CHESSBOARD_SIZE_Y)
        {
            return null;
        }

        return tiles[x, y];
    }

    [Button("Generate chessboard")]
    private void DrawBoard()
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                tiles[x, y].DrawTile(x, y);
            }
        }
    }

}
