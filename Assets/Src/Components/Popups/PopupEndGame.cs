using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PopupEndGame : Popup
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text starText;
    [SerializeField] private Text highScoreText;
    private bool isTweenScore = false;
    private bool isTweenStar = false;
    public void OnClickRestart()
    {
        GameManager.Instance.ReloadGame();
    }

    void Start()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        highScoreText.text = playerData.highScore.ToString();
    } 

    public void TweenScore()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        int startScore = 0;
        DOTween.To(() => startScore, x => startScore = x, playerData.score, 1.5f).OnUpdate(() => {
            scoreText.text = startScore.ToString();
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Couting, 0.5f);
        });
    }

    public void TweenStar()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        int startStar = 0;
        DOTween.To(() => startStar, x => startStar = x, playerData.numStar, 1.5f).OnUpdate(() => {
            starText.text = startStar.ToString();
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Couting, 0.5f);
        });
    }
}
