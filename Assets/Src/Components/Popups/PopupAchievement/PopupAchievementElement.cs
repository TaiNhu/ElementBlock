using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PopupAchievementElement : MonoBehaviour
{
    [SerializeField] private Image itemBackground;
    [SerializeField] private int targetScore;
    [SerializeField] private int currentScore;
    [SerializeField] private int maximumClaimCount;
    [SerializeField] private int currentClaimCount;
    [SerializeField] private int baseReward;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text gaugeIndicator;
    [SerializeField] private Image claimMedal;
    [SerializeField] private GameObject ButtonClaim;
    [SerializeField] private Text reward;
    [SerializeField] private GameObject MaxText;
    [SerializeField] private GameObject ProgressBar;
    [SerializeField] private DefineData.AchievementType achievementType;
    private Sequence tweenButtonClaim;

    public bool SetData(AchievementManager achievementManager)
    {
        AchievementManager.AchievementInfo achievementInfo = achievementManager.achievementInfos[achievementType];
        currentScore = achievementInfo.currentScore;
        currentClaimCount = achievementInfo.amountClaim;

        float scorePerClaim = targetScore / maximumClaimCount;

        bool canClaim = currentClaimCount < Mathf.Clamp(Mathf.FloorToInt(currentScore / scorePerClaim), 0, maximumClaimCount);

        int canClaimNumber = Mathf.Clamp(Mathf.FloorToInt(currentScore / scorePerClaim), 0, maximumClaimCount) - currentClaimCount;

        if (canClaim)
        {
            ProgressBar.SetActive(false);
            ButtonClaim.SetActive(true);
            reward.gameObject.SetActive(true);
            reward.text = $"{baseReward * canClaimNumber}";
            ButtonClaim.GetComponent<Button>().interactable = true;
            ButtonClaim.GetComponent<PressEffect>().isEnabled = true;
            MaxText.SetActive(false);

            if (tweenButtonClaim != null)
            {
                tweenButtonClaim.Kill();
            }
            tweenButtonClaim = DOTween.Sequence();
            tweenButtonClaim.Append(ButtonClaim.transform.DOScale(1.05f, 0.3f));
            tweenButtonClaim.Append(ButtonClaim.transform.DOScale(0.95f, 0.3f));
            // tweenButtonClaim.Append(ButtonClaim.transform.DOScale(1.05f, 0.3f));
            tweenButtonClaim.SetLoops(-1, LoopType.Yoyo);
        }
        else if (currentClaimCount >= maximumClaimCount)
        {
            ProgressBar.SetActive(false);
            ButtonClaim.SetActive(true);
            reward.gameObject.SetActive(false);
            MaxText.SetActive(true);
            ButtonClaim.GetComponent<Button>().interactable = false;
            ButtonClaim.GetComponent<PressEffect>().isEnabled = false;
            tweenButtonClaim.Kill();
        }
        else
        {
            ProgressBar.SetActive(true);
            ButtonClaim.SetActive(false);
            reward.gameObject.SetActive(false);
            ButtonClaim.GetComponent<Button>().interactable = false;
            ButtonClaim.GetComponent<PressEffect>().isEnabled = false;
            tweenButtonClaim.Kill();
        }
        progressBar.fillAmount = (float)(currentScore % scorePerClaim) / scorePerClaim;
        gaugeIndicator.text = $"{currentScore % scorePerClaim}/{scorePerClaim}";

        ResourcesHolder resourcesHolder = GameManager.Instance.GetResourcesHolder();
        UpdateMedal(currentClaimCount);

        itemBackground.sprite = resourcesHolder.achievementSprites[(int)achievementType * 2 + (achievementInfo.amountClaim > 0 ? 0 : 1)];
        itemBackground.SetNativeSize();

        return canClaim;
    }

    public void UpdateMedal(int amountClaim, bool isPlayAnimClaim = false)
    {
        ResourcesHolder resourcesHolder = GameManager.Instance.GetResourcesHolder();
        claimMedal.sprite = resourcesHolder.medalSprites[Mathf.Clamp(amountClaim - 1, 0, resourcesHolder.medalSprites.Length - 1)];
        claimMedal.SetNativeSize();
        if (amountClaim >= 1)
        {
            claimMedal.gameObject.SetActive(true);
            if (isPlayAnimClaim)
            {
                claimMedal.transform.localScale = Vector3.zero;
                claimMedal.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }
        }
        else
            claimMedal.gameObject.SetActive(false);
    }

    public void OnClickButtonClaim()
    {
        // Debug.Log("OnClickButtonClaim");
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Reward);
        tweenButtonClaim.Kill();
        ButtonClaim.transform.localScale = Vector3.one;
        float scorePerClaim = targetScore / maximumClaimCount;
        int canClaimNumber = Mathf.Clamp(Mathf.FloorToInt(currentScore / scorePerClaim), 0, maximumClaimCount);
        // Debug.Log($"baseReward: {baseReward} canClaimNumber: {canClaimNumber} currentClaimCount: {currentClaimCount}");
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        GameManager.Instance.UpdateNumStarPlayerData(playerData.numStar + baseReward * (canClaimNumber - currentClaimCount));
        GameManager.Instance.ClaimAchievement(achievementType, canClaimNumber - currentClaimCount);
        currentClaimCount += canClaimNumber - currentClaimCount;
        UpdateMedal(currentClaimCount, true);
    }

    public void ClearDOTween()
    {
        tweenButtonClaim.Kill();
    }
}
