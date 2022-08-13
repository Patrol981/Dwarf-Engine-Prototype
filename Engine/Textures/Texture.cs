using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using SkiaSharp;

using StbImageSharp;

namespace Dwarf.Engine.Textures;

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

  public static Texture FastTextureLoad(string path) {
    int handle = GL.GenTexture();

    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, handle);

    StbImage.stbi_set_flip_vertically_on_load(1);

    using (Stream stream = File.OpenRead(path)) {
      ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

    return new Texture(handle);
  }

  public static Texture LoadFromFile(string path) {
    int handle = GL.GenTexture();

    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, handle);

    Image<Rgba32> image = Image.Load<Rgba32>(path);
    image.Mutate(x => x.Flip(FlipMode.Vertical));

    var pixels = new List<byte>(4 * image.Width * image.Height);

    for(short y=0; y<image.Height; y++) {
      var row = image.GetPixelRowSpan(y);

      for(short x=0; x<image.Width; x++) {
        pixels.Add(row[x].R);
        pixels.Add(row[x].G);
        pixels.Add(row[x].B);
        pixels.Add(row[x].A);
      }
    }

    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
      PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

    return new Texture(handle);
  }

  public void Use(TextureUnit unit) {
    GL.ActiveTexture(unit);
    GL.BindTexture(TextureTarget.Texture2D, Handle);
  }
}