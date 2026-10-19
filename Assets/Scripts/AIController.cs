using System;
using UnityEngine;
using Random = System.Random;

public class AIController
{
    private readonly GameWorld world;
    private readonly int factionId;
    private readonly AIProfile profile;
    private readonly Random random;
    private float timer;
    private int[] incoming = new int[32];
    private int[] need = new int[32];
    private int[] threat = new int[32];
    private int[] sources = new int[32];
    private float[] sourceDist = new float[32];

    public AIController(GameWorld world, int factionId, AIProfile profile, int seed)
    {
        this.world = world;
        this.factionId = factionId;
        this.profile = profile;
        random = new Random(seed);
    }

    public void Tick(float dt)
    {
        timer += dt;

        if (timer < profile.ThinkInterval)
            return;

        timer -= profile.ThinkInterval;
        Think();
    }

    private void Think()
    {
        RefreshIncoming();

        Territory source;
        Territory target;
        Territory bestTarget;
        float bestScore;

        for (int i  = 0; i < world.Territories.Count; i++)
        {
            source = world.Territories[i];
            if (source.OwnerId != factionId ||
                source.Count < profile.MinUnitsToAttack ||
                need[source.Id] > 0 ||
                threat[source.Id] > 0 ||
                world.HasActiveAssault(source.Id))
                continue;

            if (random.NextDouble() < profile.SkipChance)
                continue;

            bestTarget = null;
            bestScore = 0;
            for (int j = 0; j < world.Territories.Count; j++)
            {
                target = world.Territories[j];
                float score = 0;

                if (target == source)
                    continue;

                if (target.OwnerId == factionId)
                {
                    int n = need[target.Id];

                    if (n <= 0)
                        continue;

                    var benefit = Math.Min(n, source.Count);
                    var distance = (target.Position - source.Position).magnitude;
                    score = benefit * profile.DefenseWeight - distance * profile.DistanceWeight;
                }
                else
                {
                    int covered = incoming[target.Id];

                    if (covered >= target.Count + profile.AttackMargin)
                        continue;

                    int defenders = Math.Max(0, target.Count - covered);
                    var advantage = source.Count - defenders - profile.AttackMargin;

                    if (advantage <= 0)
                        continue;

                    var distance = (target.Position - source.Position).magnitude;
                    score = advantage - distance * profile.DistanceWeight + (target.OwnerId == Faction.Neutral ? profile.NeutralBonus : 0);
                }

                score += (float)(random.NextDouble() * 2.0 - 1.0) * profile.ScoreNoise;
                if (score > bestScore)
                {
                    bestScore = score; 
                    bestTarget = target;
                }
            }

            if (bestTarget != null && world.Execute(new AttackCommand(factionId, source.Id, bestTarget.Id)))
            {
                if (bestTarget.OwnerId == factionId)
                    need[bestTarget.Id] -= source.Count;
                else
                    incoming[bestTarget.Id] += source.Count;
            }
        }

        TryCoalitionAttack();
    }

    private void RefreshIncoming()
    {
        Territory current = null;
        for (int i = 0; i < world.Territories.Count; i++)
        {
            current = world.Territories[i];
            incoming[current.Id] = world.CountIncoming(current.Id, factionId);
            if (current.OwnerId == factionId)
            {
                int hostile = world.CountIncomingHostile(current.Id, factionId);
                threat[current.Id] = hostile;
                need[current.Id] = hostile + profile.DefenseMargin - current.Count - incoming[current.Id];
            }
            else
            {
                need[current.Id] = 0;
                threat[current.Id] = 0;
            }
        }
    }

    private int BuildCoalition(Territory target, int required, out int total)
    {
        int n = 0;
        for (int i = 0; i < world.Territories.Count; i++)
        {
            Territory s = world.Territories[i];
            if (s.OwnerId != factionId ||
                s.Count < profile.MinUnitsToAttack ||
                need[s.Id] > 0 ||
                threat[s.Id] > 0 ||
                world.HasActiveAssault(s.Id))
                continue;

            float dist = (target.Position - s.Position).magnitude;
            int k = n;
            while (k > 0 && sourceDist[k - 1] > dist)
            {
                sources[k] = sources[k - 1];
                sourceDist[k] = sourceDist[k - 1];
                k--;
            }

            sources[k] = s.Id;
            sourceDist[k] = dist;
            n++;
        }

        int sum = 0;
        for (int i = 0; i < n; i++)
        {
            sum += world.GetTerritory(sources[i]).Count;
            if (sum > required)
            {
                total = sum;
                return i + 1;
            }
        }

        total = 0;
        return 0;
    }

    private void TryCoalitionAttack()
    {
        Territory bestTarget = null;
        float bestScore = 0;

        for (int j = 0; j < world.Territories.Count; j++)
        {
            Territory target = world.Territories[j];
            if (target.OwnerId == factionId)
                continue;

            int covered = incoming[target.Id];
            if (covered >= target.Count + profile.AttackMargin)
                continue;

            int required = Math.Max(0, target.Count - covered) + profile.AttackMargin;

            int donors = BuildCoalition(target, required, out int total);
            if (donors == 0)
                continue;

            float avgDist = 0;
            for (int k = 0; k < donors; k++)
                avgDist += sourceDist[k];
            avgDist /= donors;

            float score = (total - required) - avgDist * profile.DistanceWeight
                          + (target.OwnerId == Faction.Neutral ? profile.NeutralBonus : 0)
                          + (float)(random.NextDouble() * 2.0 - 1.0) * profile.ScoreNoise;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = target;
            }
        }

        if (bestTarget == null)
            return;

        int bestRequired = Math.Max(0, bestTarget.Count - incoming[bestTarget.Id]) + profile.AttackMargin;
        int count = BuildCoalition(bestTarget, bestRequired, out _);

        for (int k = 0; k < count; k++)
        {
            Territory source = world.GetTerritory(sources[k]);
            int units = source.Count;
            if (world.Execute(new AttackCommand(factionId, source.Id, bestTarget.Id)))
                incoming[bestTarget.Id] += units;
        }
    }
}