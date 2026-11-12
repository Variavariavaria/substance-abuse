#nullable enable
using Godot;
using SubstanceAbuse.Characters;

namespace SubstanceAbuse.UI;

/// <summary>Вкладка «Записки»: страница игрока и найденные нередактируемые записки.</summary>
public partial class NotebookPage : Control
{
    private PlayerData? _player;
    private bool _built;
    private bool _updatingText;

    private int _pageIndex;
    private Button _previous = null!;
    private Button _next = null!;
    private Label _counter = null!;
    private TextEdit _textEdit = null!;
    private ScrollContainer _noteScroll = null!;
    private Label _noteTitle = null!;
    private Label _noteBody = null!;

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

        var nav = new HBoxContainer();
        nav.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        root.AddChild(nav);

        _previous = new Button { Text = UiStyle.Text.PreviousPage };
        _previous.Pressed += () => ShowPage(_pageIndex - 1);
        nav.AddChild(_previous);

        _counter = UiStyle.Body("1/1");
        _counter.HorizontalAlignment = HorizontalAlignment.Center;
        _counter.VerticalAlignment = VerticalAlignment.Center;
        _counter.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        nav.AddChild(_counter);

        _next = new Button { Text = UiStyle.Text.NextPage };
        _next.Pressed += () => ShowPage(_pageIndex + 1);
        nav.AddChild(_next);

        var pageHolder = new Control
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        root.AddChild(pageHolder);

        _textEdit = BuildPlayerPage(pageHolder);
        _noteScroll = BuildNotePage(pageHolder);

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
        _player.Notebook.NotesChanged += OnNotesChanged;
        Refresh();
    }

    public void Refresh()
    {
        if (!_built) return;

        int max = _player?.Notebook.PageCount ?? 1;
        _pageIndex = Mathf.Clamp(_pageIndex, 0, max - 1);
        ShowPage(_pageIndex);
    }

    /// <summary>Снимает фокус с редактора при уходе с вкладки.</summary>
    public void ReleaseInputFocus()
    {
        if (_textEdit.HasFocus()) _textEdit.ReleaseFocus();
    }

    public override void _ExitTree() => Unsubscribe();

    private void Unsubscribe()
    {
        if (_player == null) return;
        _player.Notebook.NotesChanged -= OnNotesChanged;
        _player = null;
    }

    private TextEdit BuildPlayerPage(Control parent)
    {
        var edit = new TextEdit
        {
            PlaceholderText = UiStyle.Text.NotebookPlaceholder,
            WrapMode = TextEdit.LineWrappingMode.Boundary,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        edit.AddThemeFontSizeOverride("font_size", UiStyle.FontSize);
        edit.TextChanged += OnTextChanged;
        parent.AddChild(edit);
        edit.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        return edit;
    }

    private ScrollContainer BuildNotePage(Control parent)
    {
        var scroll = new ScrollContainer
        {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            Visible = false,
        };
        parent.AddChild(scroll);
        scroll.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var content = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        content.AddThemeConstantOverride("separation", UiStyle.RowSpacing);
        scroll.AddChild(content);

        _noteTitle = UiStyle.Header("");
        _noteTitle.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _noteTitle.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        content.AddChild(_noteTitle);

        _noteBody = UiStyle.Body("");
        _noteBody.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _noteBody.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        content.AddChild(_noteBody);

        return scroll;
    }

    private void ShowPage(int index)
    {
        int max = _player?.Notebook.PageCount ?? 1;
        _pageIndex = Mathf.Clamp(index, 0, max - 1);
        UpdateNavigation(max);

        bool playerPage = _pageIndex == 0;
        _textEdit.Visible = playerPage;
        _noteScroll.Visible = !playerPage;

        if (playerPage)
            LoadPlayerText();
        else
            RenderNotePage();
    }

    private void LoadPlayerText()
    {
        if (_player == null) return;

        _updatingText = true;
        _textEdit.Text = _player.Notebook.PlayerText;
        _updatingText = false;
    }

    private void RenderNotePage()
    {
        if (_player == null || _pageIndex <= 0) return;

        var notes = _player.Notebook.FoundNotes;
        int noteIndex = _pageIndex - 1;
        if (noteIndex < 0 || noteIndex >= notes.Count) return;

        _noteTitle.Text = notes[noteIndex].Title;
        _noteBody.Text = notes[noteIndex].Text;
    }

    private void UpdateNavigation(int max)
    {
        _counter.Text = UiStyle.Text.NoteCounter(_pageIndex + 1, max);
        _previous.Disabled = _pageIndex <= 0;
        _next.Disabled = _pageIndex >= max - 1;
    }

    private void OnTextChanged()
    {
        if (_updatingText || _player == null || _pageIndex != 0) return;
        _player.Notebook.SetPlayerText(_textEdit.Text);
    }

    private void OnNotesChanged()
    {
        if (_player == null) return;

        int max = _player.Notebook.PageCount;
        if (_pageIndex >= max) _pageIndex = max - 1;

        UpdateNavigation(max);
        if (_pageIndex > 0) RenderNotePage();
    }
}
