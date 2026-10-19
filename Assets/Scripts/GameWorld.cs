using System;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld
{
    private readonly List<Territory> territories = new List<Territory>();
    private readonly BattleSettings settings;
    private readonly List<Soldier> soldiers = new List<Soldier>();
    private readonly List<Assault> assaults = new List<Assault>();

    public IReadOnlyList<Territory> Territories => territories;
    public IReadOnlyList<Soldier> Soldiers => soldiers;

    bool[] dead = new bool[1023];
    private SpatialGrid grid;

    public GameWorld(BattleSettings settings)
    {
        this.settings = settings;
    }

    public Territory AddTerritory(Territory territory)
    {
        territories.Add(territory);
        territory.OwnerChanged += ApplyFactionRules;
        ApplyFactionRules(territory);
        return territory;
    }

    public void Tick(float dt)
    {
        for (int i = 0; i < territories.Count; i++)
            territories[i].Tick(dt);

        TickAssaults(dt);

        MoveSoldiers(dt);
        ResolveMidAirCollisions();
        ResolveArrivals();
    }

    private void ResolveMidAirCollisions()
    {
        float collisionSqr = settings.SoldierCollisionRadius * settings.SoldierCollisionRadius;
        Array.Clear(dead, 0, 1023);

        EnsureGrid();
        grid.Rebuild(soldiers);

        for (int i = 0; i < soldiers.Count - 1; i++)
        {
            if (dead[i] == true)
                continue;

            int cx = grid.CellX(soldiers[i].Position);
            int cy = grid.CellY(soldiers[i].Position);

            bool stop = false;
            for (int nx = cx - 1; nx <= cx + 1 && stop == false; nx++)
            {
                for (int ny = cy - 1; ny <= cy + 1 && stop == false; ny++)
                {
                    int j = grid.First(nx, ny);
                    while (j != -1 && stop == false)
                    {
                        if (j != i &&
                            dead[j] == false &&
                            soldiers[i].FactionId != soldiers[j].FactionId &&
                            (soldiers[i].Position - soldiers[j].Position).sqrMagnitude <= collisionSqr)
                        {
                            dead[i] = true;
                            dead[j] = true;
                            stop = true;
                        }

                        j = grid.Next(j);
                    }
                }
            }
        }

        for (int i = soldiers.Count - 1; i >= 0; i--)
        {
            if (dead[i] == true)
                RemoveSoldierAt(i);
        }
    }

    private void ResolveArrivals()
    {
        float arrivalSqr = settings.ArrivalDistance * settings.ArrivalDistance;
        Territory target = null;

        for (int i = soldiers.Count - 1; i >= 0; i--)
        {
            target = GetTerritory(soldiers[i].TargetId);

            if ((soldiers[i].Position - target.Position).sqrMagnitude <= arrivalSqr)
            {
                ResolveArrival(soldiers[i], target);
                RemoveSoldierAt(i);
            }
        }
    }

    private void TickAssaults(float dt)
    {
        for (int i = assaults.Count - 1; i >= 0; i--)
        {
            Assault current = assaults[i];
            var source = GetTerritory(current.SourceId);

            if (source.OwnerId != current.FactionId ||
                current.Remaining <= 0 ||
                source.Count <= 0)
            {
                RemoveAssaultAt(i);
                continue;
            }

            current.Timer += dt;
            if (current.Timer < settings.LaunchInterval)
            {
                assaults[i] = current;
                continue;
            }

            current.Timer -= settings.LaunchInterval;
            int taken = source.Withdraw(Math.Min(settings.LaunchBatch, current.Remaining));
            current.Remaining -= taken;

            var target = GetTerritory(current.TargetId);
            Vector2 axis = (target.Position - source.Position).normalized;
            for (int j = 0; j < taken; j++)
            {
                Vector2 aim = FanFormation.GetAimPoint(target.Position, axis, j, taken, settings.LineSpacing);
                Vector2 direction = (aim - source.Position).normalized;
                soldiers.Add(new Soldier
                {
                    FactionId = current.FactionId,
                    TargetId = current.TargetId,
                    Position = source.Position,
                    Phase = SoldierPhase.Marching,
                    Direction = direction
                });
            }

            assaults[i] = current;
        }
    }

    private void MoveSoldiers(float dt)
    {
        float suctionSqr = settings.SuctionRadius * settings.SuctionRadius;

        for (int i = soldiers.Count - 1; i >= 0; i--)
        {
            Soldier current = soldiers[i];
            Territory target = GetTerritory(current.TargetId);

            if (current.Phase == SoldierPhase.Marching)
            {
                if ((target.Position - current.Position).sqrMagnitude <= suctionSqr)
                {
                    current.Phase = SoldierPhase.Sucking;
                }
                else
                {
                    current.Position += current.Direction * settings.SoldierSpeed * dt;
                    soldiers[i] = current;
                    continue;
                }
            }

            if (current.Phase == SoldierPhase.Sucking)
                current.Position = Vector2.MoveTowards(current.Position, target.Position, 
                    settings.SoldierSpeed * settings.SuctionSpeedMultiplier * dt);

            soldiers[i] = current;
        }
    }

    private void RemoveSoldierAt(int index)
    {
        int last = soldiers.Count - 1;
        soldiers[index] = soldiers[last];
        soldiers.RemoveAt(last);
    }

    private void RemoveAssaultAt(int index)
    {
        int last = assaults.Count - 1;
        assaults[index] = assaults[last];
        assaults.RemoveAt(last);
    }

    private void ResolveArrival(Soldier soldier, Territory target)
    {
        if (target.OwnerId == soldier.FactionId)
            target.Reinforce(1);
        else if (target.Count > 0)
            target.TakeDamage(1);
        else 
            target.CaptureBy(soldier.FactionId);
    }

    public Territory FindTerritoryAt(Vector2 point, float hitRadius)
    {
        var sqrRadius = hitRadius * hitRadius;

        for (int i = 0; i < territories.Count; i++)
        {
            var sqrDistance = (point - territories[i].Position).sqrMagnitude;
            if (sqrDistance <= sqrRadius)
                return territories[i];
        }

        return null;
    }

    public Territory GetTerritory(int id)
    {
        if (id >=0 && id < territories.Count)
            return territories[id];
        else
            return null;
    }

    public bool Execute(AttackCommand command)
    {
        var source = GetTerritory(command.SourceId);
        var target = GetTerritory(command.TargetId);
        if (source == null ||
            target == null ||
            source == target ||
            source.OwnerId != command.FactionId)
            return false;

        if (HasActiveAssault(command.SourceId) == true)
            return false;

        assaults.Add(new Assault
            (source.Id, target.Id, command.FactionId, source.Count, settings.LaunchInterval));

        return true;
    }
    private void ApplyFactionRules(Territory territory)
    {
        territory.SetRules(settings.GetRule(territory.OwnerId));
    }
    private void EnsureGrid()
    {
        if (grid != null)
            return;

        float minx = territories[0].Shape.Bounds.xMin;
        float maxx = territories[0].Shape.Bounds.xMax;
        float miny = territories[0].Shape.Bounds.yMin;
        float maxy = territories[0].Shape.Bounds.yMax;
        
        for (int i = 1; i < territories.Count; i++)
        {
            minx = Mathf.Min(minx, territories[i].Shape.Bounds.xMin);
            miny = Mathf.Min(miny, territories[i].Shape.Bounds.yMin);
            maxx = Mathf.Max(maxx, territories[i].Shape.Bounds.xMax);
            maxy = Mathf.Max(maxy, territories[i].Shape.Bounds.yMax);
        }

        float cellSize = Mathf.Max(settings.SoldierCollisionRadius * 2, 0.5f);
        grid = new SpatialGrid(Rect.MinMaxRect(minx - 1, miny - 1, maxx + 1, maxy + 1), cellSize);
    }

    public bool HasActiveAssault(int sourceId)
    {
        for (int i = 0; i < assaults.Count; i++)
        {
            if (assaults[i].SourceId == sourceId)
                return true;
        }

        return false;
    }

    public int CountIncoming(int targetId, int factionId)
    {
        // свои войска в пути к цели
        return CountIncomingInternal(targetId, factionId, true);
    }

    public int CountIncomingHostile(int targetId, int factionId)
    {
        // войска всех остальных фракций в пути к цели
        return CountIncomingInternal(targetId, factionId, false);
    }

    private int CountIncomingInternal(int targetId, int factionId, bool sameFaction)
    {
        int total = 0;

        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i].TargetId == targetId && (soldiers[i].FactionId == factionId) == sameFaction)
                total++;
        }

        for (int i = 0; i < assaults.Count; i++)
        {
            if (assaults[i].TargetId == targetId && (assaults[i].FactionId == factionId) == sameFaction)
                total += assaults[i].Remaining;
        }

        return total;
    }
}