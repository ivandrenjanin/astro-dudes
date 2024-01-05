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
	public GameOverScreen gameOverScreen;

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

	public uint _highScore;
	public uint HighScore
	{
		get { return _highScore; }
		set
		{
			_highScore = value;
		}
	}

	public override void _Ready()
	{
		HighScore = LoadGame();

		var spawnMarker = GetNode<Marker2D>("PlayerSpawnPosition");
		enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");

		player = GetNode<Player>("Player");
		player.GlobalPosition = spawnMarker.GlobalPosition;

		laserContainer = GetNode<Node2D>("LaserContainer");
		enemyContainer = GetNode<Node2D>("EnemyContainer");
		hud = GetNode<Hud>("UILayer/HUD");

		gameOverScreen = GetNode<GameOverScreen>("UILayer/GameOverScreen");

		player.Connect("LaserShot", new Callable(this, nameof(OnPlayerLaserShot)));
		player.Connect("DestroyedPlayer", new Callable(this, nameof(OnDestroyedPlayer)));

		Score = 0;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("quit"))
			GetTree().Quit();
		else if (Input.IsActionJustPressed("reset"))
			GetTree().ReloadCurrentScene();
	}


	public void SaveGame()
	{
		using var saveFile = FileAccess.Open("user://save.data", FileAccess.ModeFlags.Write);
		saveFile.Store32(HighScore);
	}

	public static uint LoadGame()
	{
		using var saveFile = FileAccess.Open("user://save.data", FileAccess.ModeFlags.Read);

		if (saveFile != null)
		{
			return saveFile.Get32();
		}
		else
		{
			return 0;
		}

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
		if (!player.isDestroyed)
		{
			Score += scorePointValue;
		}

		if (Score > HighScore)
		{
			HighScore = Score;
		}

		GD.Print("Enemy Destroyed: ", Score);
	}

	public async void OnDestroyedPlayer()
	{
		gameOverScreen.SetScore(Score);
		gameOverScreen.SetHighScore(HighScore);
		SaveGame();
		await ToSignal(GetTree().CreateTimer(1.5), "timeout");
		gameOverScreen.Visible = true;
	}
}
