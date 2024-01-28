using globalgamejam2024.Shared;
using Godot;

public partial class MainMenu : Node
{
    public void NewGame()
    {
        GGJ.gameController.NewGame();
    }

    public void QuitGame()
    {
        GGJ.gameController.QuitGame();
    }
}
