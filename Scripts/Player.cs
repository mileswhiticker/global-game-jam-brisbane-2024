using Godot;
using System;
using globalgamejam2024.Shared;
using System.ComponentModel.Design;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 500;
	[Export] public float Health { get; set; } = 100;
    [Export] public float BaseDamage { get; set; } = 1;

    [Export] AnimatedSprite2D WalkAnim;
    [Export] AnimatedSprite2D PunchAnim;
    [Export] AnimatedSprite2D IdleAnim;

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

		if (Input.IsActionPressed(PlayerAction.Punch))
		{
			TryPunchAttack();
		}

        //update the animation that is playing
        //first, only change the animation if we arent punching
        if (!PunchAnim.IsPlaying())
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
            SetFacingRight(false);
        }
		if (Input.IsActionPressed(MovementDirection.Right))
		{
			translatedVector += Vector2.Right;
			SetFacingRight(true);
        }

		translatedVector = translatedVector.Normalized();
		translatedVector *= Speed;

		return translatedVector;

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
                punchedEnemy.RecievePunch(BaseDamage);
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

    public void FinishPunchAttack()
    {
        PunchAnim.Visible = false;
    }

	[Signal] public delegate void HitPlayerEventHandler();
}