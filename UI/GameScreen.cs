#nullable enable
using System;
using Godot;
using SubstanceAbuse.Characters;
using SubstanceAbuse.Common;
using SubstanceAbuse.Items;
#if DEBUG
using SubstanceAbuse.Dev;
#endif

namespace SubstanceAbuse.UI;

/// <summary>Игровой экран: левая панель персонажа (1/3) и зона мира (2/3).</summary>
public partial class GameScreen : Control
{
    public Control WorldArea { get; private set; } = null!;

    private CharacterSidebar _sidebar = null!;
    private PlayerData? _player;

#if DEBUG
    private int _nextItemIndex;
    private int _nextNoteIndex;
#endif

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var columns = new HBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        columns.AddThemeConstantOverride("separation", 0);
        AddChild(columns);
        columns.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        _sidebar = new CharacterSidebar
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsStretchRatio = 1,
        };
        columns.AddChild(_sidebar);

        WorldArea = new ColorRect
        {
            Name = "WorldArea",
            Color = UiStyle.WorldBackground,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsStretchRatio = 2,
        };
        columns.AddChild(WorldArea);

        Resized += LogProportions;

        if (_player == null)
        {
#if DEBUG
            BindPlayer(DevSeed.CreateTestPlayer());
#else
            GameLog.Error("GameScreen: игрок не привязан, панель пуста");
#endif
        }

        CallDeferred(nameof(LogProportions));
    }

    /// <summary>Подключает данные игрока к левой панели.</summary>
    public void BindPlayer(PlayerData player)
    {
        try
        {
            _player = player;
            _sidebar.Bind(player);
            GameLog.Info("GameScreen: игрок привязан к панели");
        }
        catch (Exception ex)
        {
            GameLog.Error(ex);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
#if DEBUG
        if (_player == null) return;
        if (@event is not InputEventKey key || !key.Pressed || key.Echo) return;

        switch (key.Keycode)
        {
            case Key.F1:
                _player.GiveXP(10);
                break;
            case Key.F2:
                ItemData[] items = DevSeed.TestItems;
                _player.AddItem(items[_nextItemIndex++ % items.Length]);
                break;
            case Key.F3:
                NoteData[] notes = DevSeed.TestNotes;
                _player.Notebook.AddNote(notes[_nextNoteIndex++ % notes.Length]);
                break;
            case Key.F4:
                _player.ModifyHealth(-1);
                _player.ModifyStamina(-1);
                _player.ModifySanity(-1);
                break;
            case Key.F9:
                PlayerDataSelfTest.Run();
                break;
            default:
                return;
        }

        GetViewport().SetInputAsHandled();
#endif
    }

    private void LogProportions()
    {
        try
        {
            float left = _sidebar.Size.X;
            float right = WorldArea.Size.X;
            float total = left + right;
            if (total <= 0) return;

            GameLog.Info($"Пропорция панелей: Left={left:0} Right={right:0} Ratio={left / total:0.000}");
        }
        catch (Exception ex)
        {
            GameLog.Error(ex);
        }
    }
}
