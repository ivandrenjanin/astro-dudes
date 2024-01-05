using Godot;
using System;

public partial class Laser : Area2D
{
	[Export]
	public float Speed { get; set; } = 600.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 newPosition = GlobalPosition;
		newPosition.Y += -Speed * (float)delta;
		GlobalPosition = newPosition;
	}

	public void OnLaserScreenExited()
	{
		QueueFree();
	}

	public void OnAreaEntered(Area2D area)
	{
		if (area.GetType() == typeof(Enemy))
		{
			Enemy scout = (Enemy)area;
			scout.Die();
			QueueFree();
		}
	}

}
