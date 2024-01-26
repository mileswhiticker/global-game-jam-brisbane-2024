using globalgamejam2024.Shared;
using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	//only take damage from a punch once
	protected int RecievedPunchID = 0;
    protected float health = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        var velocity = Velocity;
        velocity.X = 200.0f;
        Velocity = velocity;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void MoveTo(Vector2 newPos)
	{
        //GD.Print("Enemy::MoveTo(" + newPos.X + "," + newPos.Y + ")");
        Position = newPos;
	}

	public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

	public void RecievePunch(float damage)
	{
        if (Global.CurrentPunchID != RecievedPunchID && health > 0)
        {
			//only process a hit once per punch
            RecievedPunchID = Global.CurrentPunchID;
            //GD.Print("Ow! " + RecievedPunchID);
            health -= damage;

            if(health <= 0)
            {
                //die
                QueueFree();
            }
        }
    }
    public void _on_screen_exited()
    {
        //die
        //GD.Print("Goodbye");
        QueueFree();
    }
}
