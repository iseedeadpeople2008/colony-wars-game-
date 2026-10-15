using System;
using UnityEngine;

public class Territory
{
    public event Action<Territory> CountChanged;

    public int OwnerId { get; }
    public int Count => (int)accumulator;
    public int Capacity { get; }
    public float FillRatio => Math.Min(1f, accumulator / Capacity);

    private readonly float productionPerSecond;
    private float accumulator;

    public Vector2 Position { get; }
    public Polygon Shape { get; }

    private const float RestoreDelay = 2f;
    private float timeSinceDamage;

    public Territory(TerritoryData data)
    {
        Position = data.Position;
        OwnerId = data.OwnerId;
        accumulator = data.StartCount;
        Capacity = data.Capacity;
        productionPerSecond = data.ProductionPerSecond;
        Shape = new Polygon(data.Outline);
    }

    public void Tick(float dt)
    {
        if (Count >= Capacity)
            return;

        timeSinceDamage += dt;
        if (timeSinceDamage < RestoreDelay && OwnerId == Faction.Neutral)
            return;

        accumulator = Mathf.Min(accumulator + productionPerSecond * dt, Capacity);

        CountChanged?.Invoke(this);
    }

    public void TakeDamage(int amount)
    {
        accumulator = Math.Max(0, accumulator - amount);
        timeSinceDamage = 0f;
        CountChanged?.Invoke(this);
    }
}