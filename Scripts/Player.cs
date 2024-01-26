using Godot;
using System;
using globalgamejam2024.Shared;
using System.ComponentModel.Design;

public partial class Player : CharacterBody2D
{
	[Export] public int Speed { get; set; } = 500;
	[Export] public float Health { get; set; } = 100;

    [Export] AnimatedSprite2D WalkAnim;
    [Export] AnimatedSprite2D PunchAnim;
    [Export] AnimatedSprite2D IdleAnim;

	[Export] float PunchOffsetDist = 45;
    [Export] Area2D PunchArea2D;
    [Export] CollisionShape2D PunchShape2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        WalkAnim.Visible = false;
        PunchAnim.Visible = false;
		IdleAnim.Play();
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
				if (!WalkAnim.IsPlaying())
				{
                    //enable walk anim
                    WalkAnim.Visible = true;
                    WalkAnim.Play();

					//disable idle anim
					IdleAnim.Stop();
					IdleAnim.Visible = false;
				}
            }
            else
            {
                if (!IdleAnim.IsPlaying())
                {
                    //enable idle anim
                    IdleAnim.Visible = true;
                    IdleAnim.Play();

                    //disable walk anim
                    WalkAnim.Stop();
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
            //enable punch hitbox
            PunchArea2D.Monitorable = true;
            PunchArea2D.Monitoring = true;

            //enable punch anim
            PunchAnim.Visible = true;
            PunchAnim.Play();

            //disable walk anim
            WalkAnim.Stop();
            WalkAnim.Visible = false;

            //disable idle anim
            IdleAnim.Stop();
            IdleAnim.Visible = false;
        }
    }

    public void FinishPunchAttack()
    {
        PunchAnim.Visible = false;
        PunchArea2D.Monitorable = false;
        PunchArea2D.Monitoring = false;
    }

	[Signal] public delegate void HitPlayerEventHandler();
}