using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupShop : Popup
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Text currentStar;
    [SerializeField]
    private RectTransform totalStarRect;
    [SerializeField]
    private RectTransform bgItemRect;
    [SerializeField]
    private Text price;
    [SerializeField]
    private GameObject BuyButton; 
    [SerializeField]
    private GameObject Tick;
    [SerializeField]
    private Image imageSkill;
    [SerializeField]
    private Transform closeButton;
    [SerializeField]
    private RectTransform priceRect;
    [SerializeField]
    private RectTransform sadFace;
    [SerializeField]
    private Image priceStar;
    private DefineData.SkillName skillName;
    private uint priceNumber;

    public void DoStart()
    {
        closeButton.localScale = Vector3.zero;
        closeButton.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f);
        BuyButton.SetActive(true);
        Tick.SetActive(false);
    }
    public void SetInfo(DefineData.SkillName skillName, uint price)
    {
        this.skillName = skillName;
        this.priceNumber = price;
        ResourcesHolder holder = GameManager.Instance.GetResourcesHolder();
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        currentStar.text = playerData.numStar.ToString();
        this.price.text = price.ToString();
        imageSkill.sprite = holder.skillSprites[(byte)skillName];
        CenterTotalStar();
        UpdateSizeBgItem();
    }

    private void UpdateSizeBgItem()
    {
        float width = this.price.preferredWidth;
        bgItemRect.sizeDelta = new Vector2(width + 300, bgItemRect.sizeDelta.y);
        float totalWidth = 0;
        foreach (Transform child in priceRect)
        {
            Text text = child.GetComponent<Text>(); 
            if (text != null)
            {
                totalWidth += text.preferredWidth;
            }
            else
            {
                totalWidth += child.GetComponent<RectTransform>().rect.width;
            }
        }
        float padding = totalWidth / 2;
        priceRect.anchoredPosition = new Vector2(padding, priceRect.anchoredPosition.y);
    }
    private void CenterTotalStar()
    {
        float totalWidth = 0;
        Text text;
        foreach (Transform child in totalStarRect)
        {
            text = child.GetComponent<Text>();
            if (text != null)
            {
                totalWidth += text.preferredWidth;
            }
            else
            {
                totalWidth += child.GetComponent<RectTransform>().rect.width;
            }
        }
        totalStarRect.anchoredPosition = new Vector2(-totalWidth / 2, 0);
        sadFace.anchoredPosition = new Vector2(totalWidth / 2 + 20, sadFace.anchoredPosition.y);
    }

    public void OnClickClose()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Close");
        SoundManager.Instance.PlaySound(SoundManager.SoundName.PanelUp);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
    }

    public void OnClickBuy()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        if (playerData.numStar < priceNumber)
        {
            animator.ResetTrigger("Error");
            animator.SetTrigger("Error");
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Error);
            return;
        }
        GameManager.Instance.ConfirmBuySkill(skillName, priceNumber);
        currentStar.text = playerData.numStar.ToString();
        OnClickClose();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Buying);
        PurchaseSuccess();
    }

    public void PurchaseSuccess()
    {
        BuyButton.SetActive(false);
        Tick.SetActive(true);
    }

    public void SelfClose()
    {
        gameObject.SetActive(false);
    }
}

