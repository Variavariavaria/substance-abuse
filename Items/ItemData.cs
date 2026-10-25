#nullable enable
using Godot;

namespace SubstanceAbuse.Items;

/// <summary>Определение внутриигрового предмета. Создаётся как .tres в редакторе.</summary>
[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string ItemName { get; set; } = "";

    /// <summary>Уникальный стабильный идентификатор (для сохранений и рецептов).</summary>
    [Export] public string ItemID { get; set; } = "";

    /// <summary>Редкость: целое число >= 0, чем больше — тем реже.</summary>
    [Export(PropertyHint.Range, "0,10,1,or_greater")] public int Rarity { get; set; } = 0;

    [Export] public Texture2D? Texture { get; set; }
}
