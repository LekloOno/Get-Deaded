namespace Data.Entities;

public class ScoreCompatibilityGroup
{
    public Guid     Id      { get; set; }
    public string   Label   { get; set; } = null!; // Some label to mention the reason of the compatibility separation
    public ICollection<ClientVersion> Versions { get; set; } = [];
}