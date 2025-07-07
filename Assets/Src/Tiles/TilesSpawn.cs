using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TilesSpawn : MonoBehaviour
{
    [SerializeField]
    private int amount = 3;
    private int currentAmount = 3;
    private ResourcesHolder resourcesHolder;
    private CustomShape[] customShapes;

    void Start()
    {
        resourcesHolder = GameManager.Instance.GetResourcesHolder();
        customShapes = new CustomShape[amount];
        SpawnTile();
    }
    public void SetResourcesHolder(ResourcesHolder resourcesHolder)
    {
        this.resourcesHolder = resourcesHolder;
    }

    public void SpawnTile()
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnTileDelay(i);
        }
        CancelInvoke(nameof(CheckAllIsHaveSpaceFor));
        Invoke(nameof(CheckAllIsHaveSpaceFor), 0.5f);
    }

    public void SpawnTileDelay(int i)
    {
        CustomShape customShape = Instantiate(resourcesHolder.customShapes[Random.Range(0, resourcesHolder.customShapes.Length)], transform);
        // CustomShape customShape = Instantiate(resourcesHolder.customShapes[(int)DefineData.TileType.I], transform);
        customShape.baseScale = Mathf.Min(2f / customShape.GetRenderSize().x, 1.7f / customShape.GetRenderSize().y, 0.7f);
        Debug.Log($"customShape.GetRenderSize(): {customShape.GetRenderSize()}");
        customShape.transform.localScale = Vector3.zero;
        customShape.transform.localPosition = Vector3.zero;
        customShape.transform.localPosition += new Vector3((i - (amount - 1) * 0.5f) * 2.3f, 0, 0);
        customShape.startPosition = customShape.transform.position;
        customShapes[i] = customShape;
        // StartCoroutine("DelaySpawnTile", new object[] { i, customShape });
        customShape.PlayAnimAppear(0.1f * i);
        // customShape.transform.DOScale(customShape.baseScale * Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.1f * i).onComplete = () => {
        //     customShape.CanDrag = true;
        // };
    }

    public void UpdateStartPosition()
    {
        for (int i = 0; i < amount; i++)
        {
            customShapes[i].startPosition = customShapes[i].transform.position;
        }
    }

    // public IEnumerator DelaySpawnTile(object[] data)
    // {
    //     // int i = (int)data[0];
    //     // CustomShape customShape = (CustomShape)data[1];
    //     // CustomShape customShape = Instantiate(resourcesHolder.customShapes[Random.Range(0, resourcesHolder.customShapes.Length)], transform);
    //     // // CustomShape customShape = Instantiate(resourcesHolder.customShapes[(int)DefineData.TileType.T], transform);
    //     // customShape.baseScale = Mathf.Min((2f) / customShape.GetRenderSize().x, 3f / customShape.GetRenderSize().y, 0.7f);
    //     // customShape.transform.localScale = Vector3.zero;
    //     // customShape.transform.localPosition = Vector3.zero;
    //     // customShape.transform.localPosition += new Vector3((i - (amount - 1) * 0.5f) * 2.3f, 0, 0);
    //     // customShape.startPosition = customShape.transform.position;
    //     // customShapes[i] = customShape;
    //     // yield return new WaitForSeconds(0.11f * i);
    //     // yield return StartCoroutine("PlayAnimAppear", customShape);
    // }

    // public IEnumerator PlayAnimAppear(CustomShape customShape)
    // {
    //     float time = 0.1f;
    //     float currentTime = 0;
    //     float percent;
    //     while (currentTime < time)
    //     {
    //         percent = currentTime / time;
    //         customShape.transform.localScale = CustomLerp(Vector3.zero, customShape.baseScale * Vector3.one, EaseOutBack(percent));
    //         currentTime += Time.deltaTime;
    //         yield return null;
    //     }
    //     customShape.transform.localScale = customShape.baseScale * Vector3.one;
    //     customShape.CanDrag = true;
    //     yield return new WaitForSeconds(0.1f);
    // }

    public void CheckAllIsHaveSpaceFor()
    {
        if (GameManager.Instance.isLose) return;
        bool isAllDisable = false;
        for (int i = 0; i < amount; i++)
        {
            isAllDisable = CheckIsHaveSpaceFor(customShapes[i]) || isAllDisable;
        }
        if (!isAllDisable)
        {
            GameManager.Instance.PlayAnimLose();
        }
        Debug.Log("GameIsLose: " + isAllDisable);
    }

    public bool CheckIsHaveSpaceFor(CustomShape customShape)
    {
        if (customShape == null)
        {
            return false;
        }
        bool isHaveSpace = GameManager.Instance.IsHaveSpaceFor(customShape.GetTiles());
        if (!isHaveSpace)
        {
            customShape.SetAlpha(0.5f);
            return false;
        }
        else
        {
            customShape.SetAlpha(1f);
            return true;
        }
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


    public void DescreaseAmount(CustomShape customShape)
    {
        if (currentAmount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                if (customShapes[i] == customShape)
                {
                    customShapes[i] = null;
                    break;
                }
            }
            currentAmount--;
            if (currentAmount == 0)
            {
                SpawnTile();
                currentAmount = amount;
            }
        }
    }
}
