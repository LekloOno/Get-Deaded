namespace Data.Entities;

public class ClientVersion
{
    public string   VersionKey              { get; set; } = null!;   // "0.2.3"
    public bool     AcceptingSubmissions    { get; set; } = true;

    public Guid     CompatibilityGroupId    { get; set; }
    public ScoreCompatibilityGroup CompatibilityGroup { get; set; } = null!;
}