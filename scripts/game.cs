using Godot;
using System;

public partial class Game : Node2D
{
	[Export]
	public PackedScene[] EnemyScenes;
	public Player player;
	public Node2D laserContainer;
	public Node2D enemyContainer;
	public Timer enemySpawnTimer;
	public Hud hud;

	private uint _score = 0;
	public uint Score
	{
		get { return _score; }
		set
		{
			_score = value;
			hud.Score = _score;
		}
	}

	public override void _Ready()
	{
		var spawnMarker = GetNode<Marker2D>("PlayerSpawnPosition");
		enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");

		player = GetNode<Player>("Player");
		player.GlobalPosition = spawnMarker.GlobalPosition;

		laserContainer = GetNode<Node2D>("LaserContainer");
		enemyContainer = GetNode<Node2D>("EnemyContainer");
		hud = GetNode<Hud>("UILayer/HUD");

		player.Connect("LaserShot", new Callable(this, nameof(OnPlayerLaserShot)));
		Score = 0;
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
		enemy.Connect("DestroyedEnemy", new Callable(this, nameof(OnDestroyedEnemy)));
		enemyContainer.AddChild(enemy);
	}

	public void OnDestroyedEnemy(uint scorePointValue)
	{
		Score += scorePointValue;
		GD.Print("Enemy Destroyed: ", Score);
	}
}
