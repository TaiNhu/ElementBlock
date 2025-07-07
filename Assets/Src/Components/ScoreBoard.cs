using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private int baseScore = 100;
    [SerializeField]
    private int currentMultiplier = 0;
    [SerializeField]
    private int currentScore = 0;
    [SerializeField]
    private int currentNumStar = 0;
    [Header("UI")]
    [SerializeField]
    private Text textScore;
    [SerializeField]
    private Text[] textMultiplier;
    [SerializeField]
    private Text textHighestScore;
    [SerializeField]
    private Animator animatorMultiplier;
    [SerializeField]
    private ParticleSystem particleMultiplier;
    [SerializeField]
    private Image percentMultiplier;
    [SerializeField]
    private ScoreVisualize scoreVisualize;
    [SerializeField]
    private Transform ParticleMultiplierPrefab;
    [Header("Star Section")]
    [SerializeField]
    private Transform starSection;
    [SerializeField]
    private ParticleSystem starParticles;
    [SerializeField]
    private Text numStar;
    [Header("Skill Section")]
    [SerializeField]
    private Bom skillBom;
    [SerializeField]
    private Laser skillLaser;
    private Tween tweenScore;
    private Tween tweenHighestScore;

    private const string HIGH_SCORE_KEY = "HighScore";
    private const string NUM_STAR_KEY = "NumStar";
    private const string AMOUNT_SKILL_BOM = "AmountSkillBom";
    private const string AMOUNT_SKILL_LASER = "AmountSkillLaser";
    private int highScore = 0;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        LoadHighScore();
        UpdateHighScoreDisplay();
        LoadNumStar();
        UpdateNumStarDisplay();
        LoadAmountSkillBom();
        LoadAmountSkillLaser();
    }

    public void UpdateAmountSkillBom(int amount)
    {
        PlayerPrefs.SetInt(AMOUNT_SKILL_BOM, amount);
        PlayerPrefs.Save();
        GameManager.Instance.SetPlayerSkillBom(amount);
        if (skillBom != null)
        {
            skillBom.SetCanUseSkill(amount > 0);
        }
    }

    public void UpdateAmountSkillLaser(int amount)
    {
        PlayerPrefs.SetInt(AMOUNT_SKILL_LASER, amount);
        PlayerPrefs.Save();
        GameManager.Instance.SetPlayerSkillLaser(amount);
        Debug.Log($"amount: {amount}, skillLaser: {skillLaser}");
        if (skillLaser != null)
        {
            skillLaser.SetCanUseSkill(amount > 0);
        }
    }

    private void LoadAmountSkillBom()
    {
        int skillBomAmount = PlayerPrefs.GetInt(AMOUNT_SKILL_BOM, 0);
        GameManager.Instance.SetPlayerSkillBom(skillBomAmount);
        if (skillBom != null)
        {
            skillBom.SetCanUseSkill(skillBomAmount > 0);
        }
    }

    private void LoadAmountSkillLaser()
    {
        int skillLaserAmount = PlayerPrefs.GetInt(AMOUNT_SKILL_LASER, 0);
        GameManager.Instance.SetPlayerSkillLaser(skillLaserAmount);
        if (skillLaser != null)
        {
            Debug.Log($"skillLaserAmount: {skillLaserAmount}");
            skillLaser.SetCanUseSkill(skillLaserAmount > 0);
        }
    }

    public void MoveStarToStarSection(Transform starTransform)
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundName.StarReceive);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(starTransform.DOMoveY(starSection.position.y, 1f));
        sequence.Join(starTransform.DOMoveX(starSection.position.x, 1.1f).SetEase(Ease.InOutQuad));
        sequence.Join(starTransform.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
        sequence.AppendCallback(() => {
            starParticles.Play();
            currentNumStar++;
            gameManager.SetPlayerNumStar(currentNumStar);
            SaveNumStar();
            UpdateNumStarDisplay();
            Destroy(starTransform.gameObject);
        });
    }

    public void PlayAnimPlusScore(int totalLineWin, CustomShape customShape, Vector3 pos)
    {
        int totalScore = GameManager.Instance.GetTotalScoreWithInfo(customShape, totalLineWin);
        ScoreVisualize scoreVisualize = Instantiate(this.scoreVisualize, transform);
        scoreVisualize.transform.SetParent(transform, false);
        Debug.Log("currentMultiplier: " + currentMultiplier);
        totalScore *= currentMultiplier + 1;
        scoreVisualize.SetScore(totalScore);
        scoreVisualize.SetText(totalLineWin);
        scoreVisualize.SetNumberPos(customShape.transform.position);
        scoreVisualize.SetTextPos(pos);
        scoreVisualize.PlayAnim();
        UpdateScore(totalScore);
        PlayAnimParticleMultiplier(customShape.transform.position);
        if (totalLineWin > 1)
        {
            GameManager.Instance.GetMapManager().ShakeMap();
            gameManager.IncreaseAchievementScore(DefineData.AchievementType.GREAT_COMBO);
        }
        if (totalLineWin > 0)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Match1);
        }
    }

    public void PlayAnimParticleMultiplier(Vector3 pos)
    {
        Transform particleMultiplier = Instantiate(ParticleMultiplierPrefab, transform);
        particleMultiplier.transform.position = pos;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(particleMultiplier.DOMove(transform.position, 0.6f));
        sequence.AppendInterval(0.6f);
        sequence.AppendCallback(() => {
            Destroy(particleMultiplier.gameObject);
        });
    }

    public void UpdateScore(int score)
    {
        currentScore += score;

        ResetTrigger();
        animatorMultiplier.SetTrigger("Play");
        particleMultiplier.Play();

        // Check and update high score if necessary
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            UpdateHighScoreDisplay();
            AnimateHighestScore(highScore, currentScore, 0.5f);
        }
        AnimateNumber(currentScore - score, currentScore, 0.5f);
    }

    void AnimateNumber(int fromValue, int toValue, float duration)
    {
        if (tweenScore != null)
        {
            tweenScore.Kill();
        }
        tweenScore = DOTween.To(() => fromValue, x => fromValue = x, toValue, duration)
               .OnUpdate(() => {
                currentScore = fromValue;
                // gameManager.SetPlayerScore(currentScore);
                textScore.text = fromValue.ToString();
                if (currentMultiplier != Mathf.FloorToInt(Mathf.Sqrt(currentScore / baseScore)))
                {
                    currentMultiplier = Mathf.FloorToInt(Mathf.Sqrt(currentScore / baseScore));
                    for (int i = 0; i < textMultiplier.Length; i++)
                    {
                        textMultiplier[i].text = "x" + (currentMultiplier + 1).ToString();
                    }
                    if (currentMultiplier > 0)
                    {
                        gameManager.IncreaseAchievementScore(DefineData.AchievementType.MULTIPLY);
                    }
                }
                float previusMaxScore = Mathf.Pow(currentMultiplier, 2) * baseScore;
                float maxCurrentScore = Mathf.Pow(currentMultiplier + 1, 2) * baseScore;
                percentMultiplier.fillAmount = (currentScore - previusMaxScore) / (maxCurrentScore - previusMaxScore);
            }).OnComplete(() => {
                PlayerData playerData = gameManager.GetPlayerData();
                GameManager.Instance.IncreaseAchievementScore(DefineData.AchievementType.SCORE, currentScore - playerData.score);
                gameManager.SetPlayerScore(currentScore);
                gameManager.SetPlayerMultiplier(currentMultiplier);
                // Add achievement
            });
    }

    void AnimateHighestScore(int fromValue, int toValue, float duration)
    {
        if (tweenScore != null)
        {
            tweenHighestScore.Kill();
        }
        tweenHighestScore = DOTween.To(() => fromValue, x => fromValue = x, toValue, duration)
               .OnUpdate(() => {
                highScore = fromValue;
                textHighestScore.text = fromValue.ToString();
    }).OnComplete(() => {
        gameManager.SetPlayerHighScore(highScore);
    });
    }

    private void ResetTrigger()
    {
        animatorMultiplier.ResetTrigger("Play");
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        gameManager.SetPlayerHighScore(highScore);
    }

    private void LoadNumStar()
    {
        currentNumStar = PlayerPrefs.GetInt(NUM_STAR_KEY, 0);
        gameManager.SetPlayerNumStarWithoutAchievement(currentNumStar);
    }

    private void SaveNumStar()
    {
        PlayerPrefs.SetInt(NUM_STAR_KEY, currentNumStar);
        PlayerPrefs.Save();
        gameManager.SetPlayerNumStar(currentNumStar);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
        gameManager.SetPlayerHighScore(highScore);
    }

    private void UpdateHighScoreDisplay()
    {
        if (textHighestScore != null)
        {
            textHighestScore.text = highScore.ToString();
        }
    }

    public void UpdateNumStarDisplay()
    {
        if (numStar != null)
        {
            numStar.text = currentNumStar.ToString();
            // GameManager.Instance.UpdateAchievement(DefineData.AchievementType.STAR_COLLECT, currentNumStar);
        }
    }

    public void UpdateNumStarPlayerData(int numStar)
    {
        currentNumStar = numStar;
        UpdateNumStarDisplay();
        SaveNumStar();
    }

    public void UpdateNumStarPlayerData()
    {
        currentNumStar = gameManager.GetPlayerData().numStar;
        UpdateNumStarDisplay();
        SaveNumStar();
    }

    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        PlayerPrefs.Save();
        UpdateHighScoreDisplay();
    }
}
