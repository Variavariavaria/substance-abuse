using Godot;
using System;
using SubstanceAbuse.GameCore;

public partial class GameStateManager : Node
{

	private GameState _state = GameState.MainMenu;

	public GameState StateCurrent
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
