using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColonyWars/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<TerritoryData> territories = new List<TerritoryData>();

    public IReadOnlyList<TerritoryData> Territories => territories;
}