using Godot;
using System;
using globalgamejam2024.Shared;

public partial class Chicken : CharacterBody2D
{
	//only take damage from a punch once
	protected int ReceivedPunchID = 0;
    protected float Health = 1;

    [Export] public float MaxHealth = 1f;
    [Export] public int Speed { get; set; } = 150;
    protected int MeleeRange = 50;
    protected int MeleeRangeSqrd = 50 * 50;

    [Export] public double IdleTime = 0.5f; 
    double tLeftIdle = 0f;

    [Export] public AnimatedSprite2D IdleAnim;
    [Export] public AnimatedSprite2D WalkAnim;
    [Export] public AnimatedSprite2D PunchAnim;

    [Export] public float BaseDamage = 1f;
    [Export] public Area2D PunchArea2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Enemy ready.");
        
        //some basic settings
        Health = MaxHealth;

        //initial animation settings
        WalkAnim.Play();
        IdleAnim.Play();
        PunchAnim.Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if (tLeftIdle > 0)
        {
            tLeftIdle -= delta;
            if (tLeftIdle <= 0)
            {
                WalkAnim.Visible = true;
                IdleAnim.Visible = false;
                tLeftIdle = 0;
            }
        }
    }

    public void MoveTo(Vector2 newPos)
	{
        //GD.Print("Enemy::MoveTo(" + newPos.X + "," + newPos.Y + ")");
        Position = newPos;
	}

	public override void _PhysicsProcess(double delta)
    {
        //are we idling?
        if(tLeftIdle > 0)
        {
            return;
        }

        //where is the player?
        Vector2 moveVector = new Vector2();
        if ((GGJ.player.Position - Position).LengthSquared() > MeleeRangeSqrd)
        {
            moveVector = (GGJ.player.Position - Position).Normalized() * Speed * (float)delta;
            //WalkAnim.Visible = true;
            //IdleAnim.Visible = false;
        }
        else
        {
            //WalkAnim.Visible = false;
            //IdleAnim.Visible = true;
        }
        KinematicCollision2D collisionInfo = MoveAndCollide(moveVector);

        //if we bumped into something, go idle for a short while
        if(collisionInfo != null && GGJ.random.Next(2) == 1) 
        {
            GoIdle();
            WalkAnim.Visible = false;
            IdleAnim.Visible = true;
        }
    }

    public void GoIdle()
    {
        tLeftIdle = IdleTime;
    }


    public void ReceivePunch(float damage)
	{
        if (GGJ.CurrentPunchID != ReceivedPunchID && Health > 0)
        {
			//only process a hit once per punch
            ReceivedPunchID = GGJ.CurrentPunchID;
            //GD.Print("Ow! " + RecievedPunchID);
            Health -= damage;

            if(Health <= 0)
            {
                //die
                //GGJ.mobController.KillEnemy(this);
            }
        }
    }

    public void _on_screen_exited()
    {
        //die
        //GD.Print("Goodbye");
        //GGJ.mobController.KillEnemy(this);
    }

    public bool isPunching()
    {
        return PunchAnim.IsPlaying();
    }

    public virtual bool TryAttack()
    {
        //are we idling?
        if (tLeftIdle > 0)
        {
            return false;
        }
        //loop over all the overlapping enemies and apply a punch
        var overlappingNodes = PunchArea2D.GetOverlappingBodies();
        foreach (Node2D checkNode in overlappingNodes)
        {
            //PhysicsBody2D physicsBody = (PhysicsBody2D)checkNode;
            //GD.Print(checkNode.Name + " " + physicsBody.CollisionLayer + "/" + physicsBody.CollisionMask);

            //this is safe so long as the collision masks are properly setup
            Player punchedPlayer = (Player)checkNode;
            punchedPlayer.ReceivePunch(BaseDamage);
        }

        //play the correct animation
        PunchAnim.Play();
        PunchAnim.Visible = true;
        WalkAnim.Visible = false;

        return true;
    }

    public void _on_punch_anim_finish()
    {
        PunchAnim.Visible = false;
        WalkAnim.Visible = true;
    }
}
