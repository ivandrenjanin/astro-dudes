using Godot;
using System;

public partial class Hud : Control
{
	public Label ScoreLabel;

	private uint _score;
	public uint Score
	{
		get { return _score; }
		set
		{
			_score = value;
			ScoreLabel.Text = "SCORE: " + Convert.ToString(value);
		}
	}



	public override void _Ready()
	{
		ScoreLabel = GetNode<Label>("Score");
	}

	public override void _Process(double delta)
	{
	}
}
