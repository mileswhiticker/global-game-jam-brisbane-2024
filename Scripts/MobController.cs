using globalgamejam2024.Shared;
using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public partial class MobController : Node
{
    [Export] public double tLeftSpawnEnemy = 0.5f;
    [Export] public double enemySpawnInterval = 3.0f;

    [Export] public double basePunchRate = 1.0f;
    [Export] public double tLeftPunch = 1.0f;

    [Export] public PackedScene EnemyScene;
    List<Enemy> spawnedEnemies = new List<Enemy>();
    List<Enemy> readyEnemies = new List<Enemy>();
    List<Enemy> tiredEnemies = new List<Enemy>();

    Random random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        GGJ.mobController = this;
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

        tLeftPunch -= delta;
        TryEnemyPunch();
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
            readyEnemies.RemoveAt(readyEnemies.Count - 1);
            punchingEnemy.tryPunch();

            //move them to a random spot on the "tired" list
            int index = random.Next(tiredEnemies.Count);
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
        if (EnemyScene != null)
        {
            Enemy mob = (Enemy)EnemyScene.Instantiate();
            AddChild(mob);
            Vector2 spawnPos = GetNextSpawnPos();
            mob.MoveTo(spawnPos);
            spawnedEnemies.Add(mob);
            tiredEnemies.Add(mob);
        }
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
