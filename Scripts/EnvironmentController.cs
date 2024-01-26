using Godot;
using System;

public partial class EnvironmentController : Node
{
    [Export]
    public PackedScene WallScene;

    protected Vector2 WallDims = new Vector2(200, 100);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Window mainWindow = GetWindow();

        //create a horizontal row of fences along the screen
        //start with the top edge
        Vector2 curPos = new Vector2(-WallDims.X/2, WallDims.Y/2);
        while (curPos.X < mainWindow.Size.X + WallDims.X/2)
        {
            //GD.Print("spawning fence at (" + curPos + ")");
            Node2D wall = (Node2D)WallScene.Instantiate();
            AddChild(wall);
            wall.Position = curPos;
            curPos.X += WallDims.X;

            if (curPos.X >= mainWindow.Size.X + WallDims.X / 2)
            {
                if (curPos.Y <= WallDims.Y)
                {
                    //switch to the bottom edge of the screen
                    curPos = new Vector2(-WallDims.X / 2, mainWindow.Size.Y - WallDims.Y/2);
                }
                else
                {
                    break;
                }
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
