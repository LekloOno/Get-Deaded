namespace Data.Entities;

public class Player
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public ICollection<Score> Scores { get; set; } = [];
}