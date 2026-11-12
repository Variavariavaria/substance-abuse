#nullable enable
using System;
using System.Collections.Generic;
using SubstanceAbuse.Common;

namespace SubstanceAbuse.Characters;

/// <summary>Записная книжка игрока: редактируемая первая страница и найденные записки.</summary>
public class NotebookData
{
    private readonly List<NoteData> _found = new();

    /// <summary>Текст первой (редактируемой) страницы игрока.</summary>
    public string PlayerText { get; private set; } = "";

    public IReadOnlyList<NoteData> FoundNotes => _found;

    /// <summary>Всего страниц: 1 (страница игрока) + найденные записки.</summary>
    public int PageCount => 1 + _found.Count;

    /// <summary>Вызывается только при добавлении найденной записки.</summary>
    public event Action? NotesChanged;

    /// <summary>Сохраняет текст страницы игрока. Событие намеренно не вызывается, чтобы не сбивать курсор.</summary>
    public void SetPlayerText(string? text)
    {
        try
        {
            PlayerText = text ?? "";
            GameLog.Info($"Страница игрока обновлена ({PlayerText.Length} симв.)");
        }
        catch (Exception ex)
        {
            GameLog.Error(ex);
        }
    }

    /// <summary>Добавляет записку. Дубликаты по NoteId игнорируются.</summary>
    public bool AddNote(NoteData? note)
    {
        try
        {
            if (note == null)
            {
                GameLog.Warn("Попытка добавить null-записку");
                return false;
            }

            if (string.IsNullOrWhiteSpace(note.NoteId))
            {
                GameLog.Warn("У записки пустой NoteId");
                return false;
            }

            foreach (NoteData existing in _found)
            {
                if (existing.NoteId != note.NoteId) continue;
                GameLog.Warn($"Записка '{note.NoteId}' уже есть в книжке");
                return false;
            }

            _found.Add(note);
            GameLog.Info($"Найдена записка: {note.Title} [{note.NoteId}]");
            NotesChanged?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            GameLog.Error(ex);
            return false;
        }
    }
}
