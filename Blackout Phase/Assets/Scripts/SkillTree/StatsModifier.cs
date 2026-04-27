//
// Use to modify the stats of the unit
// Weijun

[System.Serializable]
public class StatsModifier
{
    public StatsType statsType; // access the type of stats

    public int value; // the amount of changes

    public bool isPercent; // percent of the change amount
}