using UnityEngine;
using UnityEngine.UI;

public class EffectLaser : MonoBehaviour
{
    [SerializeField]
    private Image image;
    void Start()
    {
        Destroy(gameObject, 0.45f);
    }

    public void SetScaleBaseOnPreferHeight(float preferHeight)
    {
        Debug.Log($"preferHeight: {preferHeight}, image.rectTransform.rect.height: {image.rectTransform.rect.height}");
        image.transform.localScale = new Vector3(1, preferHeight / (image.rectTransform.rect.height * 0.01f), 1);
    }
}
