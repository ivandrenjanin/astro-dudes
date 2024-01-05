using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 300.0f;
	[Export]
	public double RateOfFire { get; set; } = 0.25;
	public bool shootCd = false;
	public Marker2D muzzle;
	public PackedScene laser = GD.Load<PackedScene>("res://scenes/laser.tscn");

	[Signal]
	public delegate void LaserShotEventHandler(PackedScene laserScene, Vector2 location);

	public override void _Ready()
	{
		muzzle = GetNode<Marker2D>("Muzzle");
	}

	public override async void _Process(double delta)
	{
		if (Input.IsActionPressed("shoot") && !shootCd)
		{
			shootCd = true;
			Shoot();
			await ToSignal(GetTree().CreateTimer(RateOfFire), "timeout");
			shootCd = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		var engineAnimated = GetNode<AnimatedSprite2D>("EngineAnimated");

		if (direction != Vector2.Zero)
			engineAnimated.Animation = "movement_up";
		else
			engineAnimated.Animation = "idle";

		Velocity = direction * Speed;
		MoveAndSlide();

		var screenSize = GetViewportRect().Size;

		// Clamp Example
		// GlobalPosition = GlobalPosition.Clamp(Vector2.Zero, screenSize);

		// Screen Wrap Example
		GlobalPosition = new Vector2(
			x: Mathf.PosMod(GlobalPosition.X, screenSize.X),
			y: GlobalPosition.Y
		);
	}

	public void Shoot()
	{
		EmitSignal(SignalName.LaserShot, laser, muzzle.GlobalPosition);
	}

	public void Destroy()
	{
		QueueFree();
	}
}
