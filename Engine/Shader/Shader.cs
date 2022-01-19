
using System.Text;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;


namespace Voxelized.Shaders;

public class Shader {
  private protected int _handle;
  private bool _disposed = false;

  public Shader(string vertexPath, string fragmentPath) {
    LoadShaders(vertexPath,fragmentPath);
  }

  ~Shader() {
    GL.DeleteProgram(_handle);
  }

  public void Use() {
    GL.UseProgram(_handle);
  }

  private void LoadShaders(string vertexPath, string fragmentPath) {
    string vertexShaderSource;
    string fragmentShaderSource;

    using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8)) {
      vertexShaderSource = reader.ReadToEnd();
    }

    using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8)) {
      fragmentShaderSource = reader.ReadToEnd();
    }

    int vertexShader = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vertexShader, vertexShaderSource);

    int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fragmentShader, fragmentShaderSource);

    string infoLogVertex = System.String.Empty;
    string infoLogFragment = System.String.Empty;

    GL.CompileShader(vertexShader);
    GL.GetShaderInfoLog(vertexShader, out infoLogVertex);
    if(infoLogVertex != System.String.Empty) {
      System.Console.Write(infoLogVertex);
    }

    GL.CompileShader(fragmentShader);
    GL.GetShaderInfoLog(fragmentShader, out infoLogFragment);
    if(infoLogFragment != System.String.Empty) {
      System.Console.Write(infoLogFragment);
    }

    _handle = GL.CreateProgram();

    GL.AttachShader(_handle, vertexShader);
    GL.AttachShader(_handle, fragmentShader);

    GL.LinkProgram(_handle);

    // no need to store shaders, because they are linked
    GL.DetachShader(_handle, vertexShader);
    GL.DetachShader(_handle, fragmentShader);
    GL.DeleteShader(vertexShader);
    GL.DeleteShader(fragmentShader);
  }

  protected virtual void Dispose(bool disposing) {
    if(!_disposed) {
      GL.DeleteProgram(_handle);
      _disposed = true;
    }
  }

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}