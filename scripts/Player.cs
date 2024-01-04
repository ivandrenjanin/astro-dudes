using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 300.0f;

	[Signal]
	public delegate void LaserShotEventHandler(PackedScene laserScene, Vector2 location);

	public PackedScene laser = GD.Load<PackedScene>("res://scenes/laser.tscn");
	public Marker2D muzzle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		muzzle = GetNode<Marker2D>("Muzzle");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			Shoot();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");


		var engineAnimated = GetNode<AnimatedSprite2D>("EngineAnimated");

		if (direction != Vector2.Zero)
		{
			engineAnimated.Animation = "movement_up";
		}
		else
		{
			engineAnimated.Animation = "idle";
		}

		Velocity = direction * Speed;
		MoveAndSlide();
	}

	public void Shoot()
	{
		EmitSignal(SignalName.LaserShot, laser, muzzle.GlobalPosition);
	}
}
