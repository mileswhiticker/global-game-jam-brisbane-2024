using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		var velocity = Velocity;
		velocity.X += 10.0f;
		Velocity = velocity;
		MoveAndSlide();
	}
}
