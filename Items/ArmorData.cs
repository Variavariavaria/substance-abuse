#nullable enable
using Godot;

namespace SubstanceAbuse.Items;

/// <summary>Временное минимальное описание брони. Боевую систему определит её автор.</summary>
[GlobalClass]
public partial class ArmorData : ItemData
{
    [Export] public int DefenceMod { get; set; } = 0;
}
