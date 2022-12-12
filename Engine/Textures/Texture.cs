using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Dwarf.Engine.Globals;

using OpenTK.Graphics.OpenGL4;

using StbImageSharp;
namespace Dwarf.Engine.Textures;

public struct Pixel {
  public int R;
  public int G;
  public int B;
  public int A;
}

public struct TextureStruct {
  public int Id;
  public string Type;
  public string Path;
}

public class Texture {
  public readonly int Handle;

  public Texture(int glHandle) {
    Handle = glHandle;
  }

  public static Texture FastTextureLoad(string path, int flip = 1) {
    int handle = GL.GenTexture();
    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, handle);

    StbImage.stbi_set_flip_vertically_on_load(flip);

    using var stream = File.OpenRead($"{path}");
    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    stream.Dispose();

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    GL.Disable(EnableCap.Blend);

    return new Texture(handle);
  }

  public void Use(TextureUnit unit) {
    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    GL.ActiveTexture(unit);
    GL.BindTexture(TextureTarget.Texture2D, Handle);
    GL.Disable(EnableCap.Blend);
  }
}