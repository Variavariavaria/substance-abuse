#nullable enable
using Godot;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.UI;

/// <summary>Цвета, размеры и русские строки интерфейса в одном месте.</summary>
public static class UiStyle
{
    public const int PanelPadding = 8;
    public const int RowSpacing = 6;
    public const int IconSize = 24;
    public const int AvatarSize = 96;
    public const int FontSize = 16;
    public const int HeaderFontSize = 20;
    public const int MaxRarityColorIndex = 4;

    public static readonly Color PanelBackground = Color.FromHtml("#000000");
    public static readonly Color WorldBackground = Color.FromHtml("#555555");
    public static readonly Color TextColor = Color.FromHtml("#d8d8d8");
    public static readonly Color MutedText = Color.FromHtml("#7a7a7a");
    public static readonly Color Accent = Color.FromHtml("#e0b040");
    public static readonly Color HealthColor = Color.FromHtml("#b02020");
    public static readonly Color StaminaColor = Color.FromHtml("#9aa820");
    public static readonly Color SanityColor = Color.FromHtml("#5a4fc0");
    public static readonly Color BarBackground = Color.FromHtml("#202020");
    public static readonly Color ActiveTabBackground = Color.FromHtml("#2a2a2a");
    public static readonly Color InactiveTabBackground = Color.FromHtml("#101010");
    public static readonly Color ActiveTabText = Color.FromHtml("#ffffff");
    public static readonly Color InactiveTabText = Color.FromHtml("#9a9a9a");
    public static readonly Color SlotBackground = Color.FromHtml("#101010");
    public static readonly Color AvatarBackground = Color.FromHtml("#1a1a1a");
    public static readonly Color IconPlaceholder = Color.FromHtml("#303030");

    /// <summary>Цвет названия предмета по редкости (индекс зажимается в 0..4).</summary>
    public static Color RarityColor(int rarity) => Mathf.Clamp(rarity, 0, MaxRarityColorIndex) switch
    {
        0 => Color.FromHtml("#b0b0b0"),
        1 => Color.FromHtml("#5fbf5f"),
        2 => Color.FromHtml("#4f8fff"),
        3 => Color.FromHtml("#b060ff"),
        _ => Color.FromHtml("#ff8a30"),
    };

    public static StyleBoxFlat Box(Color background, int margin = 0) => new()
    {
        BgColor = background,
        ContentMarginLeft = margin,
        ContentMarginRight = margin,
        ContentMarginTop = margin,
        ContentMarginBottom = margin,
    };

    public static StyleBoxFlat TabStyle(Color background) => Box(background, 4);

    public static Label Header(string text)
    {
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        label.AddThemeFontSizeOverride("font_size", HeaderFontSize);
        label.AddThemeColorOverride("font_color", TextColor);
        return label;
    }

    public static Label Body(string text)
    {
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        label.AddThemeFontSizeOverride("font_size", FontSize);
        label.AddThemeColorOverride("font_color", TextColor);
        return label;
    }

    public static void StyleBar(ProgressBar bar, Color fill)
    {
        bar.AddThemeStyleboxOverride("background", Box(BarBackground));
        bar.AddThemeStyleboxOverride("fill", Box(fill));
    }

    /// <summary>Иконка предмета 24×24: текстура или цветной квадрат-заглушка.</summary>
    public static Control CreateItemIcon(ItemData? item, int size = IconSize)
    {
        Control holder = new Control { CustomMinimumSize = new Vector2(size, size) };

        if (item?.Texture != null)
        {
            var texture = new TextureRect
            {
                Texture = item.Texture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            };
            holder.AddChild(texture);
            texture.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        }
        else
        {
            var placeholder = new ColorRect { Color = IconPlaceholder };
            holder.AddChild(placeholder);
            placeholder.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        }

        return holder;
    }

    public static class Text
    {
        public const string Character = "Персонаж";
        public const string Inventory = "Инвентарь";
        public const string Notes = "Записки";

        public const string Health = "Здоровье";
        public const string Stamina = "Стамина";
        public const string Sanity = "Менталка";

        public const string Parameters = "Параметры";
        public const string Equipment = "Экипировка";
        public const string EquippedWeapon = "Экипированное оружие:";
        public const string EquippedArmor = "Экипированная броня:";
        public const string Effects = "Эффекты";
        public const string NoEffects = "Нет эффектов";
        public const string Avatar = "аватар";
        public const string None = "—";

        public const string Equip = "Экипировать";
        public const string Unequip = "Снять";
        public const string Empty = "Пусто";
        public const string NotebookPlaceholder = "Запишите что-нибудь…";
        public const string PreviousPage = "<-";
        public const string NextPage = "->";

        public const string DedicationTooltip = "Dedication / Самоотдача: +0.15 к модификатору урона";
        public const string AspirationTooltip = "Aspiration / Стремление: +1 к максимуму стамины, восстанавливает 1–4 стамины";
        public const string NegotiationTooltip = "Negotiation / Преодоление: +4 к максимуму здоровья, восстанавливает 4–10 здоровья";
        public const string ReservationTooltip = "Reservation / Спокойствие: +1 к максимуму менталки, восстанавливает 1–2";

        public static string LevelValue(int level) => $"Уровень: {level}";
        public static string ExperienceValue(int xp, int next) => $"Опыт: {xp} / {next}";
        public static string UpgradePointsValue(int points) => $"Очки улучшений: {points}";
        public static string DamageModValue(double mod) => $"Модификатор урона: ×{mod:0.00}";
        public static string ItemsCount(int count) => $"Предметов: {count}";
        public static string Fraction(int current, int max) => $"{current}/{max}";
        public static string NoteCounter(int current, int total) => $"{current}/{total}";
        public static string WeaponDamage(WeaponData weapon) => $"урон {weapon.MinDamage}–{weapon.MaxDamage}";
        public static string ArmorDefence(ArmorData armor) => $"защита {armor.DefenceMod}";
    }
}
