using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColonyWars/Faction Palette")]
public class FactionPalette : ScriptableObject
{
    [SerializeField] private FactionColor neutral;
    [SerializeField] private List<FactionColor> factions = new List<FactionColor>();

    public Color GetDotColor(int factionId)
    {
        return FindFaction(factionId).DotColor;
    }

    public Color GetAreaColor(int factionId)
    {
        return FindFaction(factionId).AreaColor;
    }

    private FactionColor FindFaction(int factionId)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i].FactionId == factionId)
                return factions[i];
        }

        return neutral;
    }
}