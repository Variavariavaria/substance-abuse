using Godot;
using System;

public partial class GameStateManager : Node
{
	public enum GameStates
	{
		MainMenu,
		GameStarting,
		GameRunning,
		GameEnding,
		SettingsScreen,
		SaveGame,
		LoadGame
	}

	private GameStates _state = GameStates.MainMenu;

	public GameStates StateCurrent
	{
		get => _state;
		set
		{
			if (_state == value) return;
			var old = _state;
			_state = value;
			Core.Instance.Bus.EmitStateChanged(old, _state);
		}
	}

	private bool _traderPopup;
	public bool TraderPopup
	{
		get => _traderPopup;
		set
		{
			if (_traderPopup == value) return;
			_traderPopup = value;
			Core.Instance.Bus.EmitTraderPopupChanged(value);
		}
	}

	public bool CraftingTablePopup { get; set; } //доделать
	public bool KitchenPopup { get; set; } //доделать
	//etc.
}
