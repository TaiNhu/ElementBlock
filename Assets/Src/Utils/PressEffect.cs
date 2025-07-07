using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressEffect : MonoBehaviour
{
    Vector3 baseScale { get; set; } = Vector3.one;
    public bool isEnabled = true;
    public float scaleOffset = 0.9f;

    public GameObject objectWantAnimate;

    void Awake()
    {
        if (objectWantAnimate.IsUnityNull())
        {
            baseScale = transform.localScale;
        }
        else
        {
            baseScale = objectWantAnimate.transform.localScale;
        }
    }
    void Start()
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger.IsUnityNull())
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener((data) => { OnPointerDownDelegate(); });
        trigger.triggers.Add(entry);
        entry = new()
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((data) => { OnPointerUpDelegate(); });
        trigger.triggers.Add(entry);
    }
    void OnPointerDownDelegate()
    {
        if (isEnabled == false) return;
        
        // if (SoundManager.Instance)
        // {
        //     SoundManager.Instance.PlayAudioClip(NAMESOUND.SoundClick);
        // }

        if (objectWantAnimate.IsUnityNull())
        {
            PlayAnimDown(transform, 0.1F);
            // transform.localScale = scaleOffset * baseScale;
        }
        else
        {
            PlayAnimDown(objectWantAnimate.transform, 0.1F);
            // objectWantAnimate.transform.localScale = scaleOffset * baseScale;
        }
    }
    void OnPointerUpDelegate()
    {
        if (isEnabled == false) return;

        if (objectWantAnimate.IsUnityNull())
        {
            PlayAnimUp(transform, 0.1F);
            // transform.localScale = baseScale;
        }
        else
        {
            PlayAnimUp(objectWantAnimate.transform, 0.1F);
            // objectWantAnimate.transform.localScale = baseScale;
        }
    }

    void PlayAnimDown(Transform transform, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleDown(transform, duration));
    }

    void PlayAnimUp(Transform transform, float duration)
    {

        StopAllCoroutines();
        StartCoroutine(ScaleUp(transform, duration));
    }

    IEnumerator ScaleDown(Transform transform, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    IEnumerator ScaleUp(Transform transform, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = baseScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}