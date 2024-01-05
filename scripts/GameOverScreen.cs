using Godot;
using System;

public partial class GameOverScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetScore(uint value)
	{
		var score = GetNode<Label>("Panel/Score");
		score.Text = "Score: " + Convert.ToString(value);
	}

	public void SetHighScore(uint value)
	{
		var score = GetNode<Label>("Panel/HighScore");
		score.Text = "High Score: " + Convert.ToString(value);
	}

	public void OnRetryPressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
