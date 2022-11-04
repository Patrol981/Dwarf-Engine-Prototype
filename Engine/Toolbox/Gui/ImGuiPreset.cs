using Dwarf.Engine.Toolbox.Gui.Interfaces;
using ImGuiNET;
using System.Numerics;

namespace Dwarf.Engine.Toolbox.Gui {
  public class ImGuiPreset : IImGuiPreset {

    public Vector4 PrimaryColor { get; set; }
    public Vector4 SecondaryColor { get; set; }

    public Vector4 BackgroundColor { get; set; } 
    public Vector4 BorderColor { get; set; }
    public Vector4 FontColor { get; set; }
    public Vector4 DisabledColor { get; set; }
    public Vector4 HoverColor { get; set; }
    public Vector4 ClickColor { get; set; }

    public void Update() {
      PrimaryColor = new Vector4(1f, 1f, 1f, 1f);
      SecondaryColor = new Vector4(0.11f, 0.11f, 0.11f, 1f);
      BackgroundColor = new Vector4(1f, 1f, 1f, 0.9f);
      FontColor = new Vector4(0f, 0f, 0f, 1f);


      BorderColor = new Vector4(1f, 1f, 1f, 1f);
      DisabledColor = new Vector4(1f, 1f, 1f, 1f);
      HoverColor = new Vector4(0.2f, 0.2f, 0.2f, 1f);
      ClickColor = new Vector4(0.1f, 0.1f, 0.1f, 1f);

      var style = ImGui.GetStyle();
      style.Colors[(int)ImGuiCol.WindowBg] = BackgroundColor;
      style.Colors[(int)ImGuiCol.MenuBarBg] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TitleBgActive] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TitleBgCollapsed] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TitleBg] = PrimaryColor;

      style.Colors[(int)ImGuiCol.Text] = SecondaryColor;
      style.Colors[(int)ImGuiCol.TextSelectedBg] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TextDisabled] = SecondaryColor;

      style.Colors[(int)ImGuiCol.ButtonHovered] = (SecondaryColor - PrimaryColor) * (-1);
      style.Colors[(int)ImGuiCol.ButtonActive] = PrimaryColor;
      style.Colors[(int)ImGuiCol.Button] = new Vector4(1,1,1,0.1f);

      style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.2f, 0.2f, 0.2f, 1f);

      style.Colors[(int)ImGuiCol.Tab] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TabActive] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TabHovered] = PrimaryColor;

      style.Colors[(int)ImGuiCol.TextSelectedBg] = PrimaryColor;
      style.Colors[(int)ImGuiCol.ChildBg] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TableRowBg] = PrimaryColor;
      style.Colors[(int)ImGuiCol.TabActive] = PrimaryColor;

      //style.Colors[(int)ImGuiCol.Tab] = PrimaryColor;
      //style.Colors[(int)ImGuiCol.Tab] = PrimaryColor;
      // style.Colors[(int)ImGuiCol.FrameBg] = BorderColor;
      // style.Colors[(int)ImGuiCol.TableHeaderBg] = BorderColor;
    }
  }
}
