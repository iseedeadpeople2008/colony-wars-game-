using UnityEngine;

public enum SoldierPhase { Marching, Sucking }

public struct Soldier
{
    public Vector2 Position;
    public int FactionId;
    public int TargetId;
    public SoldierPhase Phase;
    public Vector2 Direction;   
}