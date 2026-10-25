using Godot;
using System;

public partial class EventBus : Node
{
	public event Action<GameStateManager.GameStates, GameStateManager.GameStates> OnStateChanged;
	public event Action<int> OnPlayerHealthChanged;
	public event Action OnTraderOpened;
	public event Action<bool> OnTraderPopupChanged;

	public void EmitStateChanged(GameStateManager.GameStates oldState, GameStateManager.GameStates newState)
		=> OnStateChanged?.Invoke(oldState, newState);

	public void EmitPlayerHealthChanged(int currentHealth)
		=> OnPlayerHealthChanged?.Invoke(currentHealth);

	public void EmitTraderPopupChanged(bool isOpen)
		=> OnTraderPopupChanged?.Invoke(isOpen);
}
