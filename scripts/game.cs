using Godot;
using System;

public partial class Game : Node2D
{
	[Export]
	public PackedScene[] EnemyScenes;
	public Player player = null;
	public Node2D laserContainer;
	public Node2D enemyContainer;
	public Timer enemySpawnTimer;
	public override void _Ready()
	{
		var spawnMarker = GetNode<Marker2D>("PlayerSpawnPosition");
		enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");

		player = GetNode<Player>("Player");
		player.GlobalPosition = spawnMarker.GlobalPosition;

		laserContainer = GetNode<Node2D>("LaserContainer");
		enemyContainer = GetNode<Node2D>("EnemyContainer");

		player.Connect("LaserShot", new Callable(this, nameof(OnPlayerLaserShot)));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("quit"))
			GetTree().Quit();
		else if (Input.IsActionJustPressed("reset"))
			GetTree().ReloadCurrentScene();
	}

	public void OnPlayerLaserShot(PackedScene laserScene, Vector2 location)
	{
		var laser = laserScene.Instantiate<Laser>();
		laser.GlobalPosition = location;
		laserContainer.AddChild(laser);
	}

	public void OnEnemySpawnTimerTimeout()
	{
		var scene = EnemyScenes[GD.RandRange(0, 1)];
		var enemy = scene.Instantiate<Enemy>();
		enemy.GlobalPosition = new Vector2(
			x: GD.RandRange(50, 500),
			y: -50
		);
		enemyContainer.AddChild(enemy);
	}
}
