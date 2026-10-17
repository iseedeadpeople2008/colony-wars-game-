using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class GameWorld
{
    private readonly List<Territory> territories = new List<Territory>();
    private readonly BattleSettings settings;
    private readonly List<Soldier> soldiers = new List<Soldier>();
    private readonly List<Assault> assaults = new List<Assault>();

    public IReadOnlyList<Territory> Territories => territories;
    public IReadOnlyList<Soldier> Soldiers => soldiers;

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
        float arrivalSqr = settings.ArrivalDistance * settings.ArrivalDistance;
        float suctionSqr = settings.SuctionRadius * settings.SuctionRadius;

        for (int i = soldiers.Count - 1; i >= 0; i--)
        {
            Soldier current = soldiers[i];
            Territory target = GetTerritory(current.TargetId);

            if (target == null)
            {
                RemoveSoldierAt(i);
                continue;
            }

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

            if ((target.Position - current.Position).sqrMagnitude <= arrivalSqr)
            {
                ResolveArrival(current, target);
                RemoveSoldierAt(i);
            }
            else
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

        for (int i = 0; i < assaults.Count; i++)
        {
            if (assaults[i].SourceId == command.SourceId)
                return false;
        }

        assaults.Add(new Assault
            (source.Id, target.Id, command.FactionId, source.Count, settings.LaunchInterval));

        return true;
    }
    private void ApplyFactionRules(Territory territory)
    {
        territory.SetRules(settings.GetRule(territory.OwnerId));
    }
}