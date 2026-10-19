using System;
using UnityEngine;

[Serializable]
public struct AIProfile
{
    [SerializeField] private float thinkInterval;
    [SerializeField] private int minUnitsToAttack;
    [SerializeField] private int attackMargin;
    [SerializeField] private float distanceWeight;
    [SerializeField] private float neutralBonus;
    [SerializeField] private int defenseMargin;
    [SerializeField] private float defenseWeight;
    [SerializeField] private float scoreNoise;
    [SerializeField] private float skipChance;

    public float ThinkInterval => thinkInterval;
    public float MinUnitsToAttack => minUnitsToAttack; 
    public int AttackMargin => attackMargin;
    public float DistanceWeight => distanceWeight;
    public float NeutralBonus => neutralBonus;
    public int DefenseMargin => defenseMargin;
    public float DefenseWeight => defenseWeight;
    public float ScoreNoise => scoreNoise;
    public float SkipChance => skipChance;
}