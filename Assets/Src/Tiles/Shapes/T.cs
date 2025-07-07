using UnityEngine;

public class T : CustomShape
{
    private int horizontalLenght = 3;
    private int verticalLenght = 1;

    protected override void Start()
    {
        base.Start();
        lenght = horizontalLenght + verticalLenght;
        tileType = DefineData.TileType.T;
        CreateTile();
    }

    public void CreateTile()
    {
        tiles = new Tile[lenght];
        int l = horizontalLenght;
        Tile tile;
        for (int i = 0; i < l; i++)
        {
            tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.transform.localPosition = new Vector3((i - (l - 1) * 0.5f) * tile.GetSize().x, -tile.GetSize().y * ((verticalLenght - 1) * 0.5f + 0.5f), 0);
            tiles[i] = tile;
        }
        l = verticalLenght;
        for (int i = 0; i < l; i++)
        {
            tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.transform.localPosition = new Vector3(0, (-tile.GetSize().y * ((verticalLenght - 1) * 0.5f + 0.5f)) + ((i + 1) * tile.GetSize().y), 0);
            tiles[horizontalLenght + i] = tile;
        }
        // StopCoroutine("PlayAnimAppear");
        // StartCoroutine("PlayAnimAppear");
    }

    public override Vector2 GetRenderSize()
    {
        Vector2 renderSize = tilePrefab.GetComponent<Tile>().GetRenderSize();
        return new Vector2(horizontalLenght * renderSize.x, (verticalLenght + 1) * renderSize.y);
    }
    
}
