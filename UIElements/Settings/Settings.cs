using Godot;

namespace SubstanceAbuse.UiElements.Settings;

public partial class Settings : Control
{
	private Button _audioTab;
	private Button _videoTab;
	private Button _controlsTab;
	private Button _backButton;
	private Control _audioPanel;
	private Control _videoPanel;
	private Control _controlsPanel;
	private HSlider _overallVolumeSlider;
	private HSlider _musicSlider;
	private HSlider _effectsSlider;
	private Label _overallVolumeValue;
	private Label _musicValue;
	private Label _effectsValue;
	public override void _Ready()
	{
		_audioTab = GetNode<Button>("%AudioButton");
		_videoTab = GetNode<Button>("%VideoButton");
		_controlsTab = GetNode<Button>("%ControlsButton");
		_backButton = GetNode<Button>("%BackButton");
		_audioPanel = GetNode<Control>("%AudioPanel");
		_videoPanel = GetNode<Control>("%VideoPanel");
		_controlsPanel = GetNode<Control>("%ControlsPanel");
		_overallVolumeSlider = GetNode<HSlider>("%OverallVolumeSlider");
		_musicSlider = GetNode<HSlider>("%MusicSlider");
		_effectsSlider = GetNode<HSlider>("%EffectsSlider");
		_overallVolumeValue = GetNode<Label>("%OverallVolumeValue");
		_musicValue = GetNode<Label>("%MusicValue");
		_effectsValue = GetNode<Label>("%EffectsValue");

		_audioTab.Pressed += () => ShowSection(SettingsSection.Audio);
		_videoTab.Pressed += () => ShowSection(SettingsSection.Video);
		_controlsTab.Pressed += () => ShowSection(SettingsSection.Controls);
		_backButton.Pressed += GoBack;

		_overallVolumeSlider.ValueChanged += _ => UpdateVolumeLabels();
		_musicSlider.ValueChanged += _ => UpdateVolumeLabels();
		_effectsSlider.ValueChanged += _ => UpdateVolumeLabels();

		ShowSection(SettingsSection.Audio);
		UpdateVolumeLabels();
		_audioTab.GrabFocus();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return;

		GetViewport().SetInputAsHandled();
		GoBack();
	}

	private void ShowSection(SettingsSection section)
	{
		_audioPanel.Visible = section == SettingsSection.Audio;
		_videoPanel.Visible = section == SettingsSection.Video;
		_controlsPanel.Visible = section == SettingsSection.Controls;
	}

	private void UpdateVolumeLabels()
	{
		_overallVolumeValue.Text = FormatVolume(_overallVolumeSlider.Value);
		_musicValue.Text = FormatVolume(_musicSlider.Value);
		_effectsValue.Text = FormatVolume(_effectsSlider.Value);
	}

	private static string FormatVolume(double value)
	{
		return $"{Mathf.RoundToInt(value)}%";
	}

	private void GoBack()
	{
		GetTree().ChangeSceneToFile("res://UIElements/MainMenu/MainMenu.tscn");
	}
}
