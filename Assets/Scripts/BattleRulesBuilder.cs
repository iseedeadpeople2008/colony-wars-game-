using System.Collections.Generic;

public static class BattleRulesBuilder
{
    public static BattleSettings BuildSettings(
        BattleSettings baseSettings,
        LevelConfig level,
        SaveData save)
    {
        FactionRule playerRule = baseSettings.GetRule(Faction.Player);

        int startBonus = save.UpgradeStartUnits * save.StartUnitsPerLvl;
        float prodBonus = save.UpgradeProduction * save.ProductionPerLvl;
        int capBonus = save.UpgradeCapacity * save.CapacityPerLvl;

        FactionRule upgradedPlayerRule = new FactionRule(
            Faction.Player,
            playerRule.StartCount + startBonus,
            playerRule.ProductionPerSecond + prodBonus,
            playerRule.Capacity + capBonus,
            playerRule.RestoreDelay);

        FactionRule neutralRule = baseSettings.GetRule(Faction.Neutral);
        List<FactionRule> finalRules = new List<FactionRule> { neutralRule, upgradedPlayerRule };
        for (int i = 0; i < level.Bots.Count; i++)
        {
            finalRules.Add(level.Bots[i].CustomRule);
        }

        return baseSettings.WithRules(finalRules.ToArray());
    }
}