using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using Voxelized.Textures;
using Voxelized.Shaders;
using Voxelized.Globals;

using Voxelized.Cameras;
using Voxelized.ECS;

namespace Voxelized.Challanges;

public class CubesTest : Component {
  private readonly Shader _shader;
  private readonly Texture _texture, _texture2;

  private readonly float[] _vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

  private readonly uint[] _indices = {
    0, 1, 3,
    1, 2, 3
  };

  private readonly Vector3[] _cubePositions = {
    new Vector3(0, 0, 0),
    new Vector3(1, 0, 0),
    new Vector3(2, 0, 0),
    // new Vector3(3, 0, 0),
    new Vector3(4, 0, 0),

    // new Vector3(0, 1, 0),
    // new Vector3(1, 1, 0),
    new Vector3(2, 1, 0),
    // new Vector3(3, 1, 0),
    new Vector3(4, 1, 0),

    new Vector3(0, 2, 0),
    new Vector3(1, 2, 0),
    new Vector3(2, 2, 0),
    new Vector3(3, 2, 0),
    new Vector3(4, 2, 0),

    new Vector3(0, 3, 0),
    // new Vector3(1, 3, 0),
    new Vector3(2, 3, 0),
    // new Vector3(3, 3, 0),
    // new Vector3(4, 3, 0),

    new Vector3(0, 4, 0),
    // new Vector3(1, 4, 0),
    new Vector3(2, 4, 0),
    new Vector3(3, 4, 0),
    new Vector3(4, 4, 0)
  };

  private int _vbo, _vao, _ebo;

  private float _rotation;
  private bool _textureSwitch = false;

  public CubesTest() {}

  public CubesTest(Shader shader) {
    _shader = shader;

    _vao = GL.GenVertexArray();
    GL.BindVertexArray(_vao);

    _vbo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

    _ebo = GL.GenBuffer();
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

    _shader.Use();

    var vertexLocation = _shader.GetAttribLocation("aCubePosition");
    GL.EnableVertexAttribArray(vertexLocation);
    GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

    var texCoordLocation = _shader.GetAttribLocation("aCubeTexCoord");
    GL.EnableVertexAttribArray(texCoordLocation);
    GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

    _texture = Texture.LoadFromFile("Resources/grass.png");
    _texture.Use(TextureUnit.Texture0);

    _texture2 = Texture.LoadFromFile("Resources/water.png");
    _texture2.Use(TextureUnit.Texture1);
  }

  public void Update() {
    _shader.Use();

    _rotation += 4.0f * (float)WindowGlobalState.GetTime();
    _shader!.SetMatrix4("view", CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>().GetViewMatrix());
    _shader!.SetMatrix4("projection", CameraGlobalState.GetCameraEntity().GetComponent<FreeCamera>().GetProjectionMatrix());

    for(uint i=0; i<_cubePositions.Length; i++) {
      /*
      if(_textureSwitch) {
        _shader.SetInt("texture0", 0);
        _texture.Use(TextureUnit.Texture0);
        _textureSwitch = false;
      } else {
        _shader.SetInt("texture1", 1);
        _texture2.Use(TextureUnit.Texture1);
        _textureSwitch = true;
      }
      */

      // var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_rotation));
      var model = Matrix4.CreateTranslation(_cubePositions[i].X, _cubePositions[i].Y, _cubePositions[i].Z);
      
      // model *= Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_rotation));
      model *= Matrix4.Identity * Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(_rotation));
      _shader!.SetMatrix4("model", model);
      GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }
  }
}