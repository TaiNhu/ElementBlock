
using UnityEngine;

public class O : CustomShape
{
    private int radius = 1;
    new void Start()
    {
        base.Start();
        tileType = DefineData.TileType.O;
        radius = Random.Range(1, 4);
        CreateTile();
    }

    public void CreateTile()
    {
        lenght = radius * radius;
        tiles = new Tile[lenght];
        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.SetTilePattern(tilePattern);
                tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
                tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
                tile.transform.localPosition = new Vector3((x - (radius - 1) * 0.5f) * tile.GetSize().x, 
                (y - (radius - 1) * 0.5f) * tile.GetSize().y, 0);
                tiles[x*radius+y] = tile;
            }
        }
    }

    public override Vector2 GetRenderSize()
    {
        Vector2 renderSize = tilePrefab.GetRenderSize();
        return new Vector2(radius * renderSize.x, radius * renderSize.y);
    }
}
