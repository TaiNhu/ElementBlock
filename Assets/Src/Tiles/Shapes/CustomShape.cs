using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomShape : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public DefineData.TileType tileType = DefineData.TileType.I;
    public DefineData.ColorType colorType = DefineData.ColorType.Red;
    public DefineData.TilePattern tilePattern = DefineData.TilePattern.Circle;
    protected int lenght = 3;
    protected Tile[] tiles;
    protected ResourcesHolder resourcesHolder;
    public float baseScale = 0.7f;
    public Vector3 startPosition;
    [SerializeField]
    protected Tile tilePrefab;
    public bool CanDrag = false;

    private Vector3 _dragOffset;
    private Tween tweenMove;
    private Tween tweenScale;
    private bool isPointerDown = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanDrag) return;
        if (GameManager.Instance.isLose) 
        {
            CanDrag = false;
            StopTween();
            tweenMove = transform.DOMove(startPosition, 0.1f);
            tweenScale = transform.DOScale(Vector3.one * baseScale, 0.1f);
            return;
        }
        StopTween();
        tweenScale = transform.DOScale(0.9f * Vector3.one, 0.1f);
        GlowItem(true);
        _dragOffset = transform.position - GameManager.Instance.GetMousePosition();
        isPointerDown = true;
        SoundManager.Instance.PlaySound(SoundManager.SoundName.FigureTap);
    }

    public void StopTweenMove()
    {
        if (tweenMove != null)
        {
            tweenMove.Kill();
        }
    }

    public void StopTweenScale()
    {
        if (tweenScale != null)
        {
            tweenScale.Kill();
        }
    }

    public void StopTween()
    {
        StopTweenMove();
        StopTweenScale();
    }

    protected void GlowItem(bool enabled)
    {
        foreach (Tile tile in tiles)
        {
            tile.GlowEnabled(enabled);
        }
    }

    protected IEnumerator ScaleTo(object[] data)
    {
        float time = (float)data[0];
        Vector3 startScale = transform.localScale;
        Vector3 endScale = (Vector3)data[1];
        Transform tile = (Transform)data[2];
        float currentTime = 0;
        while (currentTime < time)
        {
            tile.localScale = Vector3.Lerp(startScale, endScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        tile.localScale = endScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!CanDrag) return;
        if (GameManager.Instance.isLose) 
        {
            CanDrag = false;
            StopTween();
            transform.DOMove(startPosition, 0.1f);
            transform.DOScale(Vector3.one * baseScale, 0.1f);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.FigureFail);
            return;
        }
        // StopCoroutine("ScaleTo");
        GlowItem(false);
        Vector3[] snapItems = GameManager.Instance.CheckValidTile(tiles, tilePattern);
        if (snapItems.Length < 1)
        {
            StopTween();
            transform.DOMove(startPosition, 0.1f);
            transform.DOScale(Vector3.one * baseScale, 0.1f);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.FigureFail);
        }
        else
        {
            CanDrag = false;
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].transform.DOScale(Vector3.one * 1.01f, 0.06f);
                tiles[i].transform.DOMove(snapItems[i], 0.065f);
            }
            CancelInvoke(nameof(MoveToNewParentDelay));
            Invoke(nameof(MoveToNewParentDelay), 0.075f);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.FigureRelease);
        }
        isPointerDown = false;
    }

    public void MoveToNewParentDelay()
    {
        GameManager.Instance.MoveTileToMapTile(this);
        GameManager.Instance.DescreaseAmountTilesSpawn(this);
    }

    protected IEnumerator MoveToNewParent(object[] data)
    {
        float time = (float)data[0];
        Transform tile = (Transform)data[2];
        Vector3 endPosition = (Vector3)data[1];
        yield return StartCoroutine("MoveTo", new object[] { time, endPosition, tile });
    }

    protected IEnumerator MoveTo(object[] data)
    {
        float time = (float)data[0];
        Transform tile = (Transform)data[2];
        Vector3 startPosition = tile.position;
        Vector3 endPosition = (Vector3)data[1];
        float currentTime = 0;
        while (currentTime < time)
        {
            tile.position = Vector3.Lerp(startPosition, endPosition, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        tile.position = endPosition;
    }

    public void ReturnToStartPosition()
    {
        StopCoroutine("MoveTo");
        StartCoroutine("MoveTo", new object[] { 0.1f, startPosition, transform });
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag) return;
        if (GameManager.Instance.isLose) 
        {
            CanDrag = false;
            StopTween();
            tweenMove = transform.DOMove(startPosition, 0.1f);
            tweenScale = transform.DOScale(Vector3.one * baseScale, 0.1f);
            return;
        }
        if (!isPointerDown)
        {
            StopTweenScale();
            tweenScale = transform.DOScale(0.9f * Vector3.one, 0.1f);
            GlowItem(true);
            isPointerDown = true;
        }
        transform.position = GameManager.Instance.GetMousePosition() + _dragOffset;
        GameManager.Instance.CheckValidTile(tiles, tilePattern);
    }
    public void SetTileColor(DefineData.ColorType colorType)
    {
        this.colorType = colorType;
    }

    public void SetTilePattern(DefineData.TilePattern tilePattern)
    {
        this.tilePattern = tilePattern;
    }
    
    protected virtual void Start()
    {
        resourcesHolder = GameManager.Instance.GetResourcesHolder();
        SetTilePattern((DefineData.TilePattern)Random.Range(0, (int)DefineData.TilePattern.LightBolt));
    }

    public void PlayAnimAppear(float delay)
    {
        StopTweenScale();
        tweenScale = transform.DOScale(baseScale * Vector3.one, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
        tweenScale.onComplete = () => {
            CanDrag = true;
        };
    }

    private Vector3 CustomLerp(Vector3 start, Vector3 end, float t)
    {
        return start + (end - start) * t;
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    public virtual Vector2 GetRenderSize()
    {
        return new Vector2(0, 0);
    }

    protected Vector3 ConvertPosition(Vector3 position, float rotate)
    {
        return new Vector3(position.x * Mathf.Cos(rotate) - position.y * Mathf.Sin(rotate), 
        position.x * Mathf.Sin(rotate) + position.y * Mathf.Cos(rotate), 0);
    }

    public Tile[] GetTiles()
    {
        return tiles;
    }

    public void SetAlpha(float alpha)
    {
        foreach (Tile tile in tiles)
        {
            tile.SetAlphaWithAnimation(alpha);
        }
    }

    public void SetTintColor(Color color)
    {
        foreach (Tile tile in tiles)
        {
            tile.SetTintColor(color);
        }
    }

    public int GetLength()
    {
        return tiles.Length;
    }
    
}
