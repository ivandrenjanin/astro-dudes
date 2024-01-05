using Godot;
using System;

public partial class Enemy : Area2D
{
	[Export]
	public float Speed { get; set; } = 150.0f;
	[Export]
	public byte HitPoints { get; set; } = 1;
	[Export]
	public uint ScorePointValue { get; set; } = 50;

	[Signal]
	public delegate void DestroyedEnemyEventHandler(uint scorePointValue);

	public override void _PhysicsProcess(double delta)
	{
		Vector2 newPosition = GlobalPosition;
		newPosition.Y += Speed * (float)delta;
		GlobalPosition = newPosition;
	}

	public void Destroy(bool shouldScore)
	{
		if (shouldScore)
		{
			EmitSignal(SignalName.DestroyedEnemy, ScorePointValue);
		}

		QueueFree();
	}

	public void TakeDamage(byte amount)
	{
		HitPoints = (byte)Math.Max(0, HitPoints - amount);

		if (HitPoints == 0)
		{
			Destroy(true);
		}
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body.GetType() == typeof(Player))
		{
			Player player = (Player)body;
			player.Destroy();
			Destroy(false);
		}
	}

	public void OnScoutScreenExited()
	{
		QueueFree();
	}
}
