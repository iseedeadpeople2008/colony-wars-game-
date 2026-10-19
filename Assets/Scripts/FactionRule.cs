using System;
using UnityEngine;

[Serializable]
public struct FactionRule
{
    [SerializeField] private int factionId;
    [SerializeField] private int startCount;
    [SerializeField] private float productionPerSecond;
    [SerializeField] private int capacity;
    [SerializeField] private float restoreDelay;

    public FactionRule(int factionId, int startCount, float productionPerSecond, int capacity, float restoreDelay)
    {
        this.factionId = factionId;
        this.startCount = startCount;
        this.productionPerSecond = productionPerSecond;
        this.capacity = capacity;
        this.restoreDelay = restoreDelay;
    }

    public int FactionId => factionId;
    public int StartCount => startCount;
    public float ProductionPerSecond => productionPerSecond;
    public int Capacity => capacity;
    public float RestoreDelay => restoreDelay;
}