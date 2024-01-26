using Godot;
using System;

public partial class EnvironmentController : Node
{
    [Export]
    public PackedScene WallScene;

    [Export]
    public Node TopWallHolder;
    [Export]
    public Node BottomWallHolder;

    protected Vector2 WallDims = new Vector2(200, 100);
    protected double GameTime = 0;
    protected bool Initialised = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GameTime += delta;
        if (!Initialised && GameTime > 0)
        {
            Initialised = true;
            CreateInitialWorld();
        }

    }

    protected void CreateInitialWorld()
    {
        Window mainWindow = GetWindow();

        //create a horizontal row of fences along the screen
        //start with the top edge
        Vector2 curPos = new Vector2(-WallDims.X / 2, WallDims.Y / 2);
        Node wallHolder = TopWallHolder;
        while (curPos.X < mainWindow.Size.X + WallDims.X / 2)
        {
            //GD.Print("spawning fence at (" + curPos + ")");
            Node2D wall = (Node2D)WallScene.Instantiate();
            wallHolder.AddSibling(wall);
            //GD.Print(wall.GetParent().Name);
            wall.Position = curPos;
            curPos.X += WallDims.X;

            if (curPos.X >= mainWindow.Size.X + WallDims.X / 2)
            {
                if (curPos.Y <= WallDims.Y)
                {
                    //switch to the bottom edge of the screen
                    curPos = new Vector2(-WallDims.X / 2, mainWindow.Size.Y - WallDims.Y / 2);
                    wallHolder = BottomWallHolder;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
