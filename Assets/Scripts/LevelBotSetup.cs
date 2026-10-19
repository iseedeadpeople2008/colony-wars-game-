using System;
using UnityEngine;

[Serializable]
public struct LevelBotSetup
{
    [SerializeField] private int factionId;
    [SerializeField] private AIProfile profile;       
    [SerializeField] private FactionRule customRule; 
    public int FactionId => factionId;
    public AIProfile Profile => profile;
    public FactionRule CustomRule => customRule;
}