using Dwarf.Engine.Textures;
using System.IO;
using ImGuiNET;
using System.Diagnostics;

namespace Dwarf.Engine.ImGuiNET.FileExplorer;

public enum DialogMode {
  /// <summary>
  /// Opens file via external programs
  /// </summary>
  Open,
  /// <summary>
  /// Saves data into specified path
  /// </summary>
  Save,
  /// <summary>
  /// Opens file and returns byte[] data
  /// </summary>
  Select
}

public class FileEntity {
  public string? Name;
  public string? Ext;
  public string? Path;
  public Texture? Icon;
  public bool IsFolder;

  public delegate List<FileEntity> FileAction(string path);
  public FileAction? Action;
}

public class FileExplorer {

  private const string _fileIconName = "fileIcon.png";
  private const string _folderIconName = "folderIcon.png";

  private DialogMode _dialogMode;
  private List<FileEntity> _filesInFolder = new();
  private List<DriveInfo> _drives = new();
  private string _currentPath = "";

  private Texture _backTexture;

  private int currItem;
  private ImGuiInputTextCallback? _textCallback;

  private byte[] _byteResponse = null!;

  public unsafe FileExplorer(DialogMode dialogMode) {
    _dialogMode = dialogMode;
    _backTexture = Texture.FastTextureLoad("Resources/ico/backIcon.png", 0);

    _filesInFolder = GetFilesWithinDirectory(Directory.GetCurrentDirectory());
    _currentPath = Directory.GetCurrentDirectory();

    _drives = GetDrives().ToList();

    //_textCallback = TextCallback;
    _textCallback = new ImGuiInputTextCallback(TextCallback);
  }

  public void Update() {
    // ImGui.GetWindowViewport();

    ImGui.Begin("File Explorer");

    // ImGui.ImageButton((IntPtr)_backTexture.Handle, new System.Numerics.Vector2(64,64));

    if(ImGui.ArrowButton("left_btn", ImGuiDir.Left)) {
      GetParentDir(_currentPath);
    }
    ImGui.SameLine();
    
    ImGui.InputText("", ref _currentPath, 128, ImGuiInputTextFlags.None, _textCallback);
    ImGui.SameLine();
    var buttonSize = ImGui.GetContentRegionAvail();

    switch(_dialogMode) {
      case DialogMode.Open:
        if(ImGui.Button("Open", new System.Numerics.Vector2(buttonSize.X, 0))) {
          HandleOpen();
        }
        break;
      case DialogMode.Save:
        if(ImGui.Button("Save", new System.Numerics.Vector2(buttonSize.X, 0))) {
          HandleSave();
        }
        break;
      case DialogMode.Select:
        if (ImGui.Button("Select", new System.Numerics.Vector2(buttonSize.X, 0))) {
          HandleSelect();
        }
        break;
      default:
        break;
    }

    

    string[] test = {
      "",
      ""
    };


    // Left
    ImGui.BeginChild("left panel", new System.Numerics.Vector2(150, 0), true);


    foreach (var drive in _drives) {
      if(ImGui.Selectable(drive.Name)) {
        _filesInFolder = GetFilesWithinDirectory(drive.RootDirectory.FullName);
      }
    }

    ImGui.EndChild();

    ImGui.SameLine();

    ImGui.BeginGroup();
    ImGui.BeginChild("files");
    float padding = 16.0f;
    float thumbSize = 64;
    float cellSize = thumbSize + padding;
    System.Numerics.Vector2 vector2 = ImGui.GetContentRegionAvail();
    int columnCount = (int)(vector2.X / cellSize);
    if (columnCount < 1) columnCount = 1;

    ImGui.Columns(columnCount, "", false);

    foreach (var file in _filesInFolder) {
      ImGui.ImageButton((IntPtr)file?.Icon?.Handle, new System.Numerics.Vector2(thumbSize, thumbSize));
      if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left)) {
        if (file!.IsFolder) _filesInFolder = file!.Action!.Invoke(file.Path!);
      }
      if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
        _currentPath = file!.Path!;
      }
      ImGui.TextWrapped(file!.Name);
      ImGui.NextColumn();
    }

    ImGui.Columns(1);
    ImGui.EndChild();
    ImGui.SameLine();
    ImGui.EndGroup();

    ImGui.End();
  }

  public DriveInfo[] GetDrives() {
    return DriveInfo.GetDrives();
  }

  public void GetParentDir(string parent) {
    if(parent == null) return;
    var newPath = Directory.GetParent(parent);
    if(newPath == null) return;
    _currentPath = newPath.FullName;
    _filesInFolder = GetFilesWithinDirectory(_currentPath);
  }

  public List<FileEntity> GetFilesWithinDirectory(string directory) {
    List<FileEntity> fileEntities = new List<FileEntity>();

    DirectoryInfo dirInfo = new DirectoryInfo(directory);
    var files = Directory.GetFileSystemEntries(directory);

    using var ms = new MemoryStream();

    foreach (var file in files) {
      var fileEntity = new FileEntity();

      fileEntity.Name = Path.GetFileName(file);
      fileEntity.Path = file;
      fileEntity.Ext = Path.GetExtension(file);
      fileEntity.IsFolder = fileEntity.Ext.Length <= 0;

      var icon = Texture.FastTextureLoad(
        $"Resources/ico/{(fileEntity.IsFolder == true ? _folderIconName : _fileIconName)}",
        0
      );

      fileEntity.Icon = icon;

      if (fileEntity.IsFolder) {
        fileEntity.Action = new FileEntity.FileAction(GetFilesWithinDirectory);
      }

      fileEntities.Add(fileEntity);
    }

    _currentPath = directory;

    return fileEntities;
  }

  public void HandleSave() {
    // implement later on
    // using var fs = new FileStream(_currentPath, FileMode.Create, FileAccess.Write);
  }

  public void HandleOpen() {
    new Process {
      StartInfo = new ProcessStartInfo(_currentPath) {
        UseShellExecute = true
      }
    }.Start();
  }

  public void HandleSelect() {
    _byteResponse = File.ReadAllBytes(_currentPath);
  }

  public unsafe int TextCallback(ImGuiInputTextCallbackData* data) {
    Console.WriteLine("Callback");
    return 0;
  }

  public void OpenDialog(string label, DialogMode mode) {
    _dialogMode = mode;
  }
}
