using globalgamejam2024.Shared;
using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public partial class MobController : Node
{
    public double tLeftSpawnEnemy = 0.5f;
    protected int enemiesSpawnLeft = 10;
    [Export] public double enemySpawnInterval = 1.0f;
    [Export] public int enemiesPerWave = 12;
    [Export] protected int enemiesMax = 6;

    [Export] public double basePunchRate = 1.0f;
    [Export] public double tLeftPunch = 1.0f;

    [Export] public Godot.Collections.Array<PackedScene> EnemyScene = new();
    List<Enemy> spawnedEnemies = new();
    List<Enemy> readyEnemies = new();
    List<Enemy> tiredEnemies = new();

    protected bool IsSpawning = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        GGJ.mobController = this;

        //some misc settings
        enemiesSpawnLeft = enemiesPerWave;
        tLeftSpawnEnemy = enemySpawnInterval;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if(IsSpawning && spawnedEnemies.Count < enemiesMax && enemiesSpawnLeft > 0)
        {
            tLeftSpawnEnemy -= delta;
        }

		if(tLeftSpawnEnemy < 0)
        {
            //GD.Print("spawning enemy");
            tLeftSpawnEnemy = enemySpawnInterval;
			SpawnEnemy();
            enemiesSpawnLeft--;
        }

        tLeftPunch -= delta;
        TryEnemyPunch();
    }

    public void StartSpawning()
    {
        IsSpawning = true;
    }

    public void StopSpawning()
    {
        IsSpawning = false;
    }

    protected void TryEnemyPunch()
    {
        //this means no matter how many enemies, there will only ever be a constant rate of punching
        if (tLeftPunch <= 0 & spawnedEnemies.Count > 0)
        {
            if(readyEnemies.Count == 0)
            {
                //swap the lists to "refresh" our tired enemies
                //GD.Print("swapping lists");
                readyEnemies = tiredEnemies;
                tiredEnemies = new List<Enemy>();
            }

            //pick a random enemy to punch
            Enemy punchingEnemy = readyEnemies[readyEnemies.Count - 1];

            //was the punch successful?
            if(punchingEnemy.TryAttack())
            {
                //
            }
            readyEnemies.RemoveAt(readyEnemies.Count - 1);

            //move them to a random spot on the "tired" list
            int index = GGJ.random.Next(tiredEnemies.Count);
            tiredEnemies.Insert(index, punchingEnemy);

            //how long is the cooldown until the next punch?
            double bonusPunch = 1;
            if (spawnedEnemies.Count > 1)
            {
                //every extra enemy means 10% more punches
                bonusPunch = Math.Pow(0.9f, spawnedEnemies.Count - 1);
            }
            tLeftPunch = (1 / (basePunchRate)) * bonusPunch;
        }
    }

	protected void SpawnEnemy()
    {
        if (EnemyScene == null) return;
        
        Enemy mob = (Enemy)EnemyScene[GGJ.random.Next(EnemyScene.Count)].Instantiate();
        AddChild(mob);
        Vector2 spawnPos = GetNextSpawnPos();
        mob.MoveTo(spawnPos);
        spawnedEnemies.Add(mob);
        tiredEnemies.Add(mob);
    }

    protected Vector2 GetNextSpawnPos()
    {
        Vector2 dimsLower = new Vector2(50, 100);
        Vector2 dimsUpper = new Vector2(50, 500);
        Vector2 spawnPos = new Vector2((float)GD.RandRange(dimsLower.X, dimsUpper.X), (float)GD.RandRange(dimsLower.Y, dimsUpper.Y));
        return spawnPos;
    }

    public void KillEnemy(Enemy deadEnemy)
    {
        //remove it from our lists
        spawnedEnemies.Remove(deadEnemy);
        readyEnemies.Remove(deadEnemy);
        tiredEnemies.Remove(deadEnemy);

        //have godot clean it up on the next frame
        deadEnemy.QueueFree();
    }
}
