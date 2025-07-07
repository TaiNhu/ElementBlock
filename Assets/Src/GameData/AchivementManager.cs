using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AchievementManager
{
    [System.Serializable]
    public class AchievementInfo
    {
        public int currentScore;
        public int amountClaim;
    }
    public Dictionary<DefineData.AchievementType, AchievementInfo> achievementInfos;

    public AchievementManager()
    {
        achievementInfos = new ()
        {
            {DefineData.AchievementType.SCORE, new AchievementInfo()},
            {DefineData.AchievementType.STAR_COLLECT, new AchievementInfo()},
            {DefineData.AchievementType.MULTIPLY, new AchievementInfo()},
            {DefineData.AchievementType.GREAT_COMBO, new AchievementInfo()},
            {DefineData.AchievementType.SKILL_USE, new AchievementInfo()},
            {DefineData.AchievementType.CROSS_LINE, new AchievementInfo()},
            {DefineData.AchievementType.PLACE_TILE, new AchievementInfo()},
            {DefineData.AchievementType.WIN_WITH_ONE_TILE, new AchievementInfo()},
            {DefineData.AchievementType.BEST_SCORE, new AchievementInfo()},
        };
    }

    public void InitPreloadData(Dictionary<DefineData.AchievementType, AchievementInfo> as1, Action<string> log)
    {
        foreach (KeyValuePair<DefineData.AchievementType, AchievementInfo> a in as1)
        {
            if (achievementInfos.ContainsKey(a.Key))
            {
                achievementInfos[a.Key].currentScore = a.Value.currentScore;
                achievementInfos[a.Key].amountClaim = a.Value.amountClaim;
            }
        }
    }

    public void AddScore(DefineData.AchievementType achievementType, int score)
    {
        achievementInfos[achievementType].currentScore = Mathf.Max(achievementInfos[achievementType].currentScore, score);
    }

    public void AddAmountClaim(DefineData.AchievementType achievementType)
    {
        achievementInfos[achievementType].amountClaim++;
    }

    public void AddAmountClaim(DefineData.AchievementType achievementType, int value)
    {
        achievementInfos[achievementType].amountClaim += value;
    }

    public void IncreaseScore(DefineData.AchievementType achievementType)
    {
        achievementInfos[achievementType].currentScore++;
    }

    public void IncreaseScore(DefineData.AchievementType achievementType, int value)
    {
        achievementInfos[achievementType].currentScore += value;
    }
    
}