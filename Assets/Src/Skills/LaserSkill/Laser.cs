using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
public class Laser : BaseSkill
{
    [SerializeField]
    private EffectLaser effectLaser;
    [SerializeField]
    private Transform effectLaserParent;
    [SerializeField]
    private DefineData.SkillName skillName;
    [SerializeField]
    private uint price;
    protected override void OnDragAction()
    {
        GameManager.Instance.CheckLaserSkillRange(transform.position);
    }

    protected override void OnPointerDownAction()
    {
        GameManager.Instance.CheckLaserSkillRange(transform.position);
    }

    protected override void OpenShopToBuySkill()
    {
        GameManager.Instance.OpenShopToBuySkill(skillName, price);
    }

    private EffectLaser SpawnLaserEffect(Vector2 origin, float distance, float corner)
    {
        EffectLaser laserEffect = Instantiate(effectLaser, effectLaserParent);
        laserEffect.transform.position = origin;
        laserEffect.SetScaleBaseOnPreferHeight(distance);
        laserEffect.transform.localRotation = Quaternion.Euler(0, 0, corner);
        return laserEffect;
    }

    protected override void OnPointerUpAction()
    {
        List<Tile> laserSkillRange = GameManager.Instance.GetLaserSkillRange(transform.position);
        if (laserSkillRange.Count <= 0) 
        {
            ReturnToBasePosition();
            return;
        }
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        canDrag = playerData.amountSkillLaser > 0;
        SetCanUseSkill(canDrag);
        List<Vector2> origins = new List<Vector2>();
        Tile tile = laserSkillRange[0];
        Tile tile2;
        Vector2 origin = tile.transform.position;
        if (tile.isStar)
        {
            GameManager.Instance.CollectStar(tile);
        }
        tile.PlayAnimWin(0);

        for (int i = 1; i < laserSkillRange.Count; i+=2)
        {
            if (origins.Count > 0)
            {
                origin = origins[Random.Range(0, origins.Count)];
            }
            tile = laserSkillRange[i];
            if (i + 1 < laserSkillRange.Count)
            {
                tile2 = laserSkillRange[i+1];
            }
            else
            {
                tile2 = null;
            }
            if (tile != null)
            {
                origins.Add(tile.transform.position);
                tile.PlayAnimWin(i / 2 * 3);
                float corner = GetAngle(origin, tile.transform.position);
                float distance = Vector2.Distance(origin, tile.transform.position);
                StartCoroutine(DelayLaserEffect(i / 2 * 2 * 0.1f, origin, distance, corner, tile));
                // tile.SetDebugText(distance.ToString());
            }
            if (tile2 != null)
            {
                origins.Add(tile2.transform.position);
                tile2.PlayAnimWin(i / 2 * 3);
                float distance = Vector2.Distance(origin, tile2.transform.position);
                float corner = GetAngle(origin, tile2.transform.position);
                StartCoroutine(DelayLaserEffect(i / 2 * 2 * 0.1f, origin, distance, corner, tile2));
                // tile.SetDebugText(distance.ToString());
            }
        }

        transform.position = startPosition;
        transform.localScale = Vector3.one;
    }

    IEnumerator DelayLaserEffect(float delay, Vector2 origin, float distance, float corner, Tile tile)
    {
        yield return new WaitForSeconds(delay);
        SpawnLaserEffect(new Vector2(origin.x, origin.y), distance, corner);
        if (tile.isStar)
        {
            GameManager.Instance.CollectStar(tile);
        }
    }

    public float GetAngle(Vector2 origin, Vector2 target)
    {
        Vector2 direction = target - origin;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
    }

    void Start()
    {
        startPosition = transform.position;
    }
}

