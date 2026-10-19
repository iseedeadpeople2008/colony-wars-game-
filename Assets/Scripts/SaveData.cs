using System;

[Serializable]
public class SaveData
{
    public int UpgradeStartUnits = 0;
    public int UpgradeProduction = 0;
    public int UpgradeCapacity = 0;

    public int StartUnitsPerLvl = 1;
    public float ProductionPerLvl = 0.1f;
    public int CapacityPerLvl = 1;
}