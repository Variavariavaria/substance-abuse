#nullable enable
using Godot;
using SubstanceAbuse.Characters;

namespace SubstanceAbuse.UI;

/// <summary>Вкладка «Персонаж»: аватар, полосы, сводка, параметры, экипировка, эффекты.</summary>
public partial class StatsPage : Control
{
    private PlayerData? _player;
    private bool _built;

    private Label _level = null!;
    private Label _xp = null!;
    private Label _points = null!;
    private Label _damage = null!;
    private ProgressBar _xpBar = null!;
    private ProgressBar _healthBar = null!;
    private ProgressBar _staminaBar = null!;
    private ProgressBar _sanityBar = null!;
    private Label _healthValue = null!;
    private Label _staminaValue = null!;
    private Label _sanityValue = null!;
    private readonly Label[] _skillLevels = new Label[4];
    private readonly Button[] _skillButtons = new Button[4];
    private EquipmentSlotView _weaponSlot = null!;
    private EquipmentSlotView _armorSlot = null!;

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var scroll = new ScrollContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
        };
        AddChild(scroll);
        scroll.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var content = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        content.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        scroll.AddChild(content);

        content.AddChild(Header(UiStyle.Text.Character));

        var avatarRow = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        avatarRow.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        content.AddChild(avatarRow);

        var avatar = new PanelContainer
        {
            CustomMinimumSize = new Vector2(UiStyle.AvatarSize, UiStyle.AvatarSize),
        };
        avatar.AddThemeStyleboxOverride("panel", UiStyle.Box(UiStyle.AvatarBackground));
        avatarRow.AddChild(avatar);
        avatar.AddChild(new Label
        {
            Text = UiStyle.Text.Avatar,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        });

        var bars = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        bars.AddThemeConstantOverride("separation", 4);
        avatarRow.AddChild(bars);

        AddBar(bars, UiStyle.Text.Health, UiStyle.HealthColor, out _healthBar, out _healthValue);
        AddBar(bars, UiStyle.Text.Stamina, UiStyle.StaminaColor, out _staminaBar, out _staminaValue);
        AddBar(bars, UiStyle.Text.Sanity, UiStyle.SanityColor, out _sanityBar, out _sanityValue);

        _level = Body(UiStyle.Text.LevelValue(1));
        content.AddChild(_level);

        _xp = Body(UiStyle.Text.ExperienceValue(0, 10));
        content.AddChild(_xp);

        _xpBar = new ProgressBar { ShowPercentage = false, CustomMinimumSize = new Vector2(0, 8) };
        UiStyle.StyleBar(_xpBar, UiStyle.Accent);
        _xpBar.AddThemeStyleboxOverride("background", UiStyle.Box(UiStyle.BarBackground));
        content.AddChild(_xpBar);

        _points = Body(UiStyle.Text.UpgradePointsValue(0));
        content.AddChild(_points);

        _damage = Body(UiStyle.Text.DamageModValue(1.0));
        content.AddChild(_damage);

        content.AddChild(Header(UiStyle.Text.Parameters));
        content.AddChild(BuildSkillGrid());

        content.AddChild(Header(UiStyle.Text.Equipment));
        var equipment = new VBoxContainer();
        equipment.AddThemeConstantOverride("separation", 4);
        equipment.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        content.AddChild(equipment);

        _weaponSlot = new EquipmentSlotView();
        _weaponSlot.Configure(UiStyle.Text.EquippedWeapon, false);
        equipment.AddChild(_weaponSlot);

        _armorSlot = new EquipmentSlotView();
        _armorSlot.Configure(UiStyle.Text.EquippedArmor, false);
        equipment.AddChild(_armorSlot);

        content.AddChild(Header(UiStyle.Text.Effects));
        var effects = new VBoxContainer();
        effects.AddThemeConstantOverride("separation", 2);
        effects.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        var noEffects = Body(UiStyle.Text.NoEffects);
        noEffects.AddThemeColorOverride("font_color", UiStyle.MutedText);
        effects.AddChild(noEffects);
        content.AddChild(effects);

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
        _player.StatsChanged += Refresh;
        _player.EquipmentChanged += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        if (!_built || _player == null) return;

        PlayerData player = _player;

        _level.Text = UiStyle.Text.LevelValue(player.Level);
        _xp.Text = UiStyle.Text.ExperienceValue(player.XP, player.XPtoLVLup);
        _xpBar.MaxValue = player.XPtoLVLup;
        _xpBar.Value = player.XP;
        _points.Text = UiStyle.Text.UpgradePointsValue(player.UpgradePoints);
        _points.AddThemeColorOverride("font_color",
            player.UpgradePoints > 0 ? UiStyle.Accent : UiStyle.TextColor);
        _damage.Text = UiStyle.Text.DamageModValue(player.DamageMod);

        UpdateBar(_healthBar, _healthValue, player.HealthPointsCurrent, player.HealthPointsMax);
        UpdateBar(_staminaBar, _staminaValue, player.StaminaCurrent, player.StaminaMax);
        UpdateBar(_sanityBar, _sanityValue, player.SanityPointsCurrent, player.SanityPointsMax);

        for (int i = 0; i < _skillButtons.Length; i++)
        {
            _skillLevels[i].Text = SkillLevel(i).ToString();
            _skillButtons[i].Disabled =
                player.UpgradePoints <= 0 ||
                (i == 0 && player.Dedication >= PlayerData.MaxDedicationLevel);
        }

        _weaponSlot.SetItem(player.Weapon);
        _armorSlot.SetItem(player.Armor);
    }

    public override void _ExitTree() => Unsubscribe();

    private void Unsubscribe()
    {
        if (_player == null) return;
        _player.StatsChanged -= Refresh;
        _player.EquipmentChanged -= Refresh;
        _player = null;
    }

    private static void UpdateBar(ProgressBar bar, Label value, int current, int max)
    {
        bar.MaxValue = max;
        bar.Value = current;
        value.Text = UiStyle.Text.Fraction(current, max);
    }

    private GridContainer BuildSkillGrid()
    {
        var grid = new GridContainer { Columns = 3, SizeFlagsHorizontal = SizeFlags.ExpandFill };
        grid.AddThemeConstantOverride("h_separation", UiStyle.RowSpacing);
        grid.AddThemeConstantOverride("v_separation", 4);

        for (int i = 0; i < _skillButtons.Length; i++)
        {
            int index = i;
            var name = Body(SkillName(i));
            name.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            name.TooltipText = SkillTooltip(i);
            grid.AddChild(name);

            _skillLevels[i] = Body("0");
            _skillLevels[i].HorizontalAlignment = HorizontalAlignment.Center;
            _skillLevels[i].CustomMinimumSize = new Vector2(24, 0);
            grid.AddChild(_skillLevels[i]);

            _skillButtons[i] = new Button
            {
                Text = "+",
                CustomMinimumSize = new Vector2(32, 0),
                TooltipText = SkillTooltip(i),
            };
            _skillButtons[i].Pressed += () => UpgradeSkill(index);
            grid.AddChild(_skillButtons[i]);
        }

        return grid;
    }

    private void UpgradeSkill(int index)
    {
        if (_player == null) return;

        switch (index)
        {
            case 0: _player.UpgradeDedication(); break;
            case 1: _player.UpgradeAspiration(); break;
            case 2: _player.UpgradeNegotiation(); break;
            case 3: _player.UpgradeReservation(); break;
        }
    }

    private int SkillLevel(int index) => _player switch
    {
        null => 0,
        _ => index switch
        {
            0 => _player.Dedication,
            1 => _player.Aspiration,
            2 => _player.Negotiation,
            _ => _player.Reservation,
        },
    };

    private static string SkillName(int index) => index switch
    {
        0 => "Dedication",
        1 => "Aspiration",
        2 => "Negotiation",
        _ => "Reservation",
    };

    private static string SkillTooltip(int index) => index switch
    {
        0 => UiStyle.Text.DedicationTooltip,
        1 => UiStyle.Text.AspirationTooltip,
        2 => UiStyle.Text.NegotiationTooltip,
        _ => UiStyle.Text.ReservationTooltip,
    };

    private static Label Header(string text)
    {
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        label.AddThemeFontSizeOverride("font_size", UiStyle.HeaderFontSize);
        label.AddThemeColorOverride("font_color", UiStyle.TextColor);
        return label;
    }

    private static Label Body(string text)
    {
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        label.AddThemeFontSizeOverride("font_size", UiStyle.FontSize);
        label.AddThemeColorOverride("font_color", UiStyle.TextColor);
        return label;
    }

    private static void AddBar(VBoxContainer parent, string title, Color color,
        out ProgressBar bar, out Label value)
    {
        var caption = Body(title);
        parent.AddChild(caption);

        var holder = new Control
        {
            CustomMinimumSize = new Vector2(0, 18),
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        parent.AddChild(holder);

        bar = new ProgressBar { ShowPercentage = false };
        UiStyle.StyleBar(bar, color);
        holder.AddChild(bar);
        bar.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        value = Body("0/0");
        value.HorizontalAlignment = HorizontalAlignment.Center;
        value.VerticalAlignment = VerticalAlignment.Center;
        holder.AddChild(value);
        value.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
    }
}
