using UnityEngine;

public class Bom : BaseSkill
{
    [SerializeField]
    private GameObject _effect;
    [SerializeField]
    private uint price;
    [SerializeField]
    private DefineData.SkillName skillName;

    protected override void OnDragAction()
    {
        GameManager.Instance.CheckBomSkillRange(transform.position);
    }

    protected override void OnPointerDownAction()
    {
        GameManager.Instance.CheckBomSkillRange(transform.position);
    }

    protected override void OpenShopToBuySkill()
    {
        GameManager.Instance.OpenShopToBuySkill(skillName, price);
    }

    private void HideEffect()
    {
        _effect.SetActive(false);
    }

    protected override void OnPointerUpAction()
    {
        bool isCanUse = GameManager.Instance.UseBomSkill(transform.position);
        if (isCanUse)
        {
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            canDrag = playerData.amountSkillBom > 0;
            _effect.SetActive(true);
            _effect.transform.position = transform.position;
            transform.position = startPosition;
            transform.localScale = Vector3.one;
            CancelInvoke(nameof(HideEffect));
            Invoke(nameof(HideEffect), 0.7f);
            GameManager.Instance.GetMapManager().ShakeMap();
            SetCanUseSkill(canDrag);
        }
        else
        {
            ReturnToBasePosition();
        }
    }

    void Start()
    {
        startPosition = transform.position;
    } 
}
