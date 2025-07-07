using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Resources")]
    [SerializeField]
    private ResourcesHolder resourcesHolder;
    [Header("Instance")]
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private TilesSpawn tilesSpawn;
    [SerializeField]
    private ScoreBoard scoreBoard;
    [SerializeField]
    private PopupManager popupManager;
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private AchievementManager achievementManager;
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject notification;

    public bool isLose = false;
    private Camera _camera;
    private int fps = 121;
    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;
        Application.targetFrameRate = fps;
        playerData = new PlayerData();
        achievementManager = new ();
        LoadAchivement();
    }

    public void UpdateAchievement(DefineData.AchievementType achievementType, int value)
    {
        achievementManager.AddScore(achievementType, value);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void ClaimAchievement(DefineData.AchievementType achievementType)
    {
        achievementManager.AddAmountClaim(achievementType);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void ClaimAchievement(DefineData.AchievementType achievementType, int value)
    {
        achievementManager.AddAmountClaim(achievementType, value);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void LoadAchivement()
    {
        try 
        {
            AchievementManager achievementManagerOnLoad = SaveAndLoadFile.LoadData<AchievementManager>("achievement", new AchievementManager());
            achievementManager.InitPreloadData(achievementManagerOnLoad.achievementInfos, Debug.Log);
            popupManager.UpdateAchievement(achievementManager);
            Debug.Log($"LoadAchivement {JsonConvert.SerializeObject(achievementManager.achievementInfos)}");
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.LogError("LoadAchivement: " + e.Message);
        }
    }

    public void SetPlayerScore(int score)
    {
        playerData.score = score;
    }

    public void SetPlayerHighScore(int highScore)
    {
        playerData.highScore = highScore;
        achievementManager.AddScore(DefineData.AchievementType.BEST_SCORE, highScore);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void SetPlayerMultiplier(int multiplier)
    {
        playerData.multiplier = multiplier;
    }

    public void SetPlayerNumStar(int numStar)
    {
        achievementManager.IncreaseScore(DefineData.AchievementType.STAR_COLLECT, numStar - playerData.numStar);
        playerData.numStar = numStar;
        popupManager.UpdateAchievement(achievementManager);
    }

    public void SetPlayerNumStarWithoutAchievement(int numStar)
    {
        playerData.numStar = numStar;
        popupManager.UpdateAchievement(achievementManager);
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }
    public void UpdateTileSpawnStartPosition()
    {
        tilesSpawn.UpdateStartPosition();
    }

    public ResourcesHolder GetResourcesHolder()
    {
        return resourcesHolder;
    }

    public MapManager GetMapManager()
    {
        return mapManager;
    }

    public TilesSpawn GetTilesSpawn()
    {
        return tilesSpawn;
    }

    public Vector3[] CheckValidTile(Tile[] tiles, DefineData.TilePattern tilePattern)
    {
        return mapManager.GetSnapItemsByTiles(tiles, tilePattern);
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }

    public int GetTotalScoreWithInfo(CustomShape customShape, int lineWin)
    {
        int totalScore = customShape.GetLength();
        totalScore += (int)((1 + lineWin) * (lineWin * 0.5f) * 10);
        return totalScore;
    }

    public void PlayAnimPlusScore(int totalLineWin, CustomShape customShape, Vector3 pos)
    {
        scoreBoard.PlayAnimPlusScore(totalLineWin, customShape, pos);
    }

    public void MoveTileToMapTile(CustomShape customShape)
    {
        mapManager.MoveTileToMapTile(customShape);
    }

    public void DescreaseAmountTilesSpawn(CustomShape customShape)
    {
        tilesSpawn.DescreaseAmount(customShape);
    }

    public bool IsHaveSpaceFor(Tile[] tiles)
    {
        return mapManager.IsHaveSpaceFor(tiles);
    }

    public void PlayAnimLose()
    {
        isLose = true;
        mapManager.PlayAnimLose();
        SoundManager.Instance.PlaySound(SoundManager.SoundName.Lose);
        DelayShowPopupEndGame();
    }

    public void DelayShowPopupEndGame()
    {
        CancelInvoke(nameof(ShowPopupEndGame));
        Invoke(nameof(ShowPopupEndGame), 1f);
    }

    public void ShowPopupEndGame()
    {
        popupManager.ShowPopupEndGame();
    }

    public void DelayCheckAllIsHaveSpaceFor(bool isWin)
    {
        if (isLose) return;
        float time = 1f;
        if (isWin)
        {
            time = 0.5f;
        }
        Debug.Log("time: " + time);
        CancelInvoke(nameof(CheckAllIsHaveSpaceFor));
        Invoke(nameof(CheckAllIsHaveSpaceFor), time);
    }

    public void EnableSkill(bool isEnable)
    {
        mapManager.EnableSkill(isEnable);
    }

    public void CheckBomSkillRange(Vector3 position)
    {
        mapManager.GetBomSkillRange(position);
    }

    public bool UseBomSkill(Vector3 position)
    {
        bool isCanUse = mapManager.UseBomSkill(position);
        if (isCanUse)
        {
            CancelInvoke(nameof(CheckAllIsHaveSpaceFor));
            CheckAllIsHaveSpaceFor();
            SetPlayerSkillBom(playerData.amountSkillBom - 1);
            scoreBoard.UpdateAmountSkillBom(playerData.amountSkillBom);
            IncreaseAchievementScore(DefineData.AchievementType.SKILL_USE);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Bomb);
        }
        return isCanUse;
    }

    private void CheckAllIsHaveSpaceFor()
    {
        tilesSpawn.CheckAllIsHaveSpaceFor();
    }

    public void TurnOnNotification()
    {
        notification.SetActive(true);
    }

    public void TurnOffNotification()
    {
        notification.SetActive(false);
    }
    
    public void ReloadGame()
    {
        SaveAndLoadFile.SaveData("achievement", achievementManager);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public PopupManager GetPopupManager()
    {
        return popupManager;
    }

    public void MoveStarToStarSection(Transform starTransform)
    {
        scoreBoard.MoveStarToStarSection(starTransform);
    }

    public void CheckLaserSkillRange(Vector3 position)
    {
        mapManager.GetLaserSkillRange(position);
    }

    public List<Tile> GetLaserSkillRange(Vector3 position)
    {
        List<Tile> laserSkillRange = mapManager.GetTileLaserSkillRange(position);
        if (laserSkillRange.Count > 0)
        {
            CancelInvoke(nameof(CheckAllIsHaveSpaceFor));
            CheckAllIsHaveSpaceFor();
            SetPlayerSkillLaser(playerData.amountSkillLaser - 1);
            scoreBoard.UpdateAmountSkillLaser(playerData.amountSkillLaser);
            IncreaseAchievementScore(DefineData.AchievementType.SKILL_USE);
            SoundManager.Instance.PlaySound(SoundManager.SoundName.Lighting);
        }
        return laserSkillRange;
    }

    public void CollectStar(Tile tile)
    {
        mapManager.CollectStar(tile);
    }

    public void SetPlayerSkillBom(int amount)
    {
        playerData.amountSkillBom = amount;
    }

    public void SetPlayerSkillLaser(int amount)
    {
        Debug.Log($"SetPlayerSkillLaser amount: {amount}");
        playerData.amountSkillLaser = amount;
    }

    public void OpenShopToBuySkill(DefineData.SkillName skillName, uint price)
    {
        popupManager.ShowPopupShop(skillName, price);
    }

    public void IncreaseAchievementScore(DefineData.AchievementType achievementType)
    {
        achievementManager.IncreaseScore(achievementType);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void IncreaseAchievementScore(DefineData.AchievementType achievementType, int value)
    {
        achievementManager.IncreaseScore(achievementType, value);
        popupManager.UpdateAchievement(achievementManager);
    }

    public void UpdateNumStarPlayerData(int numStar)
    {
        scoreBoard.UpdateNumStarPlayerData(numStar);
    }

    public void ConfirmBuySkill(DefineData.SkillName skillName, uint price)
    {
        playerData.numStar -= (int)price;
        scoreBoard.UpdateNumStarPlayerData();
        Debug.Log($"skillName: {skillName}");
        switch (skillName)
        {
            case DefineData.SkillName.Bom:
                SetPlayerSkillBom(playerData.amountSkillBom + 1);
                scoreBoard.UpdateAmountSkillBom(playerData.amountSkillBom);
                break;
            case DefineData.SkillName.Laser:
                // Debug.Log($"playerData.amountSkillLaser 11111111: {playerData.amountSkillLaser}");
                SetPlayerSkillLaser(playerData.amountSkillLaser + 1);
                // Debug.Log($"playerData.amountSkillLaser 22222222: {playerData.amountSkillLaser}");
                scoreBoard.UpdateAmountSkillLaser(playerData.amountSkillLaser);
                break;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log($"OnApplicationQuit {JsonConvert.SerializeObject(achievementManager.achievementInfos)}");
        SaveAndLoadFile.SaveData("achievement", achievementManager);
    }
}
