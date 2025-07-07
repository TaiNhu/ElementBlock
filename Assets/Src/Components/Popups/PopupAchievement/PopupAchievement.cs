using UnityEngine;
using UnityEngine.UI;

public class PopupAchievement : Popup
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PopupAchievementElement[] popupAchievementItem;

    public void UpdateAchievement(AchievementManager achievementManager)
    {
        bool isUpdate = false; 
        foreach (var item in popupAchievementItem)
        {
            isUpdate = item.SetData(achievementManager) || isUpdate;
        }
        if (isUpdate)
        {
            GameManager.Instance.TurnOnNotification();
        }
        else
        {
            GameManager.Instance.TurnOffNotification();
        }
    }

    public void OnClickButtonClose()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Close");
        SoundManager.Instance.PlaySound(SoundManager.SoundName.PanelUp);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
    }

    public void SelfClose()
    {
        foreach (var item in popupAchievementItem)
        {
            item.ClearDOTween();
        }
        gameObject.SetActive(false);
    }
}
