using Godot;
public partial class UI : Node
{
	[Export] public TextureProgressBar? ProgressBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (ProgressBar == null) return;
		
		
		GD.Print(ProgressBar.Name);		
		GD.Print(ProgressBar.Value);		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public double TakeDamage()
	{
		if (ProgressBar == null) return 0;

		ProgressBar.Value -= 1;
		return ProgressBar.Value;
	}
}