using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using WindowTemplate;

namespace Engine.Common
{
    public class Texture
    {
        public readonly int Handle;

        public static Texture LoadFromFile(string path, out Vector2 ImageSize)
        {
            // Replace path if it's invalid or unsupported file format
            if (!File.Exists(path))
            {
                Console.WriteLine("Path doesn't exist: " + path);
                path = $"{HostWindow.base_directory}Resources/Images/checker.png";
            }

            int handle = GL.GenTexture();

            // GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                ImageSize = new Vector2(image.Width, image.Height);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return new Texture(handle);
        }

        public Texture(int glHandle)
        {
            Handle = glHandle;
        }

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}