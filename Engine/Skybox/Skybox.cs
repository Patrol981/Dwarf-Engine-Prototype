using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
//using System.Drawing;
//using System.Drawing.Imaging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Dwarf.Engine.Cameras;
using Dwarf.Engine.ECS;
using Dwarf.Engine.Globals;
using Dwarf.Engine.Shaders;

namespace Dwarf.Engine.Skyboxes;
public class Skybox {
  private int _textureID;
  private Shader _shader;
  private int _vao, _vbo, _ebo;

  Matrix4 _projection;
  Matrix4 _view;

  float _ratio = 0;

  float[] _skyboxVertices = {
        // positions          
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f
    };

  private TextureTarget[] targets = {
    TextureTarget.TextureCubeMapNegativeX, TextureTarget.TextureCubeMapNegativeY,
    TextureTarget.TextureCubeMapNegativeZ, TextureTarget.TextureCubeMapPositiveX,
    TextureTarget.TextureCubeMapPositiveY, TextureTarget.TextureCubeMapPositiveZ
  };

  public Skybox(float ratio) {
    const string BasePath = "G:\\repos\\priv\\OpenTK\\Dwarf Engine";

    _shader = new Shader($"Shaders\\skybox.vert", $"Shaders\\skybox.frag");
    _ratio = ratio;

    

    List<string> faces = new List<string>();
    faces.Add($"Resources/Skyboxes/Sunny/left.jpg");
    faces.Add($"Resources/Skyboxes/Sunny/bottom.jpg");
    
    faces.Add($"Resources/Skyboxes/Sunny/back.jpg");
    faces.Add($"Resources/Skyboxes/Sunny/right.jpg");

    faces.Add($"Resources/Skyboxes/Sunny/top.jpg");
    faces.Add($"Resources/Skyboxes/Sunny/front.jpg");

    _vao = GL.GenVertexArray();
    _vbo = GL.GenBuffer();

    GL.BindVertexArray(_vao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

    GL.BufferData(
      BufferTarget.ArrayBuffer,
      _skyboxVertices.Length * sizeof(float),
      _skyboxVertices,
      BufferUsageHint.StaticDraw
    );

    // var vertexLocation = _shader!.GetAttribLocation("aPos");
    GL.EnableVertexAttribArray(0);
    // GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

    LoadCubemap(faces);

    _shader.Use();
    _shader.SetInt("skybox", 0);
  }

  public void Update(Camera camera) {
    if (CameraGlobalState.GetCameraEntity() == null) return;
    GL.DepthFunc(DepthFunction.Lequal);
    _shader!.Use();
    var ogView = camera.GetViewMatrix();
    var rotY = Matrix4.CreateRotationY(camera.Yaw / camera.Fov * (-1));
    var rotX = Matrix4.CreateRotationX(camera.Pitch / camera.Fov);
    var view = ogView.ClearRotation() * rotX * rotY;
    var ogProj = camera.GetProjectionMatrix();
    var projection = Matrix4.Transpose(ogProj);
    _shader!.SetMatrix4("view", view);
    _shader!.SetMatrix4("projection", projection);

    GL.BindVertexArray(_vao);
    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.TextureCubeMap, _textureID);

    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    GL.BindVertexArray(0);
    GL.DepthFunc(DepthFunction.Less);
  }

  private void LoadCubemap(List<string> faces) {
    _textureID = GL.GenTexture();
    GL.BindTexture(TextureTarget.TextureCubeMap, _textureID);

    for (int i = 0; i < faces.Count; i++) {
      Image<Rgba32> image = Image.Load<Rgba32>(faces[i]);
      // image.Mutate(x => x.Flip(FlipMode.Vertical));

      var pixels = new List<byte>(4 * image.Width * image.Height);

      for (short y = 0; y < image.Height; y++) {
        var row = image.GetPixelRowSpan(y);

        for (short x = 0; x < image.Width; x++) {
          pixels.Add(row[x].R);
          pixels.Add(row[x].G);
          pixels.Add(row[x].B);
          pixels.Add(row[x].A);
        }
      }

      GL.TexImage2D(
        targets[i], // <------
        0,
        PixelInternalFormat.Rgba,
        image.Width,
        image.Height,
        0,
        PixelFormat.Rgba,
        PixelType.UnsignedByte,
        pixels.ToArray()
      );
    }

    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);



    // return 0;
  }
}
