using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesHolder", menuName = "ResourcesHolder")]
public class ResourcesHolder : ScriptableObject
{
    public Sprite[] floorSprites;
    public Sprite[] tileSprites;
    public Sprite[] starSprites;
    public Sprite[] glowSprires;
    public CustomShape[] customShapes;
    public Sprite[] congratulationSprites;
    public Sprite[] soundSprites;
    public Sprite[] musicSprites;
    public Sprite[] tickSprites;
    public Sprite[] skillSprites;
    public Sprite[] achievementSprites;
    public Sprite[] medalSprites;
    [Header("Audio")]
    public AudioClip[] sfxClips;
}
