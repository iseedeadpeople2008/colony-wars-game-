using System;
using UnityEngine;

[Serializable]
public struct BattleSettings
{
    [SerializeField] private FactionRule[] factionRules;
    [SerializeField] private float soldierSpeed;
    [SerializeField] private float arrivalDistance;
    [SerializeField] private int launchBatch;      
    [SerializeField] private float launchInterval;
    [SerializeField] private float lineSpacing;             
    [SerializeField] private float suctionRadius;           
    [SerializeField] private float suctionSpeedMultiplier;
    [SerializeField] private float soldierCollisionRadius;

    public float SoldierSpeed => soldierSpeed;
    public float ArrivalDistance => arrivalDistance;
    public int LaunchBatch => launchBatch;
    public float LaunchInterval => launchInterval;
    public float LineSpacing => lineSpacing;
    public float SuctionRadius => suctionRadius;
    public float SuctionSpeedMultiplier => suctionSpeedMultiplier;
    public float SoldierCollisionRadius => soldierCollisionRadius;

    public FactionRule GetRule(int factionId)
    {
        for (int i = 0; i < factionRules.Length; i++)
        {
            if (factionRules[i].FactionId == factionId)
                return factionRules[i];
        }

        return new FactionRule();
    }

    public BattleSettings WithRules(FactionRule[] rules)
    {
        factionRules = rules;
        return this;
    }
}