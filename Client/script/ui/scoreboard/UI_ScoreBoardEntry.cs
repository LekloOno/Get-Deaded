using System;
using Client.Api.Godot;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardEntry : Control
{
    [Export] private UI_ScoreBoardDetails _detailsTemplate;
    [Export] private Label _ranking;
    [Export] private Label _userName;
    [Export] private Label _time;
    [Export] private Label _kills;
    [Export] private Label _damage;
    [Export] private TextureRect _weapon;
    [Export] private Label _accuracy;

    private Guid _scoreId;
    private UI_ScoreBoardDetails _details;

    public override void _Ready()
    {
        _detailsTemplate.GetParent().RemoveChild(_detailsTemplate);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                GetDetails();
    }

    private async void GetDetails()
    {
        if (_details != null)
        {
            _details.Visible = !_details.Visible;
            return;
        }

        ScoreDto? scoreDetails = await ApiGodotGlue.Instance.ScoreApi.GetScoreDetailAsync(_scoreId);
        if (scoreDetails == null)
            return;

        _details = (UI_ScoreBoardDetails) _detailsTemplate.Duplicate();
        _details.Initialize(scoreDetails);
        

        int idx = GetIndex();
        Node parent = GetParent();

        parent.AddChild(_details);
        parent.MoveChild(_details, idx + 1);
    }

    public void Initialize(LeaderboardRowDto scoreRow)
    {
        _scoreId = scoreRow.ScoreId;

        _ranking.Text = scoreRow.Rank.ToString();
        _userName.Text = scoreRow.Player;
        _time.Text = Mathf.RoundToInt((float) scoreRow.TimeMs / 1000).ToString();
        _kills.Text = scoreRow.Kills.ToString();
        _damage.Text = scoreRow.Damage.ToString();

        //_weapon.Texture = //... need to implement a registry of weapons icons.

        _accuracy.Text = UI_ScoreBoardExtensions.DisplayAccuracy(scoreRow.BestWeaponAccuracy);
    }

    public void Clean()
    {
        _details?.QueueFree();
    }
}