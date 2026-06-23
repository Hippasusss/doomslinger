using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] private AnimationPlayer animationPlayer;
    [Export] private Button startButton;
    [Export] private Button savesButton;
    [Export] private Button exitButton;
    [Export] private PackedScene mainScene;

    public override void _Ready()
    {
        animationPlayer.AnimationFinished += _ => animationPlayer.Play("Throb");
        startButton.Pressed += StartGame;
        savesButton.Pressed += ViewSavedGames;
        exitButton.Pressed += ExitGame;
    }

    private void StartGame()
    {
        GetTree().ChangeSceneToPacked(mainScene);
    }

    private void ViewSavedGames()
    {

    }

    private void ExitGame()
    {

    }
}
