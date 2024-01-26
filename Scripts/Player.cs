using Godot;
using System;
using globalgamejam2024.Shared;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 500;
	[Export] public float Health { get; set; } = 100;

	public override void _Process(double delta)
	{
		/*
		var translatedVector = GetMovementVector();
		translatedVector = translatedVector.Normalized();
		translatedVector *= Speed * (float)delta;
		
		Transform = Transform.Translated(translatedVector);
		*/
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = GetMovementVector();
		//GD.Print(Velocity);

		// "MoveAndSlide" already takes delta time into account.
		 MoveAndSlide();
	}

	private Vector2 GetMovementVector()
	{
		var translatedVector = Vector2.Zero;
		
		if (Input.IsActionPressed(MovementDirection.Up))
		{
			translatedVector += Vector2.Up;
		}
		if (Input.IsActionPressed(MovementDirection.Down))
		{
			translatedVector += Vector2.Down;
		}
		if (Input.IsActionPressed(MovementDirection.Left))
		{
			translatedVector += Vector2.Left;
		}
		if (Input.IsActionPressed(MovementDirection.Right))
		{
			translatedVector += Vector2.Right;
		}

		translatedVector = translatedVector.Normalized();
		translatedVector *= Speed;

		return translatedVector;

	}
	
	[Signal] public delegate void HitPlayerEventHandler();
}