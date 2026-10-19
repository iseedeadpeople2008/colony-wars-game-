using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColonyWars/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<TerritoryData> territories = new List<TerritoryData>();
    [SerializeField] private List<LevelBotSetup> bots = new List<LevelBotSetup>();

    public IReadOnlyList<TerritoryData> Territories => territories;
    public IReadOnlyList<LevelBotSetup> Bots => bots;
}