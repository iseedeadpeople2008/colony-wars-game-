using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ColonyWars/Faction Color")]
public class FactionColor : ScriptableObject
{
    [SerializeField] private int factionId;
    [SerializeField] private Color dotColor = Color.white;
    [SerializeField] private Color areaColor = Color.white;

    public int FactionId => factionId;
    public Color DotColor => dotColor;
    public Color AreaColor => areaColor;
}