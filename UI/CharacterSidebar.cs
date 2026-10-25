#nullable enable
using Godot;
using SubstanceAbuse.Characters;
using SubstanceAbuse.Common;

namespace SubstanceAbuse.UI;

/// <summary>Левая панель персонажа с тремя вкладками.</summary>
public partial class CharacterSidebar : PanelContainer
{
    private PlayerData? _player;
    private bool _built;
    private int _current = -1;

    private Button[] _tabButtons = null!;
    private StatsPage _stats = null!;
    private InventoryPage _inventory = null!;
    private NotebookPage _notebook = null!;
    private Control[] _pages = null!;

    public override void _Ready()
    {
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        SizeFlagsVertical = SizeFlags.ExpandFill;
        AddThemeStyleboxOverride("panel", UiStyle.Box(UiStyle.PanelBackground, UiStyle.PanelPadding));

        var root = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        root.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        AddChild(root);
        root.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var tabRow = new HBoxContainer();
        tabRow.AddThemeConstantOverride("separation", 2);
        root.AddChild(tabRow);

        var group = new ButtonGroup();
        string[] titles =
        {
            UiStyle.Text.Character,
            UiStyle.Text.Inventory,
            UiStyle.Text.Notes,
        };

        _tabButtons = new Button[titles.Length];
        for (int i = 0; i < titles.Length; i++)
        {
            var button = new Button
            {
                Text = titles[i],
                ToggleMode = true,
                ButtonGroup = group,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                ClipText = true,
                CustomMinimumSize = new Vector2(0, 0),
            };
            button.AddThemeFontSizeOverride("font_size", 14);
            ApplyTabStyle(button);
            int index = i;
            button.Pressed += () => ShowTab(index);
            tabRow.AddChild(button);
            _tabButtons[i] = button;
        }

        var pageHolder = new Control
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        root.AddChild(pageHolder);

        _stats = new StatsPage();
        _inventory = new InventoryPage();
        _notebook = new NotebookPage();
        _pages = new Control[] { _stats, _inventory, _notebook };

        foreach (Control page in _pages)
        {
            pageHolder.AddChild(page);
            page.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            page.Visible = false;
        }

        _built = true;
        ShowTab(0);

        if (_player != null) BindPages(_player);
    }

    public void Bind(PlayerData player)
    {
        _player = player;
        if (_built) BindPages(player);
    }

    private void BindPages(PlayerData player)
    {
        _stats.Bind(player);
        _inventory.Bind(player);
        _notebook.Bind(player);
    }

    private void ShowTab(int index)
    {
        if (!_built) return;

        index = Mathf.Clamp(index, 0, _pages.Length - 1);
        if (_current == 2 && index != 2) _notebook.ReleaseInputFocus();

        _current = index;
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].Visible = i == index;
            _tabButtons[i].ButtonPressed = i == index;
        }

        GameLog.Info($"Вкладка: {_tabButtons[index].Text}");
    }

    private static void ApplyTabStyle(Button button)
    {
        button.AddThemeStyleboxOverride("normal", UiStyle.TabStyle(UiStyle.InactiveTabBackground));
        button.AddThemeStyleboxOverride("hover", UiStyle.TabStyle(UiStyle.ActiveTabBackground));
        button.AddThemeStyleboxOverride("pressed", UiStyle.TabStyle(UiStyle.ActiveTabBackground));
        button.AddThemeColorOverride("font_color", UiStyle.InactiveTabText);
        button.AddThemeColorOverride("font_hover_color", UiStyle.ActiveTabText);
        button.AddThemeColorOverride("font_pressed_color", UiStyle.ActiveTabText);
    }
}
