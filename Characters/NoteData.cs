#nullable enable
using Godot;

namespace SubstanceAbuse.Characters;

/// <summary>Ресурс найденной записки. Создаётся в редакторе и кладётся в контейнеры мира.</summary>
[GlobalClass]
public partial class NoteData : Resource
{
    [Export] public string NoteId { get; set; } = "";
    [Export] public string Title { get; set; } = "";
    [Export(PropertyHint.MultilineText)] public string Text { get; set; } = "";
}
