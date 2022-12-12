using System.IO;

using Dwarf.Engine.ECS;
using Dwarf.Engine.Textures;

using OpenTK.Mathematics;

using StbImageSharp;

namespace Dwarf.Engine.DataStructures;
public class Sprite : Component {
  private float[] _vertices = {
    0.5f,  0.5f, 0.0f,  1.0f, 1.0f, // top right
    0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f,  0.0f, 1.0f  // top left 
  };

  private int[] _indices = {
    0, 1, 3, // first triangle
    1, 2, 3  // second triangle
  };

  private Vector2 _size = Vector2.Zero;

  private Texture _texture;

  public Sprite() { }

  public Sprite(string texturePath) {
    _texture = Texture.FastTextureLoad(texturePath);
    SetupProportions(texturePath);
  }

  public void LoadTexture(string texturePath) {
    _texture = Texture.FastTextureLoad(texturePath);
  }

  private void SetupProportions(string texturePath) {
    using var stream = File.OpenRead($"{texturePath}");
    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

    _size = new Vector2(image.Width, image.Height);

    float[] tvertices = {
    0.5f,  0.5f, 0.0f,  1.0f, 1.0f, // top right
    0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f,  0.0f, 1.0f  // top left 
  };

    stream.Dispose();
  }

  public Texture Texture {
    get { return _texture; }
  }
  public float[] Vertices {
    get { return _vertices; }
  }

  public int[] Indices {
    get { return _indices; }
  }

  public Vector2 Size {
    get { return _size; }
  }
}
