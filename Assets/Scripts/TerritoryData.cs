using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class TerritoryData
{

    [SerializeField] private Vector2 position;
    [SerializeField] private int ownerId = Faction.Neutral;
    [SerializeField] private int startCount = 10;
    [SerializeField] private int capacity = 50;
    [SerializeField] private float productionPerSecond = 1f;
    [SerializeField] private List<Vector2> outline = new List<Vector2>();

    public IReadOnlyList<Vector2> Outline => outline;
    public Vector2 Position => position;
    public int OwnerId => ownerId;
    public int StartCount => startCount;
    public int Capacity => capacity;
    public float ProductionPerSecond => productionPerSecond;
}