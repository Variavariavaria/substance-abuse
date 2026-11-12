#nullable enable
using Godot;

namespace SubstanceAbuse.Items;

/// <summary>Временное минимальное описание оружия. Боевую систему определит её автор.</summary>
[GlobalClass]
public partial class WeaponData : ItemData
{
    [Export] public int MinDamage { get; set; } = 1;
    [Export] public int MaxDamage { get; set; } = 1;
}
