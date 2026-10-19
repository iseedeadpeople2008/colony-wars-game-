using System;
using UnityEngine;

public class Territory
{
    public event Action<Territory> CountChanged;
    public event Action<Territory> OwnerChanged;

    public int OwnerId { get; private set; }
    public int Count => (int)accumulator;
    private int capacity;
    public float FillRatio => Math.Min(1f, accumulator / capacity);

    private float productionPerSecond;
    private float accumulator;

    public int Id { get; }
    public Vector2 Position { get; }
    public Polygon Shape { get; }

    private float restoreDelay;
    private float timeSinceAction;

    public Territory(int id, TerritoryData data, int startCount)
    {
        Id = id;
        Position = data.Position;
        OwnerId = data.OwnerId;
        accumulator = startCount;
        Shape = new Polygon(data.Outline);
    }

    public void Tick(float dt)
    {
        if (Count >= capacity)
            return;

        timeSinceAction += dt;
        if (timeSinceAction < restoreDelay)
            return;

        accumulator = Math.Min(accumulator + productionPerSecond * dt, capacity);

        CountChanged?.Invoke(this);
    }

    public void TakeDamage(int amount)
    {
        accumulator = Math.Max(0, accumulator - amount);
        timeSinceAction = 0f;
        CountChanged?.Invoke(this);
    }

    public void Reinforce(int amount)
    {
        if (Count >= capacity)
            return;

        accumulator = Math.Min(capacity, accumulator + amount);
        CountChanged?.Invoke(this);
    }

    public void CaptureBy(int factionId)
    {
        OwnerId = factionId;
        accumulator = 1f;
        timeSinceAction = 0f;
        OwnerChanged?.Invoke(this);
        CountChanged?.Invoke(this);
    }

    public void SetRules(FactionRule rule)
    {
        productionPerSecond = rule.ProductionPerSecond;
        capacity = rule.Capacity;
        restoreDelay = rule.RestoreDelay;
    }

    public int Withdraw(int amount)
    {
        int taken = Math.Min(amount, Count);

        accumulator -= taken;
        timeSinceAction = 0f;
        CountChanged?.Invoke(this);
        return taken;
    }
}