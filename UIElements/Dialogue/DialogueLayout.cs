using System.Collections.Generic;
using Godot;

namespace SubstanceAbuse.UiElements.Dialogue;

public partial class DialogueLayout : Control
{
	private const string MainMenuScene = "res://UIElements/MainMenu/MainMenu.tscn";
	private const int PlacementAttempts = 700;
	private const float ChoiceWidth = 280.0f;
	private const float ChoiceHeight = 52.0f;
	private const float PlacementMargin = 8.0f;
	private const float ChoicePadding = 10.0f;

	private static readonly string[] CharacterResponses =
	{
		"Я ждал тебя.",
		"Это место помнит нас.",
		"Ты сам выбрал этот путь.",
		"Не всё можно исправить.",
		"Теперь слушай."
	};

	private Button _backButton;
	private Control _dialogueContent;
	private Control _choices;
	private Control _npcResponsePanel;
	private Label _npcResponseText;
	private Button[] _choiceButtons;
	private readonly RandomNumberGenerator _rng = new();

	public override void _Ready()
	{
		_backButton = GetNode<Button>("%BackButton");
		_dialogueContent = GetNode<Control>("%DialogueContent");
		_choices = GetNode<Control>("%Choices");
		_npcResponsePanel = GetNode<Control>("%NpcResponsePanel");
		_npcResponseText = GetNode<Label>("%NpcResponseText");
		_choiceButtons = new[]
		{
			GetNode<Button>("%Choice1"),
			GetNode<Button>("%Choice2"),
			GetNode<Button>("%Choice3"),
			GetNode<Button>("%Choice4"),
			GetNode<Button>("%Choice5")
		};

		_rng.Randomize();
		_choices.Visible = false;
		_backButton.Pressed += GoBack;

		for (var i = 0; i < _choiceButtons.Length; i++)
		{
			var choiceIndex = i;
			_choiceButtons[i].Pressed += () => ShowCharacterResponse(choiceIndex);
		}

		RandomizeChoicesOnOpenAsync();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel")) return;

		GetViewport().SetInputAsHandled();
		GoBack();
	}

	private async void RandomizeChoicesOnOpenAsync()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		if (!IsInsideTree()) return;

		RandomizeChoices();
		_choices.Visible = true;
		_choiceButtons[0].GrabFocus();
	}

	private void RandomizeChoices()
	{
		var safeLeft = 44.0f;
		var safeTop = _backButton.Position.Y + _backButton.Size.Y + 24.0f;
		var safeRight = _dialogueContent.Size.X - 44.0f;
		var safeBottom = _npcResponsePanel.Position.Y - 20.0f;

		_choices.SetAnchorsPreset(LayoutPreset.TopLeft);
		_choices.Position = new Vector2(safeLeft, safeTop);
		_choices.Size = new Vector2(safeRight - safeLeft, safeBottom - safeTop);

		var minX = PlacementMargin;
		var maxX = _choices.Size.X - ChoiceWidth - PlacementMargin;
		var minY = PlacementMargin;
		var maxY = _choices.Size.Y - ChoiceHeight - PlacementMargin;

		if (maxX < minX || maxY < minY) return;

		var placedRects = new List<Rect2>();
		foreach (var button in _choiceButtons)
		{
			var position = FindFreePosition(minX, maxX, minY, maxY, placedRects);
			if (position == null) continue;

			button.Position = position.Value;
			button.Size = new Vector2(ChoiceWidth, ChoiceHeight);
			placedRects.Add(new Rect2(position.Value, new Vector2(ChoiceWidth, ChoiceHeight)));
		}
	}

	private Vector2? FindFreePosition(float minX, float maxX, float minY, float maxY, List<Rect2> placedRects)
	{
		for (var attempt = 0; attempt < PlacementAttempts; attempt++)
		{
			var position = new Vector2(
				_rng.RandfRange(minX, maxX),
				_rng.RandfRange(minY, maxY));
			var candidate = new Rect2(
				position,
				new Vector2(ChoiceWidth, ChoiceHeight)).Grow(ChoicePadding);

			if (!IntersectsAny(candidate, placedRects)) return position;
		}

		for (var y = minY; y <= maxY; y += 12.0f)
		{
			for (var x = minX; x <= maxX; x += 12.0f)
			{
				var position = new Vector2(x, y);
				var candidate = new Rect2(
					position,
					new Vector2(ChoiceWidth, ChoiceHeight)).Grow(ChoicePadding);

				if (!IntersectsAny(candidate, placedRects)) return position;
			}
		}

		return null;
	}

	private static bool IntersectsAny(Rect2 rect, List<Rect2> others)
	{
		foreach (var other in others)
		{
			if (rect.Intersects(other)) return true;
		}

		return false;
	}

	private void ShowCharacterResponse(int choiceIndex)
	{
		_npcResponseText.Text = CharacterResponses[choiceIndex];
	}

	private void GoBack()
	{
		GetTree().ChangeSceneToFile(MainMenuScene);
	}
}
