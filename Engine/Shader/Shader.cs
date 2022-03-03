
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
  private Dictionary<string, int> _uniformLocations;
  private bool _disposed = false;

  ~Shader() {
    GL.DeleteProgram(_handle);
  }

  public Shader(string vertexPath, string fragmentPath) {
    var vertexShaderSource = File.ReadAllText(vertexPath);
    var fragmentShaderSource = File.ReadAllText(fragmentPath);

    var vertexShader = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vertexShader, vertexShaderSource);
    CompileShader(vertexShader);

    var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fragmentShader, fragmentShaderSource);
    CompileShader(fragmentShader);

    _handle = GL.CreateProgram();

    GL.AttachShader(_handle, vertexShader);
    GL.AttachShader(_handle, fragmentShader);

    GL.LinkProgram(_handle);

    // no need to store shaders, because they are linked
    GL.DetachShader(_handle, vertexShader);
    GL.DetachShader(_handle, fragmentShader);
    GL.DeleteShader(vertexShader);
    GL.DeleteShader(fragmentShader);

    /*
    GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var uniformsNumber);
    _uniformLocations = new Dictionary<string, int>();

    for(var i=0; i<uniformsNumber; i++) {
      var key = GL.GetActiveUniform(_handle, i, out _, out _);
      var location = GL.GetUniformLocation(_handle, key);
      _uniformLocations.Add(key, location);
    }
    */
  }

  public void Use() {
    GL.UseProgram(_handle);
  }

  private static void CompileShader(int shader) {
    GL.CompileShader(shader);

    GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
    if(code != (int)All.True) {
      var infoLog = GL.GetShaderInfoLog(shader);
      throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
    }
  }

  private static void LinkProgram(int program) {
    GL.LinkProgram(program);

    GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
    if (code != (int)All.True) {
      throw new Exception($"Error occurred whilst linking Program({program})");
    }
  }

  public int GetAttribLocation(string attribName) {
    return GL.GetAttribLocation(_handle, attribName);
  }

  /// <summary>
  /// Set a uniform int on this shader.
  /// </summary>
  /// <param name="name">The name of the uniform</param>
  /// <param name="data">The data to set</param>
  public void SetInt(string name, int data) {
    GL.UseProgram(_handle);
    // GL.Uniform1(_uniformLocations[name], data);
    GL.Uniform1(GL.GetUniformLocation(_handle, name), data);
  }

  /// <summary>
  /// Set a uniform float on this shader.
  /// </summary>
  /// <param name="name">The name of the uniform</param>
  /// <param name="data">The data to set</param>
  public void SetFloat(string name, float data) {
    GL.UseProgram(_handle);
    // GL.Uniform1(_uniformLocations[name], data);
    GL.Uniform1(GL.GetUniformLocation(_handle, name), data);
  }

  /// <summary>
  /// Set a uniform Matrix4 on this shader
  /// </summary>
  /// <param name="name">The name of the uniform</param>
  /// <param name="data">The data to set</param>
  /// <remarks>
  ///   <para>
  ///   The matrix is transposed before being sent to the shader.
  ///   </para>
  /// </remarks>
  public void SetMatrix4(string name, Matrix4 data) {
    GL.UseProgram(_handle);
    // GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    GL.UniformMatrix4(GL.GetUniformLocation(_handle, name), true, ref data);
  }

  /// <summary>
  /// Set a uniform Vector3 on this shader.
  /// </summary>
  /// <param name="name">The name of the uniform</param>
  /// <param name="data">The data to set</param>
  public void SetVector3(string name, Vector3 data) {
    GL.UseProgram(_handle);
    GL.Uniform3(GL.GetUniformLocation(_handle, name), data);
    // GL.Uniform3(_uniformLocations[name], data);
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