using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundName
    {
        Achievement,
        Awesome,
        Block,
        Bomb,
        Buying,
        Click,
        Couting,
        Error,
        Excellent,
        FigureFail,
        FigureRelease,
        FigureTap,
        Fire,
        Good,
        Great,
        HeartBeat,
        LevelUp,
        Lighting,
        Lose,
        Match1,
        Match2,
        Match3,
        Match4,
        Match5,
        Match6,
        Match7,
        Panel,
        PanelUp,
        Revive,
        Reward,
        Stamp,
        StarAppear,
        StarReceive
    }
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private ResourcesHolder resourcesHolder;
    [SerializeField] private float volumeSfx = 1f;
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resourcesHolder = GameManager.Instance.GetResourcesHolder();
    }

    public void PlaySound(SoundName soundName, float volume = 1f)
    {
        if (volume == 1)
        {
            volume = volumeSfx;
        }
        sfxAudioSource.PlayOneShot(resourcesHolder.sfxClips[(int)soundName], volume);
    }

    public void StopSound(SoundName soundName)
    {
        sfxAudioSource.Stop();
    }

    public void ChangeVolumeSfx(float volume)
    {
        sfxAudioSource.volume = volume;
    }

    public void ChangeVolumeMusic(float volume)
    {
        musicAudioSource.volume = volume;
    }
}