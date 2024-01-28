using Godot;
using System;

public partial class GameUIController : CanvasLayer
{
	[Export] public TextureProgressBar? ProgressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (ProgressBar == null) return;

		Visible = false;
		GD.Print(ProgressBar.Name);		
		GD.Print(ProgressBar.Value);		
	}

	public double TakeDamage()
	{
		if (ProgressBar == null) return 0;

		ProgressBar.Value -= 1;
		return ProgressBar.Value;
	}

	public void ResetHealth()
	{
		if (ProgressBar == null) return;
		
		ProgressBar.Value = ProgressBar.MaxValue;
	}
}
