public readonly struct AttackCommand
{
    public int FactionId { get; }
    public int SourceId { get; }
    public int TargetId { get; }

    public AttackCommand(int factionId, int sourceId, int targetId)
    {
        FactionId = factionId;
        SourceId = sourceId;
        TargetId = targetId;
    }
}