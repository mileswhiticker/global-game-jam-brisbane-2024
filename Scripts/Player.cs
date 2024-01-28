using Godot;
using System;
using globalgamejam2024.Shared;
using System.ComponentModel.Design;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 300;
	[Export] public float HealthMax { get; set; } = 5;
	private float Health = 20;
	[Export] public float BaseDamage { get; set; } = 1;

	[Export] public int DodgeBonusSpeed = 250;
	[Export] public double DodgeLength = 0.5f;
	protected double tLeftDodge = 0;

	[Export] AnimatedSprite2D WalkAnim;
	[Export] AnimatedSprite2D PunchAnim;
	[Export] AnimatedSprite2D IdleAnim;
	[Export] AnimatedSprite2D DodgeAnim;

	[Export] float PunchOffsetDist = 45;
	[Export] Area2D PunchArea2D;
	[Export] CollisionShape2D PunchShape2D;

	protected int NextPunchID = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//start out non-visible 
		WalkAnim.Visible = false;
		PunchAnim.Visible = false;

		//have these playing in the background
		IdleAnim.Play();
		WalkAnim.Play();
		DodgeAnim.Play();

		//some misc settings
		Health = HealthMax;
		GGJ.player = this;

		//these are now set in the editor, but ive left them here as an example of how to do it in code
		/*
		this.CollisionLayer = GGJCollisionLayers.Bitmask(GGJCollisionLayers.Player);
		this.CollisionMask = GGJCollisionLayers.Bitmask(GGJCollisionLayers.Player);
		PunchArea2D.CollisionLayer = GGJCollisionLayers.Bitmask(GGJCollisionLayers.Enemy);
		PunchArea2D.CollisionMask = GGJCollisionLayers.Bitmask(GGJCollisionLayers.Enemy);
		*/
	}

	public override void _Process(double delta)
	{
		/*
		var translatedVector = GetMovementVector();
		translatedVector = translatedVector.Normalized();
		translatedVector *= Speed * (float)delta;
		
		Transform = Transform.Translated(translatedVector);
		*/

		if(tLeftDodge > 0)
		{
			tLeftDodge -= delta;
			if (tLeftDodge <= 0)
			{
				FinishDodge();
			}
		}

		if (Input.IsActionPressed(PlayerAction.Punch))
		{
			TryPunchAttack();
		}

		//update the animation that is playing
		//first, only change the animation if we arent punching or dodging
		if (!PunchAnim.IsPlaying() && !DodgeAnim.Visible)
		{
			//are we moving?
			//GD.Print(Velocity.LengthSquared());
			if (Velocity.LengthSquared() > 0.1)
			{
				if (!WalkAnim.Visible)
				{
					//enable walk anim
					WalkAnim.Visible = true;

					//disable idle anim
					IdleAnim.Visible = false;
				}
			}
			else
			{
				if (!IdleAnim.Visible)
				{
					//enable idle anim
					IdleAnim.Visible = true;

					//disable walk anim
					WalkAnim.Visible = false;
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 movementVector = GetMovementVector();
		Velocity = movementVector;

		//GD.Print(Velocity);

		// "MoveAndSlide" already takes delta time into account.
		//MoveAndSlide();

		//MoveAndCollide() is the same as MoveAndSlide() except we get collision info as well (i think?)
		KinematicCollision2D collisionInfo = MoveAndCollide(movementVector * (float)delta);

		//if we hit a fence, finish dodge roll early
		if(collisionInfo != null && tLeftDodge > 0)
		{
			FinishDodge();
		}
	}

	private Vector2 GetMovementVector()
	{
		//if we are dodging, maintain a constant speed and direction
		if (tLeftDodge > 0)
		{
			return Velocity;
		}

		var translatedVector = Vector2.Zero;
		var calculatedSpeed = Speed;

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
			SetFacingRight(false);
		}
		if (Input.IsActionPressed(MovementDirection.Right))
		{
			translatedVector += Vector2.Right;
			SetFacingRight(true);
		}
		if (Input.IsActionPressed(PlayerAction.Dodge) && translatedVector.LengthSquared() != 0f)
		{
			StartDodge();
			calculatedSpeed += DodgeBonusSpeed;
		}

		translatedVector = translatedVector.Normalized();
		translatedVector *= calculatedSpeed;

		return translatedVector;

	}

	public void StartDodge()
	{
		//update the animations
		tLeftDodge = DodgeLength;
		DodgeAnim.Visible = true;
		WalkAnim.Visible = false;
		IdleAnim.Visible = false;
	}

	public void FinishDodge()
	{
		tLeftDodge = 0;

		//update the animations
		DodgeAnim.Visible = false;
		WalkAnim.Visible = false;
		IdleAnim.Visible = true;
	}

	public void SetFacingRight(bool faceRight)
	{
		WalkAnim.FlipH = faceRight;
		IdleAnim.FlipH = faceRight;
		PunchAnim.FlipH = !faceRight;

		if (faceRight)
		{
			PunchShape2D.Position = new Vector2(PunchOffsetDist, 0);
		}
		else
		{
			PunchShape2D.Position = new Vector2(-PunchOffsetDist, 0);
		}
	}

	public void TryPunchAttack()
	{
		//are we already punching?
		if (!PunchAnim.IsPlaying())
		{
			//only one unique player punch at a time
			GGJ.CurrentPunchID = ++NextPunchID;

			//GD.Print("PunchArea2D: " + PunchArea2D.CollisionLayer + "/" + PunchArea2D.CollisionMask);

			//loop over all the overlapping enemies and apply a punch
			Godot.Collections.Array<Node2D> overlappingNodes = PunchArea2D.GetOverlappingBodies();
			foreach (Node2D checkNode in overlappingNodes)
			{
				//PhysicsBody2D physicsBody = (PhysicsBody2D)checkNode;
				//GD.Print(checkNode.Name + " " + physicsBody.CollisionLayer + "/" + physicsBody.CollisionMask);

				//this is safe so long as the collision masks are properly setup
				Enemy punchedEnemy = (Enemy)checkNode;
				punchedEnemy.ReceivePunch(BaseDamage);
			}

			//enable punch anim
			PunchAnim.Visible = true;
			PunchAnim.Play();

			//disable walk anim
			WalkAnim.Visible = false;

			//disable idle anim
			IdleAnim.Visible = false;
		}
	}

	public void ReceivePunch(float damage)
	{
		//dont take damage while dodging
		if(tLeftDodge > 0)
		{
			return;
		}

		if (Health > 0)
		{
			Health -= damage;
			GD.Print("Ow! Health: " + Health + "/" + HealthMax);

			if (Health <= 0)
			{
				//die
				GD.Print("You have died!");
			}
		}
		else
		{
			GD.Print("Ow!");
		}
	}

	public void FinishPunchAttack()
	{
		PunchAnim.Visible = false;
	}

	[Signal] public delegate void HitPlayerEventHandler();
}
