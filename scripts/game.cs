using Godot;
using System;

public partial class Game : Node2D
{
	public Player player = null;
	public Node2D laserContainer;

	public override void _Ready()
	{
		var spawnMarker = GetNode<Marker2D>("PlayerSpawnPosition");
		player = GetNode<Player>("Player");
		player.GlobalPosition = spawnMarker.GlobalPosition;
		laserContainer = GetNode<Node2D>("LaserContainer");
		player.Connect("LaserShot", new Callable(this, nameof(OnPlayerLaserShot)));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("quit"))
		{
			GetTree().Quit();
		}
		else if (Input.IsActionJustPressed("reset"))
		{
			GetTree().ReloadCurrentScene();
		}
	}

	public void OnPlayerLaserShot(PackedScene laserScene, Vector2 location)
	{
		var laser = laserScene.Instantiate<Laser>();
		laser.GlobalPosition = location;
		laserContainer.AddChild(laser);
	}
}
