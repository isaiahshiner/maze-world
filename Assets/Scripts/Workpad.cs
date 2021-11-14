using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Workpad : MonoBehaviour
{
    public Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            for (int i = 0; i < tilemap.cellBounds.size.x; i++)
            {
                for (int j = 0; j < tilemap.cellBounds.size.y; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    if (tilemap.HasTile(pos))
                    {
                        tilemap.SetTileFlags(pos, TileFlags.None);
                        tilemap.SetColor(pos, Color.magenta);
                        //return;
                    }
                }
            }
        }
    }
}
