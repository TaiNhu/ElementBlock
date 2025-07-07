using UnityEditor;
using UnityEngine;

public class I : CustomShape
{
    private float rotate = 0;

    void Awake()
    {
        lenght = Random.Range(4, 6);
        // if (lenght >= 4)
        // {
        //     rotate = Mathf.PI / 2;
        // }
        // else
        // {
            rotate = Random.Range(0, 2) * (Mathf.PI / 2);
        // }
    } 
    protected override void Start()
    {
        base.Start();
        // lenght = Random.Range(2, 6);
        // lenght = Random.Range(4, 6);
        // if (lenght >= 4)
        // {
        //     rotate = Mathf.PI / 2;
        // }
        // else
        // {
        //     rotate = Random.Range(0, 2) * (Mathf.PI / 2);
        // }
        CreateTile();
    }

    public void CreateTile()
    {
        tiles = new Tile[lenght];
        for (int i = 0; i < lenght; i++)
        {
            Tile tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.transform.localPosition = new Vector3(0, (i - (lenght - 1) * 0.5f) * tile.GetSize().y, 0);
            tile.transform.localPosition = ConvertPosition(tile.transform.localPosition, rotate);
            tiles[i] = tile;
        }
    }

    public override Vector2 GetRenderSize()
    {
        Vector2 renderSize = tilePrefab.GetRenderSize();
        if (rotate == Mathf.PI / 2)
        {
            return new Vector2(lenght * renderSize.x, renderSize.y);
        }
        else
        {
            return new Vector2(renderSize.x, lenght * renderSize.y);
        }
    }
}
