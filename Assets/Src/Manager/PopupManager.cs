using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject overlay;
    public enum PopupName
    {
        PopupEndGame,
        PopupSetting,
        PopupShop,
        PopupAchievement
    }
    [SerializeField]
    private PopupEndGame popupEndGame;
    [SerializeField]
    private PopupSetting popupSetting;
    [SerializeField]
    private PopupShop popupShop;
    [SerializeField]
    private PopupAchievement popupAchievement;
    public void ShowPopupSetting()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Panel);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
        popupSetting.gameObject.SetActive(true);
    }

    public void ShowPopupShop(DefineData.SkillName skillName, uint price)
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Panel);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
        popupShop.gameObject.SetActive(true);
        popupShop.SetInfo(skillName, price);
    }

    public void ShowPopupEndGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Panel);
        popupEndGame.gameObject.SetActive(true);
    }

    public void ShowPopupAchievement()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Panel);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
        popupAchievement.gameObject.SetActive(true);
    }

    public void HideOverlay()
    {
        overlay.SetActive(false);
    }

    public void ShowOverlay()
    {
        overlay.SetActive(true);
    }

    public void UpdateAchievement(AchievementManager achievementManager)
    {
        popupAchievement.UpdateAchievement(achievementManager);
    }
}
