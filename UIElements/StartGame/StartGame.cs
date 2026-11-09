using Godot;

namespace SubstanceAbuse.UiElements.StartGame;

public partial class StartGame : Control
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return;

		GetViewport().SetInputAsHandled();
		GetTree().ChangeSceneToFile("res://UIElements/MainMenu/MainMenu.tscn");
	}
}
