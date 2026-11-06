using Godot;
using System;
namespace SubstanceAbuse.UiElements;
public partial class MainMenu : Node
{
	private Button _playButton;
	private Button _settingsButton;
	private Button _exitButton;
	private Button _meetButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playButton = GetNode<Button>("%StartButton");
		_settingsButton = GetNode<Button>("%SettingsButton");
		_exitButton = GetNode<Button>("%ExitButton");
		_meetButton = GetNode<Button>("%MeetButton");
		SetupMouseEnteredText(_playButton, "Играть", "descend");
		SetupMouseEnteredText(_settingsButton, "Настройки", "DNA");
		SetupMouseEnteredText(_exitButton, "Выход", "sleep");
		_playButton.GrabFocus();
		_meetButton.Hide();
	}

	private void SetupMouseEnteredText(Button button, string textFocusEntered, string textFocusExited)
	{
		button.MouseEntered += button.GrabFocus;
		button.FocusEntered += () => button.Text = textFocusEntered; 
		button.FocusExited += () => button.Text = textFocusExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
