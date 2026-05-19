using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardEntry : Control
{
    [Export] private Label _ranking;
    [Export] private Label _userName;
    [Export] private Label _time;
    [Export] private Label _kills;
    [Export] private Label _damage;
    [Export] private TextureRect _weapon;
    [Export] private Label _accuracy;

    public void Initialize(LeaderboardRowDto scoreRow)
    {
        _ranking.Text = scoreRow.Rank.ToString();
        _userName.Text = scoreRow.Player;
        _time.Text = Mathf.RoundToInt((float) scoreRow.TimeMs / 1000).ToString();
        _kills.Text = scoreRow.Kills.ToString();
        _damage.Text = scoreRow.Damage.ToString();

        //_weapon.Texture = //... need to implement a registry of weapons icons.

        _accuracy.Text = UI_ScoreBoardExtensions.DisplayAccuracy(scoreRow.BestWeaponAccuracy);
    }
}