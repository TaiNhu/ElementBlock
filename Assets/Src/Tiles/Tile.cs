using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer glowSpriteRenderer;
    [SerializeField]
    private Transform starTransform;
    private Sprite baseFloorSprite;
    private DefineData.TilePattern tilePattern;
    public bool isStar = false;
    private ResourcesHolder resourcesHolder;
    [Header("Debug")]
    [SerializeField]
    private Text debugText;
    private Color baseFloorColor;
    void Start()
    {
        resourcesHolder = GameManager.Instance.GetResourcesHolder();
    }

    public void SetResourcesHolder(ResourcesHolder resourcesHolder)
    {
        this.resourcesHolder = resourcesHolder;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetTilePattern(DefineData.TilePattern tilePattern)
    {
        this.tilePattern = tilePattern;
    }

    public void SetFloorSprite(Sprite sprite)
    {
        baseFloorSprite = sprite;
        spriteRenderer.sprite = sprite;
    }

    public void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(1, 1, 1, alpha);
    }

    public void SetBaseFloorColor(Color color)
    {
        spriteRenderer.color = color;
        baseFloorColor = color;
    }

    public void ResetTileItem()
    {
        if (isStar)
        {
            spriteRenderer.sprite = resourcesHolder.starSprites[(int)tilePattern];
        }
        else
        {
            spriteRenderer.sprite = resourcesHolder.tileSprites[(int)tilePattern];
        }
    }

    public void ResetSprite()
    {
        spriteRenderer.sprite = baseFloorSprite;
        spriteRenderer.color = baseFloorColor;
    }

    public void SetGlowSprite(Sprite sprite)
    {
        glowSpriteRenderer.sprite = sprite;
    }

    public void SetSpriteWithAlpha(Sprite sprite, float alpha)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(1, 1, 1, alpha);
    }

    public Vector2 GetRenderSize()
    {
        return spriteRenderer.bounds.size;
    }

    public Vector2 GetSize ()
    {
        return spriteRenderer.size;
    }

    public void GlowEnabled(bool enabled)
    {
        // StopCoroutine("GlowEnabledCoroutine");
        // StartCoroutine("GlowEnabledCoroutine", new object[] { enabled ? 1f : 0f });
        glowSpriteRenderer.DOFade(enabled ? 1f : 0f, 0.1f);
    }

    public void EnableStar(Transform starTransform)
    {
        isStar = true;
        
        spriteRenderer.sprite = resourcesHolder.starSprites[(int)tilePattern];
        this.starTransform = starTransform;
    }

    IEnumerator GlowEnabledCoroutine(object[] data)
    {
        float time = 0.1f;
        float currentTime = 0;
        float target = (float)data[0];
        Color startColor = glowSpriteRenderer.color;
        while (currentTime < time)
        {
            glowSpriteRenderer.color = Color.Lerp(startColor, new Color(1, 1, 1, target), currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        glowSpriteRenderer.color = new Color(1, 1, 1, target);
    }

    public void SetTintColor(Color color)
    {
        spriteRenderer.color = color;

        // if (debugText == null)
        // {
        //     GameObject textObj = new GameObject("ColorText");
        //     textObj.transform.SetParent(GameObject.Find("MainUI").transform, false);
        //     Text text = textObj.AddComponent<Text>();
        //     Canvas canvas = textObj.AddComponent<Canvas>();
        //     canvas.overrideSorting = true;
        //     canvas.sortingLayerName = "EB_GAME";
        //     canvas.sortingOrder = 100;
        //     text.transform.position = transform.position;
        //     text.text = color.r.ToString();
        //     text.alignment = TextAnchor.MiddleCenter;
        //     text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        //     debugText = text;
        // }
        // else
        // {
        //     debugText.text = color.r.ToString();
        // }
    }

    public void SetDebugText(string t)
    {
        if (debugText == null)
        {
            GameObject textObj = new GameObject("ColorText");
            textObj.transform.SetParent(GameObject.Find("MainUI").transform, false);
            Text text = textObj.AddComponent<Text>();
            Canvas canvas = textObj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "EB_GAME";
            canvas.sortingOrder = 100;
            text.transform.position = transform.position;
            text.text = t;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            debugText = text;
        }
        else
        {
            debugText.text = t;
        }
    }

    public DefineData.TilePattern GetTilePattern()
    {
        return tilePattern;
    }

    public void ResetAllTrigger()
    {
        animator.ResetTrigger("Win");
        animator.ResetTrigger("Lose");
    }

    public Transform GetStarTransform()
    {
        return starTransform;
    }

    public void DelayPlayAnimWin()
    {
        ResetAllTrigger();
        animator.SetTrigger("Win");
        animator.SetInteger("Pattern", (int)tilePattern);
        // StopCoroutine("FadeOut");
        // StartCoroutine("FadeOut", 0.2f);
        spriteRenderer.DOFade(0, 0.2f);
        CancelInvoke("SelfDestroy");
        Invoke("SelfDestroy", 2f);
    }
    public void PlayAnimWin(int index)
    {
        CancelInvoke("DelayPlayAnimWin");
        Invoke("DelayPlayAnimWin", 0.04f * index);
    }

    IEnumerator FadeOut(float time = 0.2f)
    {
        float currentTime = 0;
        float target = 0f;
        Color startColor = spriteRenderer.color;
        while (currentTime < time)
        {
            spriteRenderer.color = Color.Lerp(startColor, new Color(1, 1, 1, target), currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;  
        }
        spriteRenderer.color = new Color(1, 1, 1, target);
    }

    public void PlayAnimLose()
    {
        ResetAllTrigger();
        float time = Random.Range(0.5f, 1.5f);
        animator.SetTrigger("Lose");
        spriteRenderer.DOFade(0, time - 0.3f).SetDelay(0.5f);
        spriteRenderer.DOColor(new Color(0.7f, 0.7f, 0.7f), 0.4f);
        float delay = Random.Range(0.6f, 0.8f);
        spriteRenderer.DOFade(0, time - 0.5f).SetDelay(delay);
        spriteRenderer.transform.DOJump(spriteRenderer.transform.position + new Vector3(0.5f, -1.5f, 0), 1.5f, 1, time).SetDelay(delay);
        CancelInvoke("SelfDestroy");
        Invoke("SelfDestroy", time + 1f);

        if (isStar)
        {
            SetStarGrayScale();
            SpriteRenderer starSpriteRenderer = starTransform.GetComponent<SpriteRenderer>();
            starSpriteRenderer.DOFade(0, time).SetDelay(delay);
            starTransform.DOJump(starTransform.position + new Vector3(0.5f, -1.5f, 0), 1.5f, 1, time).SetDelay(delay);
        }
    }


    private void SetStarGrayScale()
    {
        SpriteRenderer starSpriteRenderer = starTransform.GetComponent<SpriteRenderer>();
        Material starMaterial = starSpriteRenderer.material;
        starMaterial.SetFloat("_GrayScale", 0f);
        starMaterial.DOFloat(1f, "_GrayScale", 0.4f);
    }

    IEnumerator TintColor(object[] data)
    {
        float currentTime = 0;
        Color startColor = spriteRenderer.color;
        Color endColor = (Color)data[0];
        float time = (float)data[1];
        while (currentTime < time)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endColor;
    }

    IEnumerator PlayAnimDropItem(float time = 1.5f)
    {
        yield return new WaitForSeconds(Random.Range(0.4f, 0.8f));
        float currentTime = 0;
        Vector2 startPosition = spriteRenderer.transform.localPosition;
        Vector2 endPosition = spriteRenderer.transform.localPosition - new Vector3(0.1f, 2f, 0);
        Quaternion startRotation = spriteRenderer.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, Random.Range(10f, 45f));
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        while (currentTime < time)
        {   
            float t = Mathf.Clamp01(currentTime / time);
            float jumpHeight = 2f * (1 - (2 * t - 1) * (2 * t - 1)); // Parabolic jump motion
            Vector2 currentPos = Vector2.Lerp(startPosition, endPosition, t);
            currentPos.y += jumpHeight; // Add jump height
            spriteRenderer.transform.localPosition = currentPos;
            spriteRenderer.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            currentTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.transform.localPosition = endPosition;
        spriteRenderer.transform.rotation = endRotation;
        spriteRenderer.color = endColor;
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }

    public void AnimatorEnabled()
    {
        animator.enabled = true;
    }

    public void SetAlphaWithAnimation(float alpha)
    {
        StopCoroutine("FadeTo");
        StartCoroutine("FadeTo", alpha);
    }

    IEnumerator FadeTo(float alpha)
    {
        float currentTime = 0;
        float target = alpha;
        float time = 0.2f;
        Color startColor = spriteRenderer.color;
        while (currentTime < time)
        {
            spriteRenderer.color = Color.Lerp(startColor, new Color(1, 1, 1, target), currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(1, 1, 1, target);
    }
}
