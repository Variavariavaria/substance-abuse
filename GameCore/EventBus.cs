using Godot;
using System;
namespace SubstanceAbuse.GameCore;

public partial class EventBus : Node
{
	public event Action<GameState, GameState> OnStateChanged;
	public event Action<int> OnPlayerHealthChanged;
	public event Action<bool> OnTraderOpened;

	public void EmitStateChanged(GameState oldState, GameState newState)
		=> OnStateChanged?.Invoke(oldState, newState);

	public void EmitPlayerHealthChanged(int currentHealth)
		=> OnPlayerHealthChanged?.Invoke(currentHealth);

	public void EmitTraderPopupChanged(bool isOpen)
		=> OnTraderOpened?.Invoke(isOpen);
}
