using Godot;
using System;

public partial class EventBus : Node
{
	public event Action<GameStates, GameStates> OnStateChanged;
	public event Action<int> OnPlayerHealthChanged;
	public event Action OnTraderOpened;

	public void EmitStateChanged(GameState oldState, GameState newState)
		=> OnStateChanged?.Invoke(oldState, newState);

	public void EmitPlayerHealthChanged(int currentHealth)
		=> OnPlayerHealthChanged?.Invoke(currentHealth);

	public void EmitTraderPopupChanged(bool isOpen)
		=> OnTraderPopupChanged?.Invoke(isOpen);
}
