using System;
using UnityEngine;

[Serializable]
public class FactionRule
{
    [SerializeField] private int factionId;
    [SerializeField] private float productionPerSecond;
    [SerializeField] private int capacity;
    [SerializeField] private float restoreDelay;

    public int FactionId => factionId;
    public float ProductionPerSecond => productionPerSecond;
    public int Capacity => capacity;
    public float RestoreDelay => restoreDelay;
}