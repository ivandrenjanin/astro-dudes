using Godot;
using System;

public partial class GameOverScreen : Control
{
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
