using UnityEngine;
using UnityEngine.UI;
public class AutoChangeSizeFitText : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float minWidthSize;
    [SerializeField] private float maxWidthSize;
    [SerializeField] private float minHeightSize;
    [SerializeField] private float maxHeightSize;
    [SerializeField] private Image background;
    [SerializeField] private bool needRealTime = false;
    [SerializeField] private Vector2 padding;


    void Start ()
    {
        UpdateSize();
    }

    void Update()
    {
        if (needRealTime)
        {
            UpdateSize();
        }
    }

    void UpdateSize()
    {
        float preferredWidth = Mathf.Clamp(text.preferredWidth, minWidthSize, maxWidthSize);
        float preferredHeight = Mathf.Clamp(text.preferredHeight, minHeightSize, maxHeightSize);
        Vector2 sizeDelta = new Vector2(preferredWidth + padding.x, preferredHeight + padding.y);
        background.rectTransform.sizeDelta = sizeDelta;
    }
}
