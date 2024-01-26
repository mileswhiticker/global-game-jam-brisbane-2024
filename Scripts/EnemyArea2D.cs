using Godot;
using System;
using System.Diagnostics;

public partial class EnemyArea2D : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void _on_body_shape_entered(CollisionObject2D collisionObject, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		GD.Print("Body shape entered.");
	}
}