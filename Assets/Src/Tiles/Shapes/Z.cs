using UnityEngine;

public class Z : CustomShape
{
    private int horizontalLenght = 2;
    private int verticalLenght = 0;
    private bool isFlip = false;

    new void Start()
    {
        base.Start();
        tileType = DefineData.TileType.Z;
        isFlip = Random.Range(0, 2) == 0;
        CreateTile();
    }    

    public void CreateTile()
    {
        lenght = horizontalLenght * 2 + verticalLenght;
        tiles = new Tile[lenght];
        float y;
        for (int i = 0; i < horizontalLenght * 2; i++)
        {
            Tile tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            if (isFlip)
            {
                y = (i / horizontalLenght < 1 ? tile.GetSize().y : -tile.GetSize().y) * (verticalLenght * 0.5f + 0.5f);
            }
            else
            {
                y = (i / horizontalLenght < 1 ? -tile.GetSize().y : tile.GetSize().y) * (verticalLenght * 0.5f + 0.5f);
            }
            tile.transform.localPosition = new Vector3(
                (i - (horizontalLenght * 2 - 2) * 0.5f) * tile.GetSize().x - (i / horizontalLenght >= 1 ? tile.GetSize().x : 0), 
                y, 0);
            tiles[i] = tile;
        }
        for (int i = 0; i < verticalLenght; i++)
        {
            Tile tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.transform.localPosition = new Vector3(
                0, 
                (i - (verticalLenght - 1) * 0.5f) * tile.GetSize().y, 0);
            tiles[i] = tile;
        }

    }

    public override Vector2 GetRenderSize()
    {
        Vector2 renderSize = tilePrefab.GetComponent<Tile>().GetRenderSize();
        return new Vector2((horizontalLenght * 2 - 1) * renderSize.x, (verticalLenght + 2) * renderSize.y);
    }
    
}
