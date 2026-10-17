using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ColonyWars/Faction Palette")]
public class FactionPalette : ScriptableObject
{
    [SerializeField] private FactionColor neutral;
    [SerializeField] private List<FactionColor> factions = new List<FactionColor>();
    private Color[] dotColors;
    private Color[] areaColors;

    public void Build()
    {
        int maxId = neutral != null ? neutral.FactionId : 0;
        for (int i = 0; i < factions.Count; i++)
            if (factions[i].FactionId > maxId)
                maxId = factions[i].FactionId;

        dotColors = new Color[maxId + 1];
        areaColors = new Color[maxId + 1];
        if (neutral != null)
        {
            dotColors[neutral.FactionId] = neutral.DotColor;
            areaColors[neutral.FactionId] = neutral.AreaColor;
        }
        for (int i = 0; i < factions.Count; i++)
        {
            int id = factions[i].FactionId; 
            dotColors[id] = factions[i].DotColor;
            areaColors[id] = factions[i].AreaColor;
        }
    }

    public Color GetDotColor(int factionId)
    {
        return dotColors[factionId];
    }

    public Color GetAreaColor(int factionId)
    {
        return areaColors[factionId];
    }
}