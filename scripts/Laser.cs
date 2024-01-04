using Godot;
using System;

public partial class Laser : Area2D
{
	[Export]
	public float Speed { get; set; } = 600.0f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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


}
