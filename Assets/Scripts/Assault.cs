public class Assault
{
    public int SourceId { get; }
    public int TargetId { get; }
    public int FactionId { get; }
    public int Remaining { get; set; }
    public float Timer { get; set; }

    public Assault(int sourceId, int targetId, int factionId, int remaining, float timer)
    {
        SourceId = sourceId;
        TargetId = targetId;
        FactionId = factionId;
        Remaining = remaining;
        Timer = timer;
    }
}