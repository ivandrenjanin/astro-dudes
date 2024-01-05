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
	[Signal]
	public delegate void HitEnemyEventHandler();

	public bool isDestroyed = false;
	public AnimatedSprite2D shipAnimated;
	public AnimatedSprite2D engineAnimated;
	public CollisionShape2D hitbox;

	public override void _Ready()
	{
		shipAnimated = GetNode<AnimatedSprite2D>("ShipAnimated");
		engineAnimated = GetNode<AnimatedSprite2D>("EngineAnimated");
		shipAnimated.Connect("animation_finished", new Callable(this, nameof(OnScoutScreenExited)));

		hitbox = GetNode<CollisionShape2D>("CollisionShape2D");
	}


	public override void _PhysicsProcess(double delta)
	{
		if (!isDestroyed)
		{
			Vector2 newPosition = GlobalPosition;
			newPosition.Y += Speed * (float)delta;
			GlobalPosition = newPosition;
		}
	}

	public void Destroy(bool shouldScore)
	{
		hitbox.SetDeferred("disabled", true);
		isDestroyed = true;

		if (shouldScore && isDestroyed)
		{
			EmitSignal(SignalName.DestroyedEnemy, ScorePointValue);
		}

		engineAnimated.Visible = false;
		shipAnimated.Animation = "destroy";
	}

	public void TakeDamage(byte amount)
	{
		HitPoints = (byte)Math.Max(0, HitPoints - amount);

		if (HitPoints == 0)
		{
			Destroy(true);
		}
		else
		{
			EmitSignal(SignalName.HitEnemy);
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
