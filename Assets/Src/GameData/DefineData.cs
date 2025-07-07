
public class DefineData
{
    public static UnityEngine.Vector2 BaseGameSize = new UnityEngine.Vector2(744, 992);
    public enum TileType
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z,
        Floor
    }

    public enum ColorType
    {
        Red,
        Green,
        Blue,
        Yellow,
    }

    public enum TilePattern
    {
        Fire,
        Leaf,
        Circle,
        Wave,
        Sun,
        Frost,
        LightBolt,
    }

    public class WinDataInfo
    {
        public bool isRow {get; set;}
        public int index {get; set;}
        public int crossIndex {get; set;}
        public int startIndex {get; set;}
    }

    public enum SkillName
    {
        Bom,
        Laser
    }

    public enum AchievementType
    {
        SCORE,
        STAR_COLLECT,
        MULTIPLY,
        GREAT_COMBO,
        SKILL_USE,
        CROSS_LINE,
        PLACE_TILE,
        WIN_WITH_ONE_TILE,
        BEST_SCORE
        
    }
}
