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
	public ParallaxBackground parallaxBackground;
	public AudioStreamPlayer laserSound;
	public AudioStreamPlayer hitSound;
	public AudioStreamPlayer explosionEnemySound;
	public AudioStreamPlayer explosionPlayerSound;


	private uint _score = 0;
	public float scrollSpeed = 100;
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

		laserSound = GetNode<AudioStreamPlayer>("SFX/LaserSound");
		hitSound = GetNode<AudioStreamPlayer>("SFX/HitSound");
		explosionEnemySound = GetNode<AudioStreamPlayer>("SFX/ExplosionEnemySound");
		explosionPlayerSound = GetNode<AudioStreamPlayer>("SFX/ExplosionPlayerSound");

		gameOverScreen = GetNode<GameOverScreen>("UILayer/GameOverScreen");

		player.Connect("LaserShot", new Callable(this, nameof(OnPlayerLaserShot)));
		player.Connect("DestroyedPlayer", new Callable(this, nameof(OnDestroyedPlayer)));

		parallaxBackground = GetNode<ParallaxBackground>("ParallaxBackground");
		Score = 0;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("quit"))
			GetTree().Quit();
		else if (Input.IsActionJustPressed("reset"))
			GetTree().ReloadCurrentScene();

		if (enemySpawnTimer.WaitTime > 0.5)
			enemySpawnTimer.WaitTime -= delta * 0.005;
		else if (enemySpawnTimer.WaitTime < 0.5)
			enemySpawnTimer.WaitTime = 0.5;

		var offset = parallaxBackground.Offset;
		offset.Y += (float)delta * scrollSpeed;
		if (offset.Y >= 960)
		{
			offset.Y = 0;
		}
		parallaxBackground.Offset = offset;
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
		laserSound.Play();
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
		enemy.Connect("HitEnemy", new Callable(this, nameof(OnHitEnemy)));
		enemyContainer.AddChild(enemy);
	}

	public void OnHitEnemy()
	{
		hitSound.Play();
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
		explosionEnemySound.Play();
	}

	public async void OnDestroyedPlayer()
	{
		explosionPlayerSound.Play();
		gameOverScreen.SetScore(Score);
		gameOverScreen.SetHighScore(HighScore);
		SaveGame();
		await ToSignal(GetTree().CreateTimer(1.5), "timeout");
		gameOverScreen.Visible = true;
	}
}
