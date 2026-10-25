#nullable enable
using Godot;
using SubstanceAbuse.Characters;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.UI;

/// <summary>Вкладка «Инвентарь»: экипировка и строчный список предметов.</summary>
public partial class InventoryPage : Control
{
    private PlayerData? _player;
    private bool _built;

    private EquipmentSlotView _weaponSlot = null!;
    private EquipmentSlotView _armorSlot = null!;
    private Label _count = null!;
    private VBoxContainer _list = null!;

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var root = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        root.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        AddChild(root);
        root.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        root.AddChild(UiStyle.Header(UiStyle.Text.Equipment));

        _weaponSlot = new EquipmentSlotView();
        _weaponSlot.Configure(UiStyle.Text.EquippedWeapon, true, () => _player?.UnequipWeapon());
        root.AddChild(_weaponSlot);

        _armorSlot = new EquipmentSlotView();
        _armorSlot.Configure(UiStyle.Text.EquippedArmor, true, () => _player?.UnequipArmor());
        root.AddChild(_armorSlot);

        _count = UiStyle.Body(UiStyle.Text.ItemsCount(0));
        root.AddChild(_count);

        var scroll = new ScrollContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
        };
        root.AddChild(scroll);

        _list = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _list.AddThemeConstantOverride("separation", 4);
        scroll.AddChild(_list);

        _built = true;
        Refresh();
    }

    public void Bind(PlayerData player)
    {
        if (ReferenceEquals(_player, player))
        {
            Refresh();
            return;
        }

        Unsubscribe();
        _player = player;
        _player.InventoryChanged += Refresh;
        _player.EquipmentChanged += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        if (!_built || _player == null) return;

        _weaponSlot.SetItem(_player.Weapon);
        _armorSlot.SetItem(_player.Armor);
        _count.Text = UiStyle.Text.ItemsCount(_player.Inventory.Count);
        RebuildList();
    }

    public override void _ExitTree() => Unsubscribe();

    private void Unsubscribe()
    {
        if (_player == null) return;
        _player.InventoryChanged -= Refresh;
        _player.EquipmentChanged -= Refresh;
        _player = null;
    }

    private void RebuildList()
    {
        foreach (Godot.Node child in _list.GetChildren())
        {
            _list.RemoveChild(child);
            child.QueueFree();
        }

        if (_player == null) return;

        if (_player.Inventory.Count == 0)
        {
            var empty = UiStyle.Body(UiStyle.Text.Empty);
            empty.AddThemeColorOverride("font_color", UiStyle.MutedText);
            _list.AddChild(empty);
            return;
        }

        foreach (ItemData item in _player.Inventory)
        {
            _list.AddChild(CreateRow(item));
        }
    }

    private Control CreateRow(ItemData item)
    {
        var row = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        row.TooltipText = $"{item.ItemName}\nID: {item.ItemID}\nРедкость: {item.Rarity}";

        row.AddChild(UiStyle.CreateItemIcon(item));

        var name = UiStyle.Body(item.ItemName);
        name.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        name.ClipText = true;
        name.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        name.AddThemeColorOverride("font_color", UiStyle.RarityColor(item.Rarity));
        row.AddChild(name);

        if (item is WeaponData or ArmorData)
        {
            var equip = new Button { Text = UiStyle.Text.Equip };
            equip.Pressed += () => Equip(item);
            row.AddChild(equip);
        }

        return row;
    }

    private void Equip(ItemData item)
    {
        if (_player == null) return;

        if (item is WeaponData)
            _player.EquipWeapon(item);
        else if (item is ArmorData)
            _player.EquipArmor(item);
    }
}
