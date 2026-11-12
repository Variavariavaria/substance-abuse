#nullable enable
using System;
using Godot;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.UI;

/// <summary>Переиспользуемый слот экипировки: заголовок, предмет и необязательная кнопка «Снять».</summary>
public partial class EquipmentSlotView : PanelContainer
{
    private readonly VBoxContainer _root = new();
    private readonly Label _header = new();
    private readonly HBoxContainer _row = new();
    private readonly Control _iconHolder = new();
    private readonly Label _name = new();
    private readonly Label _details = new();
    private readonly Button _unequip = new();

    private string _title = "";
    private bool _showUnequip;
    private Action? _onUnequip;
    private ItemData? _item;
    private bool _built;

    public override void _Ready()
    {
        AddThemeStyleboxOverride("panel", UiStyle.Box(UiStyle.SlotBackground, 4));

        _header.AddThemeColorOverride("font_color", UiStyle.MutedText);
        _header.AddThemeFontSizeOverride("font_size", UiStyle.FontSize);
        _root.AddThemeConstantOverride("separation", 2);
        _root.AddChild(_header);

        _row.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        _row.AddChild(_iconHolder);

        _name.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _name.ClipText = true;
        _name.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        _row.AddChild(_name);

        _details.AddThemeColorOverride("font_color", UiStyle.MutedText);
        _details.AddThemeFontSizeOverride("font_size", UiStyle.FontSize - 2);
        _row.AddChild(_details);

        _unequip.Text = UiStyle.Text.Unequip;
        _unequip.Pressed += OnUnequipPressed;
        _row.AddChild(_unequip);

        _root.AddChild(_row);
        AddChild(_root);

        _built = true;
        ApplyHeader();
        ApplyItem();
    }

    public void Configure(string title, bool showUnequip, Action? onUnequip = null)
    {
        _title = title;
        _showUnequip = showUnequip;
        _onUnequip = onUnequip;

        if (_built) ApplyHeader();
    }

    public void SetItem(ItemData? item)
    {
        _item = item;
        if (_built) ApplyItem();
    }

    private void OnUnequipPressed()
    {
        try
        {
            _onUnequip?.Invoke();
        }
        catch (Exception ex)
        {
            Common.GameLog.Error(ex);
        }
    }

    private void ApplyHeader()
    {
        _header.Text = _title;
        _unequip.Visible = _showUnequip && _item != null;
    }

    private void ApplyItem()
    {
        for (int i = _iconHolder.GetChildCount() - 1; i >= 0; i--)
        {
            Godot.Node child = _iconHolder.GetChild(i);
            _iconHolder.RemoveChild(child);
            child.QueueFree();
        }

        _iconHolder.AddChild(UiStyle.CreateItemIcon(_item));

        if (_item == null)
        {
            _name.Text = UiStyle.Text.None;
            _name.AddThemeColorOverride("font_color", UiStyle.MutedText);
            _details.Text = "";
        }
        else
        {
            _name.Text = _item.ItemName;
            _name.AddThemeColorOverride("font_color", UiStyle.RarityColor(_item.Rarity));
            _details.Text = _item switch
            {
                WeaponData weapon => UiStyle.Text.WeaponDamage(weapon),
                ArmorData armor => UiStyle.Text.ArmorDefence(armor),
                _ => "",
            };
        }

        _unequip.Visible = _showUnequip && _item != null;
    }
}
