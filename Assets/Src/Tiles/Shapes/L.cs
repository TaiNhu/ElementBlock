using UnityEngine;

public class L : CustomShape
{
    private int horizontalLenght = 3;
    private int verticalLenght = 1;
    private float rotate = 0;
    void Awake()
    {
        tileType = DefineData.TileType.L;
        rotate = Random.Range(0, 3) * (Mathf.PI / 2);
        horizontalLenght = Random.Range(2, 4);
        verticalLenght = Random.Range(1, 3);
    }

    new void Start()
    {
        base.Start();
        CreateTile();
    }

    public void CreateTile()
    {
        lenght = horizontalLenght + verticalLenght;
        tiles = new Tile[lenght];
        for (int i = 0; i < horizontalLenght; i++)
        {
            Tile tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.transform.localPosition = new Vector3((i - (horizontalLenght - 1) * 0.5f) * tile.GetSize().x, 
            (- (verticalLenght) * 0.5f) * tile.GetSize().y, 0);
            tile.transform.localPosition = ConvertPosition(tile.transform.localPosition, rotate);
            tiles[i] = tile;
        }
        for (int i = 0; i < verticalLenght; i++)
        {
            Tile tile = Instantiate(tilePrefab, transform);
            tile.SetTilePattern(tilePattern);
            tile.SetGlowSprite(resourcesHolder.glowSprires[(int)tilePattern]);
            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
            tile.transform.localPosition = new Vector3(- (horizontalLenght - 1) * 0.5f * tile.GetSize().x, 
            (i + 1 - (verticalLenght) * 0.5f) * tile.GetSize().y, 0);
            tile.transform.localPosition = ConvertPosition(tile.transform.localPosition, rotate);
            tiles[horizontalLenght + i] = tile;
        }
    }

    public override Vector2 GetRenderSize()
    {
        Vector2 renderSize = tilePrefab.GetRenderSize();
        if (rotate == Mathf.PI / 2)
        {
            return new Vector2((verticalLenght + 1) * renderSize.x, horizontalLenght * renderSize.y);
        }
        return new Vector2(horizontalLenght * renderSize.x, (verticalLenght + 1) * renderSize.y);
    }
}
