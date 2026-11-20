using Godot;

namespace SubstanceAbuse.UiElements.StartGame;

public partial class StartGame : Control
{
	public override void _Ready()
	{
		GetNode<Button>("%NewGameButton").Pressed +=
			() => GetTree().ChangeSceneToFile("res://UI/GameScreen.tscn");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return;

		GetViewport().SetInputAsHandled();
		GetTree().ChangeSceneToFile("res://UIElements/MainMenu/MainMenu.tscn");
	}
}
