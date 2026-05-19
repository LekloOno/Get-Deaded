using System;
using Client.Api;
using Client.Api.Godot;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardEntry : Control
{
    [Export] private UI_ScoreBoardDetails _details;
    [Export] private Control _container;
    [Export] private Label _ranking;
    [Export] private Label _userName;
    [Export] private Label _score;
    [Export] private Label _time;
    [Export] private Label _kills;
    [Export] private Label _damage;
    [Export] private TextureRect _weapon;
    [Export] private Label _accuracy;

    private Guid _scoreId;

    public override void _Ready()
    {
        _details.Visible = false;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
            if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                GetDetails();
    }

    private async void GetDetails()
    {
        if (_details.Initialized)
        {
            _details.Visible = !_details.Visible;
            return;
        }

        ApiResult<ScoreDto> result = await ApiGodotGlue.Instance.ScoreApi.GetScoreDetailAsync(_scoreId);

        if (result == null)
            return;

        if (!result.Success || result.Data == null)
        {
            GD.Print(result.ErrorMessage);
            return;
        }

        _details.Visible = true;
        _details.Initialize(result.Data);
    }

    public void Initialize(LeaderboardRowDto scoreRow)
    {
        _scoreId = scoreRow.ScoreId;

        _ranking.Text = scoreRow.Rank.ToString();
        _userName.Text = scoreRow.Player;
        _score.Text = scoreRow.Score.ToString();
        _time.Text = Mathf.RoundToInt((float) scoreRow.TimeMs / 1000).ToString();
        _kills.Text = scoreRow.Kills.ToString();
        _damage.Text = Mathf.RoundToInt(scoreRow.Damage).ToString();

        if(DATA_WeaponRegistry.Instance.Registry.TryGetValue(scoreRow.BestWeaponKey, out DATA_Weapon weapon))
        {
            _weapon.Texture = weapon.Icon;
            _weapon.Modulate = weapon.IconColor;
        }

        _accuracy.Text = UI_ScoreBoardExtensions.DisplayAccuracy(scoreRow.BestWeaponAccuracy);
    }

    public void Clean()
    {
        _details?.Clean();
    }
}