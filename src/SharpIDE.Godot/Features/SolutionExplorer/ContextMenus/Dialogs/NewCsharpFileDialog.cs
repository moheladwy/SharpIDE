using Godot;
using SharpIDE.Application.Features.FileWatching;
using SharpIDE.Application.Features.SolutionDiscovery;

namespace SharpIDE.Godot.Features.SolutionExplorer.ContextMenus.Dialogs;

public partial class NewCsharpFileDialog : ConfirmationDialog
{
    private LineEdit _nameLineEdit = null!;
    private ItemList _fileTypeItemList = null!;

    public SharpIdeFolder ParentFolder { get; set; } = null!;

    [Inject] private readonly IdeFileOperationsService _ideFileOperationsService = null!;
    
    private Texture2D _classIcon = GD.Load<Texture2D>("uid://do0edciarrnp0");

    public override void _Ready()
    {
        _nameLineEdit = GetNode<LineEdit>("%CSharpFileNameLineEdit");
        _nameLineEdit.GrabFocus();
        _nameLineEdit.SelectAll();
        _fileTypeItemList = GetNode<ItemList>("%FileTypeItemList");
        _fileTypeItemList.AddItem("Class", _classIcon);
        _fileTypeItemList.AddItem("Interface", _classIcon);
        _fileTypeItemList.AddItem("Record", _classIcon);
        _fileTypeItemList.AddItem("Struct", _classIcon);
        _fileTypeItemList.AddItem("Enum", _classIcon);
        _fileTypeItemList.ItemSelected += FileTypeItemListOnItemSelected;
        Confirmed += OnConfirmed;
    }

    private void FileTypeItemListOnItemSelected(long index)
    {
        GD.Print("Selected file type index: " + index);
    }

    private void OnConfirmed()
    {
        var fileName = _nameLineEdit.Text.Trim();
        if (string.IsNullOrEmpty(fileName))
        {
            GD.PrintErr("File name cannot be empty.");
            return;
        }

        if (!fileName.EndsWith(".cs"))
        {
            fileName += ".cs";
        }

        _ = Task.GodotRun(async () =>
        {
           //await _ideFileOperationsService.CreateCSharpFile(ParentFolder, fileName);
        });
    }
}