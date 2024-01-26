using Godot;
using System;
using System.Text;

public partial class MobController : Node
{
	[Export]
	public PackedScene EnemyScene;
    [Export]
    public double tLeftSpawnEnemy = 1.0f;
    [Export]
    public double enemySpawnInterval = 3.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tLeftSpawnEnemy -= delta;
		if(tLeftSpawnEnemy < 0)
        {
            //GD.Print("spawning enemy");
            tLeftSpawnEnemy = enemySpawnInterval;
			SpawnEnemy();
        }
		//GD.Print(tLeftSpawnEnemy);
    }

	protected void SpawnEnemy()
    {
        if (EnemyScene != null)
        {
            Enemy mob = (Enemy)EnemyScene.Instantiate();
            AddChild(mob);
            Vector2 spawnPos = GetNextSpawnPos();
            mob.MoveTo(spawnPos);
        }
    }

    protected Vector2 GetNextSpawnPos()
    {
        Vector2 dimsLower = new Vector2(100, 100);
        Vector2 dimsUpper = new Vector2(100, 600);
        Vector2 spawnPos = new Vector2((float)GD.RandRange(dimsLower.X, dimsUpper.X), (float)GD.RandRange(dimsLower.Y, dimsUpper.Y));
        return spawnPos;
    }
}
