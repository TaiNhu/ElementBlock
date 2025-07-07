using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualize : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;
    [SerializeField] private Image images;
    [SerializeField] private Text text;

    public void SetScore(int score)
    {
        text.text = "+" + score.ToString();
    }

    public void PlayAnim()
    {
        Sequence sequence;
        Sequence sequence2;
        Sequence sequence3;
        foreach (Transform item in transforms)
        {
            sequence = DOTween.Sequence();
            sequence.Append(item.DOMoveY(item.position.y + 1.5f, 0.3f));
            sequence.Append(item.DOMoveY(item.position.y + 1.7f, 0.5f).SetDelay(0.2f));
            sequence3 = DOTween.Sequence();
            sequence3.Append(item.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        }
        sequence2 = DOTween.Sequence();
        sequence2.Append(images.DOFade(1, 0.3f));
        sequence2.Append(images.DOFade(0, 0.5f).SetDelay(0.2f));

        sequence2 = DOTween.Sequence();
        sequence2.Append(text.DOFade(1, 0.3f));
        sequence2.Append(text.DOFade(0, 0.5f).SetDelay(0.2f));
        sequence2.OnComplete(() =>
        {
            SelfDestroy();
        });
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    public void SetText(int numLine)
    {
        if (numLine - 2 < 0) 
        {
            images.gameObject.SetActive(false);
            return;
        }
        images.gameObject.SetActive(true);
        images.sprite = GameManager.Instance.GetResourcesHolder().congratulationSprites[Mathf.Min(numLine - 2, 3)];
        switch (numLine)
        {
            case 2:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Good);
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match1);
                break;
            case 3:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Great);
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match2);
                break;
            case 4:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Awesome);
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match3);
                break;
            case 5:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Excellent);
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match4);
                break;
            case 6:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match5);
                break;
            case 7:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match6);
                break;
            case 8:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match7);
                break;
            default:
                SoundManager.Instance.PlaySound(SoundManager.SoundName.Match1);
                break;
        }
    }

    public void SetNumberPos(Vector3 pos)
    {
        text.transform.position = pos;
    }

    public void SetTextPos(Vector3 pos)
    {
        images.transform.position = pos;
    }
}
