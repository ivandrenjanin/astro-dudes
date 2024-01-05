using Godot;
using System;

public partial class Enemy : Area2D
{
	[Export]
	public float Speed { get; set; } = 150.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 newPosition = GlobalPosition;
		newPosition.Y += Speed * (float)delta;
		GlobalPosition = newPosition;
	}

	public void Die()
	{
		QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body.GetType() == typeof(Player))
		{
			Player player = (Player)body;
			player.Die();
			Die();
		}
	}

	public void OnScoutScreenExited()
	{
		QueueFree();
	}
}
