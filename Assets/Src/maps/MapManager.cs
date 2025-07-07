using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MapManager : MonoBehaviour
{
    
    [SerializeField]
    private int width = 10;
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private Tile tilePrefab;
    [SerializeField]
    private SpriteRenderer starPrefab;
    private ResourcesHolder resourcesHolder;

    private Tile[] tiles;
    private Tile[] tileItems;
    private int[] amountTiles;
    private int[] lastTiles = new int[0];
    private List<int> lastSkillHighlight = new();
    private List<int> lastLaserSkillRange = new();

    // private Transform[] stars;
    private float timeIntervalSpawnStar = 5f;
    private int maxStar = 2;
    [SerializeField]
    private int currentStar = 0;
    private float timeSpawnStar = 0f;
    private List<DefineData.WinDataInfo> listWinInfo = new List<DefineData.WinDataInfo>();
    private void Awake()
    {
        tileItems = new Tile[width * height];
        amountTiles = new int[width * height];
        // stars = new Transform[maxStar];
    }

    private void Start()
    {
        Vector2 canvasSize = GameManager.Instance.GetCanvas().GetComponent<RectTransform>().sizeDelta;
        float scaleMin = Mathf.Min(DefineData.BaseGameSize.y / canvasSize.y, 1f);
        transform.localScale = scaleMin * Vector3.one;
        GameManager.Instance.UpdateTileSpawnStartPosition();
        // Debug.Log($"GameManager.Instance.GetCanvas(): {GameManager.Instance.GetCanvas().scaleFactor}");
        resourcesHolder = GameManager.Instance.GetResourcesHolder();
        GenerateTiles();
    }

    void Update()
    {
        if (timeSpawnStar > timeIntervalSpawnStar && !GameManager.Instance.isLose)
        {
            if (currentStar < maxStar)
            {
                IsSpawnStar();
            }
            timeSpawnStar = 0f;
        }
        timeSpawnStar += Time.deltaTime;
    } 

    bool IsSpawnStar()
    {
        bool isSpawnStar = false;
        Tile tile = null;
        Vector2 postion = Vector2.zero;
        for (int x = 0; x < width * height; x++)
        {
            if (tileItems[x] != null && Random.Range(0, 100) < 30 && !tileItems[x].isStar)
            {
                tile = tileItems[x];
                postion = tile.transform.localPosition;
                isSpawnStar = true;
                break;
            }
        }
        if (!isSpawnStar)
        {
            return false;
        }
        Transform starTransform = SpawnStar(tile.transform);
        tile.EnableStar(starTransform);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.StarAppear);
        return true;
    }

    Transform SpawnStar (Transform parent)
    {
        SpriteRenderer star = Instantiate(starPrefab, parent);
        Transform starTransform = star.transform;
        starTransform.localPosition = Vector3.zero;
        starTransform.localScale = Vector3.one * 2.5f;
        starTransform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        // stars[currentStar] = starTransform;
        currentStar++;

        star.DOFade(1f, 0.3f);
        
        starTransform.DOScale(Vector3.one, 0.6f);
        starTransform.DORotate(Vector3.zero, 0.6f);
        return starTransform;
    }

    public void SetResourcesHolder(ResourcesHolder resourcesHolder)
    {
        this.resourcesHolder = resourcesHolder;
    }
    
    public void GenerateTiles()
    {
        float r;
        Vector2 normalizedXY;
        Vector2 maxVector = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
        float tileSize;
        tiles = new Tile[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                normalizedXY = new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f);
                Tile tile = Instantiate<Tile>(tilePrefab, transform);
                tile.transform.SetParent(transform, true);
                tile.SetFloorSprite(resourcesHolder.floorSprites[Random.Range(0, resourcesHolder.floorSprites.Length)]);
                tileSize = 0.76f;
                tile.transform.localPosition = new Vector3(normalizedXY.x * tileSize, normalizedXY.y * tileSize, 0);
                r = Mathf.Clamp01(Mathf.Pow(1f - (normalizedXY.magnitude / maxVector.magnitude), 4f) + 0.8f);
                if (Mathf.Abs(Mathf.Abs(normalizedXY.x) - Mathf.Abs(normalizedXY.y)) < 1)
                {
                    r = Mathf.Clamp01(Mathf.Pow(1f - (normalizedXY.magnitude / maxVector.magnitude), 4f) + 0.9f);
                }
                tile.SetBaseFloorColor(new Color(r, r, r));
                tile.transform.SetSiblingIndex(0);
                tiles[x+y*width] = tile;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.03f * (x + (height - y - 1)));
            }
        }
        CancelInvoke("DelayOffOverlay");
        Invoke("DelayOffOverlay", (width + height - 1) * 0.03f);
    }

    public void DelayOffOverlay()
    {
        GameManager.Instance.GetPopupManager().HideOverlay();
    }

    public Vector3[] GetSnapItemsByTiles(Tile[] tiles, DefineData.TilePattern tilePattern)
    {
        ResetLastTiles();
        Tile t; 
        int index = GetSnapIndexItemByPosition(tiles[0].transform.position);
        // Debug.Log($"index: {index}");
        if (index < 0 || index >= width * height)
        {
            ResetLastTiles();
            return new Vector3[0];
        }
        this.tiles[index].SetSpriteWithAlpha(resourcesHolder.tileSprites[(int)tilePattern], 0.2f);
        lastTiles = new int[tiles.Length];
        lastTiles[0] = index;
        Vector3[] snapItems = new Vector3[tiles.Length];
        float tileSize = tiles[0].GetSize().x;
        // Debug.Log($"tileSize: {tileSize} tileSize: {tiles[0].GetSize()}");
        snapItems[0] = this.tiles[index].transform.position;
        int[] allSnapItems = new int[tiles.Length];
        allSnapItems[0] = index;
        Vector2 offset;
        for (int j = 1; j < tiles.Length; j++)
        {
            offset = (tiles[j].transform.localPosition - tiles[0].transform.localPosition) / tileSize;
            offset.x = Mathf.RoundToInt(offset.x);
            offset.y = Mathf.RoundToInt(offset.y);
            if ((index % width) + offset.x < 0 || (index % width) + offset.x >= width || Mathf.FloorToInt((index / width) + offset.y) < 0 || Mathf.FloorToInt((index / width) + offset.y) >= height)
            {
                ResetLastTiles();
                return new Vector3[0];
            }
            t = this.tiles[index + (int)offset.x + (int)offset.y * width];
            if (t == null)
            {
                ResetLastTiles();
                return new Vector3[0];
            }
            else
            {
                t.SetSpriteWithAlpha(resourcesHolder.tileSprites[(int)tilePattern], 0.2f);
                lastTiles[j] = index + (int)offset.x + (int)offset.y * width;
                snapItems[j] = t.transform.position;
                allSnapItems[j] = index + (int)offset.x + (int)offset.y * width;
            }
        }
        foreach (int a in allSnapItems)
        {
            if (amountTiles[a] > 0)
            {
                ResetLastTiles();
                return new Vector3[0];
            }
        }
        List<DefineData.WinDataInfo> listWinInfo = GetWinInfo(allSnapItems);
        if (listWinInfo.Count > 0)
        {
            CheckItemWinBehaviour(listWinInfo, tilePattern);
        }
        return snapItems;
    }

    public bool IsHaveSpaceFor(Tile[] tiles)
    {
        Vector2 offset;
        Vector2 tileSize = tiles[0].GetRenderSize();
        int[] isHaveSpace = new int[tiles.Length];
        for (int i = 0; i < this.tiles.Length; i++)
        {
            for(int a = 0; a < tiles.Length; a++)
            {
                isHaveSpace[a] = 0;
            }
            if (amountTiles[i] == 0)
            {
                isHaveSpace[0] = 1;
                for (int j = 1; j < tiles.Length; j++)
                {
                    offset = (tiles[j].transform.position - tiles[0].transform.position) / tileSize;
                    offset.x = Mathf.RoundToInt(offset.x);
                    offset.y = Mathf.RoundToInt(offset.y);
                    if ((i % width) + offset.x < 0 || (i % width) + offset.x >= width || Mathf.FloorToInt((i / width) + offset.y) < 0 || Mathf.FloorToInt((i / width) + offset.y) >= height)
                    {
                        break;
                    }
                    else if (amountTiles[i + (int)offset.x + (int)offset.y * width] != 0)
                    {
                        break;
                    }
                    else if (amountTiles[i + (int)offset.x + (int)offset.y * width] == 0)
                    {
                        isHaveSpace[j] = 1;
                    }
                }
                if (isHaveSpace.All((index) => index == 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void MoveStar(Transform starTransform)
    {
        ParticleSystem particleSystem = starTransform.Find("Particles").GetComponent<ParticleSystem>();
        particleSystem.Play();
        GameManager.Instance.MoveStarToStarSection(starTransform);
    }

    private void CheckItemWinBehaviour(List<DefineData.WinDataInfo> listWinInfo, DefineData.TilePattern tilePattern, bool isWin = false)
    {
        Tile tile;
        foreach (DefineData.WinDataInfo winDataInfo in listWinInfo)
        {
            if (winDataInfo.isRow)
            {
                for (int i = 0; i < width; i++)
                {
                    if (amountTiles[winDataInfo.index * width + i] == 1)
                    {
                        tile = tileItems[winDataInfo.index * width + i];
                        if (tile.isStar)
                        {
                            tile.SetSprite(resourcesHolder.starSprites[(int)tilePattern]);
                        }
                        else
                        {
                            tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
                        }
                        if (isWin)
                        {
                            if (tile.isStar)
                            {
                                Transform starTransform = tile.GetStarTransform();
                                starTransform.SetParent(transform, true);
                                starTransform.SetSiblingIndex(transform.childCount - 2);
                                tile.isStar = false;
                                MoveStar(starTransform);
                                currentStar--;
                            }
                            tile.SetTilePattern(tilePattern);
                            tile.PlayAnimWin(Mathf.Abs(i - winDataInfo.startIndex));
                            tileItems[winDataInfo.index * width + i] = null;
                            amountTiles[winDataInfo.index * width + i] = 0;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    if (winDataInfo.crossIndex != i)
                    {
                        if (amountTiles[winDataInfo.index + i * width] == 1)
                        {
                            tile = tileItems[winDataInfo.index + i * width];
                            if (tile.isStar)
                            {
                                tile.SetSprite(resourcesHolder.starSprites[(int)tilePattern]);
                            }
                            else
                            {
                                tile.SetSprite(resourcesHolder.tileSprites[(int)tilePattern]);
                            }
                            if (isWin)
                            {
                                if (tile.isStar)
                                {
                                    Transform starTransform = tile.GetStarTransform();
                                    starTransform.SetParent(transform, true);
                                    starTransform.SetSiblingIndex(transform.childCount - 2);
                                    tile.isStar = false;
                                    MoveStar(starTransform);
                                    currentStar--;
                                }
                                tile.SetTilePattern(tilePattern);
                                tile.PlayAnimWin(Mathf.Abs(i - winDataInfo.startIndex));
                                tileItems[winDataInfo.index + i * width] = null;
                                amountTiles[winDataInfo.index + i * width] = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    private List<DefineData.WinDataInfo> GetWinInfo(int[] allSnapItems, bool isPlaceTile = false)
    {
        int[] listRowHaveCheck = new int[height];
        int[] listColumnHaveCheck = new int[width];
        // List<DefineData.WinDataInfo> listIndexCanWin = new List<DefineData.WinDataInfo>();
        listWinInfo.Clear();
        bool doneX;
        bool doneY;
        bool isWinRow;
        bool isWinColumn;
        bool isWinRow1 = false;
        bool isWinColumn1 = false;
        HashSet<int> listWinWithOneTile = new HashSet<int>();
        foreach (int a in allSnapItems)
        {
            if (listRowHaveCheck[Mathf.FloorToInt(a / width)] == 0 ||
                listColumnHaveCheck[a % width] == 0
            )
            {
                doneX = false;
                doneY = false;
                isWinRow = listRowHaveCheck[Mathf.FloorToInt(a / width)] == 0;
                isWinColumn = listColumnHaveCheck[a % width] == 0;
                int length = Mathf.Max(listRowHaveCheck[Mathf.FloorToInt(a / width)] == 0 ? width : 0, listColumnHaveCheck[a % width] == 0 ? height : 0);
                listRowHaveCheck[Mathf.FloorToInt(a / width)] = 1;
                listColumnHaveCheck[a % width] = 1;
                for (int i = 0; i < length; i++)
                {
                    if (i == width)
                    {
                        doneX = true;
                    }
                    if (!doneX && isWinRow && amountTiles[Mathf.FloorToInt(a / width) * width + i] < 1 && allSnapItems.All((indexAllSnapItem) => indexAllSnapItem != Mathf.FloorToInt(a / width) * width + i))
                    {
                        isWinRow = false;
                    }
                    if (i == height)
                    {
                        doneY = true;
                    }
                    if (!doneY && isWinColumn && amountTiles[(a % width) + i * width] < 1 && allSnapItems.All((indexAllSnapItem) => indexAllSnapItem != (a % width) + i * width))
                    {
                        isWinColumn = false;
                    }
                }
                if (isWinRow)
                {
                    listWinInfo.Add(new DefineData.WinDataInfo() {isRow = true, index = Mathf.FloorToInt(a / width), crossIndex = -1, startIndex = a % width});
                    isWinRow1 = true;
                    for (int i = 0; i < width; i++)
                    {
                        if (allSnapItems.Any((indexAllSnapItem) => indexAllSnapItem == Mathf.FloorToInt(a / width) * width + i))
                        {
                            listWinWithOneTile.Add(Mathf.FloorToInt(a / width) * width + i);
                        }
                    }
                }
                if (isWinColumn)
                {
                    listWinInfo.Add(new DefineData.WinDataInfo() {isRow = false, index = a % width, crossIndex = (isWinRow == isWinColumn) ? a : -1, startIndex = Mathf.FloorToInt(a / width)});
                    isWinColumn1 = true;
                    for (int i = 0; i < height; i++)
                    {
                        if (allSnapItems.Any((indexAllSnapItem) => indexAllSnapItem == (a % width) + i * width))
                        {
                            listWinWithOneTile.Add((a % width) + i * width);
                        }
                    }
                }
            }
        }
        if (isPlaceTile && (isWinRow1 && isWinColumn1))
        {
            GameManager.Instance.IncreaseAchievementScore(DefineData.AchievementType.CROSS_LINE);
        }
        if (isPlaceTile && listWinWithOneTile.Count == 1 && (isWinRow1 || isWinColumn1))
        {
            GameManager.Instance.IncreaseAchievementScore(DefineData.AchievementType.WIN_WITH_ONE_TILE, isWinRow1 ? width : height);
        }
        return listWinInfo;
    }

    public int GetSnapIndexItemByPosition(Vector2 position)
    {
        Tile tile = tiles[0];
        Vector2 maxVector = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
        Vector2 pos = new Vector2(
            Mathf.RoundToInt((position.x + maxVector.x * this.tiles[0].GetRenderSize().x) / this.tiles[0].GetRenderSize().x),
            Mathf.RoundToInt((position.y - transform.position.y + maxVector.y * this.tiles[0].GetRenderSize().y) / this.tiles[0].GetRenderSize().y)
        );
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y > height)
        {
            return -1;
        }
        int index = Mathf.FloorToInt(pos.x + pos.y * width);
        return index;
    }

    public void ResetLastTiles()
    {
        foreach (int index in lastTiles)
        {
            if (this.tiles[index] != null)
            {
                this.tiles[index].ResetSprite();
                // this.tiles[index].SetAlpha(1f);
            }
        }
        lastTiles = new int[0];
        ClearCheckItemWinBehaviour();
    }

    private void ClearCheckItemWinBehaviour()
    {
        foreach (DefineData.WinDataInfo winDataInfo in listWinInfo)
        {
            if (winDataInfo.isRow)
            {
                for (int i = 0; i < width; i++)
                {
                    if (tileItems[winDataInfo.index * width + i] != null)
                    {
                        tileItems[winDataInfo.index * width + i].ResetTileItem();
                    }
                }
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    if (winDataInfo.crossIndex != i)
                    {
                        if (tileItems[winDataInfo.index + i * width] != null)
                        {
                            tileItems[winDataInfo.index + i * width].ResetTileItem();
                        }
                    }
                }
            }
        }
        listWinInfo.Clear();
    }

    public void MoveTileToMapTile(CustomShape customShape)
    {
        Tile[] tiless = customShape.GetTiles();
        int totalLineWin = 0;
        Vector3 pos = Vector3.zero;
        int[] allSnapItems = new int[customShape.GetTiles().Length];
        for (int i = 0; i < tiless.Length; i++)
        {
            Tile tile = tiless[i];
            int index = GetSnapIndexItemByPosition(tiless[i].transform.position);
            allSnapItems[i] = index;
            Tile t = this.tiles[index];
            if (t != null)
            {
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = t.transform.localPosition;
                tile.transform.SetSiblingIndex(transform.childCount - 2);
                tile.enabled = true;
                tile.GetComponent<BoxCollider2D>().enabled = true;
                tile.AnimatorEnabled();
                this.tileItems[index] = tile;
                amountTiles[index]++;
                tiles[index].ResetSprite();
            }
        }
        List<DefineData.WinDataInfo> listWinInfo = GetWinInfo(allSnapItems, true);
        Debug.Log($"listWinInfo: {listWinInfo.Count}");
        if (listWinInfo.Count > 0)
        {
            CheckItemWinBehaviour(listWinInfo, customShape.tilePattern, true);
            totalLineWin = listWinInfo.Count;
            if (pos == Vector3.zero)
            {
                if (listWinInfo[0].isRow)
                {
                    int randomIndex = Random.Range(0, width);
                    pos = tiles[((listWinInfo[0].startIndex + randomIndex) % width) + 
                        listWinInfo[0].index * width].transform.position;
                }
                else
                {
                    int randomIndex = Random.Range(0, height);
                    pos = tiles[((listWinInfo[0].startIndex + randomIndex) % height) * width + listWinInfo[0].index].transform.position;
                }
            }
        }
        GameManager.Instance.PlayAnimPlusScore(totalLineWin, customShape, pos);
        GameManager.Instance.IncreaseAchievementScore(DefineData.AchievementType.PLACE_TILE, customShape.GetTiles().Length);
        Destroy(customShape.gameObject);
        GameManager.Instance.DelayCheckAllIsHaveSpaceFor(totalLineWin > 0);
   }

    public void PlayAnimLose()
    {
        foreach (Tile tile in tileItems)
        {
            if (tile != null)
            {
                tile.PlayAnimLose();
            }
        }
    }

    public void ShakeMap()
    {
        transform.DOShakePosition(0.3f, 0.2f);
    }

    public void EnableSkill(bool isEnable)
    {
        Tile tile;
        for (int i = 0; i < width * height; i++)
        {
            if (tileItems[i] != null)
            {
                tile = tileItems[i];
                if (isEnable)
                {
                    tile.SetAlpha(0.5f);
                }
                else
                {
                    tile.SetAlpha(1f);
                }
            }
            else
            {
                tile = tiles[i];
                if (isEnable)
                {
                    tile.SetAlpha(0.5f);
                }
                else
                {
                    tile.ResetSprite();
                }
            }
        }
    }

    public void GetBomSkillRange(Vector3 position)
    {
        ResetSkillHighlight();
        int index = GetSnapIndexItemByPosition(position);
        Vector2[] bomSkillRange = new Vector2[0];
        if (index < 0) return;
        bomSkillRange = new Vector2[]
        {
            new (0, 0),
            new (0, 1),
            new (1, 1),
            new (1, 0),
            new (1, -1),
            new (0, -1),
            new (-1, -1),
            new (-1, 0),
            new (-1, 1),
        };
        Vector2 index2D = new Vector2(index % width, Mathf.FloorToInt(index / width));
        Vector2 indexRange2D;
        foreach (Vector2 indexRange in bomSkillRange)
        {
            indexRange2D = new Vector2(index2D.x + indexRange.x, index2D.y + indexRange.y);
            if (indexRange2D.x < 0 
            || indexRange2D.x >= width
            || indexRange2D.y < 0 
            || indexRange2D.y >= height) continue;
            if (tileItems[(int)indexRange2D.x + (int)indexRange2D.y * width] != null)
            {
                tileItems[(int)indexRange2D.x + (int)indexRange2D.y * width].SetAlpha(1);
                lastSkillHighlight.Add((int)indexRange2D.x + (int)indexRange2D.y * width);
            }
            else if (tiles[(int)indexRange2D.x + (int)indexRange2D.y * width] != null)
            {
                tiles[(int)indexRange2D.x + (int)indexRange2D.y * width].SetAlpha(1);
                lastSkillHighlight.Add((int)indexRange2D.x + (int)indexRange2D.y * width);
            }
        }
    }

    public int[] GetIndexBomSkillRange(Vector3 position)
    {
        int index = GetSnapIndexItemByPosition(position);
        int[] bomSkillRange = new int[9]
        {
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1
        };
        if (index < 0) return bomSkillRange;
        Vector2[] bomSkillRange2D = new Vector2[]
        {
            new (0, 0),
            new (0, 1),
            new (1, 1),
            new (1, 0),
            new (1, -1),
            new (0, -1),
            new (-1, -1),
            new (-1, 0),
            new (-1, 1),
        };
        Vector2 index2D = new Vector2(index % width, Mathf.FloorToInt(index / width));
        Vector2 indexRange2D;
        for (int i = 0; i < bomSkillRange2D.Length; i++)
        {
            indexRange2D = new Vector2(index2D.x + bomSkillRange2D[i].x, index2D.y + bomSkillRange2D[i].y);
            if (indexRange2D.x < 0 
            || indexRange2D.x >= width
            || indexRange2D.y < 0 
            || indexRange2D.y >= height) continue;
            if (tileItems[(int)indexRange2D.x + (int)indexRange2D.y * width] != null)
            {
                bomSkillRange[i] = (int)indexRange2D.x + (int)indexRange2D.y * width;
            }
            else if (tiles[(int)indexRange2D.x + (int)indexRange2D.y * width] != null)
            {
                bomSkillRange[i] = (int)indexRange2D.x + (int)indexRange2D.y * width;
            }
            else
            {
                bomSkillRange[i] = -1;
            }
        }
        return bomSkillRange;
    }

    public void ResetSkillHighlight()
    {
        foreach (int index in lastSkillHighlight)
        {
            if (tileItems[index] != null)
            {
                tileItems[index].SetAlpha(0.5f);
            }
            else if (tiles[index] != null)
            {
                tiles[index].SetAlpha(0.5f);
            }
        }
        lastSkillHighlight.Clear();
    }

    public void ClearSkillHighlight()
    {
        foreach (int index in lastSkillHighlight)
        {
            if (tileItems[index] != null)
            {
                tileItems[index].SetAlpha(1);
            }
            else if (tiles[index] != null)
            {
                tiles[index].ResetSprite();
            }
        }
        lastSkillHighlight.Clear();
    }

    public bool UseBomSkill(Vector3 position)
    {
        ClearSkillHighlight();
        int[] bomSkillRange = GetIndexBomSkillRange(position);
        bool isCanUse = false;
        Tile tile;
        foreach (int index in bomSkillRange)
        {
            if (index == -1) continue;
            tile = tileItems[index];
            if (tile != null)
            {
                if (tile.isStar)
                {
                    Transform starTransform = tile.GetStarTransform();
                    starTransform.SetParent(transform, true);
                    starTransform.SetSiblingIndex(transform.childCount - 2);
                    tile.isStar = false;
                    MoveStar(starTransform);
                    currentStar--;
                }
                tile.PlayAnimWin(0);
                tileItems[index] = null;
                amountTiles[index] = 0;
                isCanUse = true;
            }
        }
        return isCanUse;
    }

    public void CollectStar(Tile tile)
    {
        Transform starTransform = tile.GetStarTransform();
        starTransform.SetParent(transform, true);
        starTransform.SetSiblingIndex(transform.childCount - 2);
        tile.isStar = false;
        MoveStar(starTransform);
        currentStar--;
    }

    public void ResetLaserSkillRange()
    {
        foreach (int index in lastLaserSkillRange)
        {
            if (tileItems[index] != null)
            {
                tileItems[index].SetAlpha(0.5f);
            }
            else if (tiles[index] != null)
            {
                tiles[index].ResetSprite();
            }
        }
        lastLaserSkillRange.Clear();
    }

    public void GetLaserSkillRange(Vector3 position)
    {
        ResetLaserSkillRange();
        int index = GetSnapIndexItemByPosition(position);
        if (index < 0) return;
        Tile tile = tileItems[index];
        if (tile == null) return;
        DefineData.TilePattern tilePattern = tile.GetTilePattern();
        for (int i = 0; i < width * height; i++)
        {
            if (tileItems[i] != null)
            {
                if (tileItems[i].GetTilePattern() == tilePattern)
                {
                    tileItems[i].SetAlpha(1);
                    lastLaserSkillRange.Add(i);
                }
            }
        }
    }

    public List<Tile> GetTileLaserSkillRange(Vector3 position)
    {
        int index = GetSnapIndexItemByPosition(position);
        if (index < 0) return new List<Tile>();
        List<Tile> laserSkillRange = new List<Tile>();
        Tile tile = tileItems[index];
        if (tile == null) return new List<Tile>();
        DefineData.TilePattern tilePattern = tile.GetTilePattern();
        laserSkillRange.Add(tile);
        tileItems[index] = null;
        amountTiles[index] = 0;
        for (int i = 0; i < width * height; i++)
        {
            if (tileItems[i] != null)
            {
                if (tileItems[i].GetTilePattern() == tilePattern)
                {
                    if (tileItems[i] != laserSkillRange[0])
                    {
                        laserSkillRange.Insert(1, tileItems[i]);
                        tileItems[i] = null;
                        amountTiles[i] = 0;
                    }
                }
            }
        }
        return laserSkillRange;
    }
}