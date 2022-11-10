using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class Chessboard : MonoBehaviour
{
    public const int CHESSBOARD_SIZE_X = 8;
    public const int CHESSBOARD_SIZE_Y = 8;

    [SerializeField] private GameObject tileGameObject;

    private Tile[,] tiles = new Tile[CHESSBOARD_SIZE_X, CHESSBOARD_SIZE_Y];

    private void Awake()
    {
        for (int x = 0; x < CHESSBOARD_SIZE_X; x++)
        {
            for (int y = 0; y < CHESSBOARD_SIZE_Y; y++)
            {
                GameObject tile = Instantiate(tileGameObject, new Vector3(x, 0, y), Quaternion.identity);
                tile.transform.parent = transform;
                tiles[x, y] = tile.GetComponent<Tile>();
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
