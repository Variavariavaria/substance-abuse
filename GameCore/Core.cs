using Godot;
using System;

public partial class Core : Node
{
	public static Core Instance { get; private set; }
	
	public RNG Random { get; private set; }
	
	public SceneLoader Scenes => GetNode<SceneLoader>("/root/SceneLoader");
	public GameStateManager State => GetNode<GameStateManager>("/root/GameStateManager");
	public EventBus Bus => GetNode<EventBus>("/root/EventBus");

	public override void _Ready()
	{
		Instance = this;
		Random = new RNG();
		Random.SetSeed("Dev_Seed_12345");
	}
}
