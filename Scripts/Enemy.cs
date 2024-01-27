using globalgamejam2024.Shared;
using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	//only take damage from a punch once
	protected int RecievedPunchID = 0;
    protected float health = 1;
    [Export] float MaxHealth = 1f;
    [Export] public int Speed { get; set; } = 150;
    protected int MeleeRange = 50;

    [Export] AnimatedSprite2D WalkAnim;
    [Export] AnimatedSprite2D PunchAnim;

    [Export] float BaseDamage = 1f;
    [Export] Area2D PunchArea2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //some basic settings
        health = MaxHealth;

        //initial animation settings
        WalkAnim.Play();
        PunchAnim.Visible = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        //where is the player?
        Vector2 moveVector = (GGJ.player.Position - Position).Normalized() * Speed;
        Velocity = moveVector;
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

	public void ReceivePunch(float damage)
	{
        if (GGJ.CurrentPunchID != RecievedPunchID && health > 0)
        {
			//only process a hit once per punch
            RecievedPunchID = GGJ.CurrentPunchID;
            //GD.Print("Ow! " + RecievedPunchID);
            health -= damage;

            if(health <= 0)
            {
                //die
                GGJ.mobController.KillEnemy(this);
            }
        }
    }

    public void _on_screen_exited()
    {
        //die
        //GD.Print("Goodbye");
        GGJ.mobController.KillEnemy(this);
    }

    public bool isPunching()
    {
        return PunchAnim.IsPlaying();
    }

    public void tryPunch()
    {
        //loop over all the overlapping enemies and apply a punch
        Godot.Collections.Array<Node2D> overlappingNodes = PunchArea2D.GetOverlappingBodies();
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
    }

    public void _on_punch_anim_finish()
    {
        PunchAnim.Visible = false;
        WalkAnim.Visible = true;
    }
}
