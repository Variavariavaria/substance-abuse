using Godot;
using System;
namespace SubstanceAbuse.UiElements.MainMenu;
public partial class MainMenu : Node
{
	private Button _playButton;
	private Button _settingsButton;
	private Button _exitButton;
	private Button _meetButton;
	private Control _meetButtonWrapper;
	public override void _Ready()
	{
		_playButton = GetNode<Button>("%StartButton");
		_settingsButton = GetNode<Button>("%SettingsButton");
		_exitButton = GetNode<Button>("%ExitButton");
		_meetButton = GetNode<Button>("%MeetButton");
		_meetButtonWrapper = GetNode<Control>("%MeetButtonWrapper");
		SetupMouseEnteredText(_playButton, "Играть", "descend");
		SetupMouseEnteredText(_settingsButton, "Настройки", "DNA");
		SetupMouseEnteredText(_exitButton, "Выход", "sleep");
		//_meetButton.Visible = IsMeetButtonHidden();
		_meetButtonWrapper.Visible = _meetButton.Visible;
		_playButton.GrabFocus();
		_playButton.Pressed += () => GetTree().ChangeSceneToFile("res://UIElements/StartGame/StartGame.tscn");
		_settingsButton.Pressed += () => GetTree().ChangeSceneToFile("res://UIElements/Settings/Settings.tscn");
		_exitButton.Pressed += () => GetTree().Quit();
		_meetButton.Pressed += () => GetTree().ChangeSceneToFile("res://UIElements/Dialogue/DialogueLayout.tscn");
	}

	private void SetupMouseEnteredText(Button button, string textFocusEntered, string textFocusExited)
	{
		button.MouseEntered += button.GrabFocus;
		button.FocusEntered += () => button.Text = textFocusEntered; 
		button.FocusExited += () => button.Text = textFocusExited;
	}

	private bool IsMeetButtonHidden()
	{
		return "чето там в общем при каких то условиях появляется эта кнопка".Contains("死");
	}
	public override void _Process(double delta)
	{
	}
}
