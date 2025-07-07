using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PopupSetting : Popup
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image soundImage;
    [SerializeField] private Image musicImage;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Transform resetButton;
    [SerializeField] private Transform closeButton;
    private bool soundEnable = true;
    private bool musicEnable = true;
    private float lastSoundVolume = 0.5f;
    private float lastMusicVolume = 0.5f;

    public void DoStart()
    {
        resetButton.localScale = Vector3.zero;
        closeButton.localScale = Vector3.zero;
        resetButton.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetDelay(0.6777f);
        closeButton.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }

    public void OnClickSound()
    {
        soundEnable = !soundEnable;
        if (!soundEnable)
        {
            soundSlider.value = 0;
            SoundManager.Instance.ChangeVolumeSfx(0);
        }
        else
        {
            soundSlider.value = lastSoundVolume;
            SoundManager.Instance.ChangeVolumeSfx(lastSoundVolume);
        }
        UpdateImageSound();
    }

    public void UpdateImageSound()
    {
        ResourcesHolder resourcesHolder = GameManager.Instance.GetResourcesHolder();
        if (soundEnable)
        {
            soundImage.sprite = resourcesHolder.soundSprites[0];
        }
        else
        {
            soundImage.sprite = resourcesHolder.soundSprites[1];
        }
    }

    public void UpdateImageMusic()
    {
        ResourcesHolder resourcesHolder = GameManager.Instance.GetResourcesHolder();
        if (musicEnable)
        {
            musicImage.sprite = resourcesHolder.musicSprites[0];
        }
        else
        {
            musicImage.sprite = resourcesHolder.musicSprites[1];
        }
    }

    public void OnEndDragSound()
    {
        if (soundSlider.value != 0)
        {
            lastSoundVolume = soundSlider.value;
            SoundManager.Instance.ChangeVolumeSfx(lastSoundVolume);
        }
    }

    public void OnEndDragMusic()
    {
        if (musicSlider.value != 0)
        {
            lastMusicVolume = musicSlider.value;
            SoundManager.Instance.ChangeVolumeMusic(lastMusicVolume);
        }
    }

    public void OnValueChangedSound()
    {
        if (soundSlider.value == 0)
        {
            if (soundEnable)
            {
                soundEnable = false;
                UpdateImageSound();
            }
        }
        else
        {
            if (!soundEnable)
            {
                soundEnable = true;
                UpdateImageSound();
            }
        }
        SoundManager.Instance.ChangeVolumeSfx(soundSlider.value);
    }

    public void OnValueChangedMusic()
    {
        if (musicSlider.value == 0)
        {
            if (musicEnable)
            {
                musicEnable = false;
                UpdateImageMusic();
            }
        }
        else
        {
            if (!musicEnable)
            {
                musicEnable = true;
                UpdateImageMusic();
            }
        }
        SoundManager.Instance.ChangeVolumeMusic(musicSlider.value);
        UpdateImageMusic();
    }

    public void OnClickMusic()
    {
        musicEnable = !musicEnable;
        if (!musicEnable)
        {
            musicSlider.value = 0;
            SoundManager.Instance.ChangeVolumeMusic(0);
        }
        else
        {
            musicSlider.value = lastMusicVolume;
            SoundManager.Instance.ChangeVolumeMusic(lastMusicVolume);
        }
        UpdateImageMusic();
    }

    public void OnClickReset()
    {
        GameManager.Instance.ReloadGame();
    }

    public void OnClickClose()
    {
        animator.ResetTrigger("Close");
        animator.SetTrigger("Close");
        SoundManager.Instance.PlaySound(SoundManager.SoundName.PanelUp);
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Click);
    }

    public void SelfClose()
    {
        gameObject.SetActive(false);
    }
    
}
