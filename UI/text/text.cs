using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

using FreeTypeSharp;
using FreeTypeSharp.Native;

using static Config;
using static Engine.AppWindow;
using static WindowTemplate.HostWindow;

namespace UI
{
    public static class TextManager
    {
        public static Dictionary<char, Character> characters = new Dictionary<char, Character>();
        private static int VAO, VBO;
        
        public static unsafe void Initialize()
        {
            // Init Library
            FT_Error error = FT.FT_Init_FreeType(out IntPtr library);
            ErrorCheck(error);

            FreeTypeLibrary lib = new FreeTypeLibrary();

            FT.FT_Library_Version(library, out int major, out int minor, out int patch);
            Console.WriteLine($"Freetype Version: {major}.{minor}.{patch}");

            // Create Face Object
            error = FT.FT_New_Face(library, $"{base_directory}Resources/Fonts/VictorMono-SemiBold.ttf", 0, out IntPtr face);
            ErrorCheck(error);

            FreeTypeFaceFacade face_facade = new FreeTypeFaceFacade(lib, face);;

            // Set Pixel Size
            FT.FT_Set_Pixel_Sizes(face, 0, font_pixel_size);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (char c = (char)0; c < 128; c++)
            {
                error = FT.FT_Load_Char(face, c, FT.FT_LOAD_RENDER);
                if (error != FT_Error.FT_Err_Ok) Console.WriteLine("A Freetype Error Occured, failed to load glyph: " + error.ToString());

                // Enable SDF rendering
                FT.FT_Render_Glyph((nint)face_facade.GlyphSlot, FT_Render_Mode.FT_RENDER_MODE_SDF);
                
                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.R8,
                    (int)face_facade.GlyphBitmap.width,
                    (int)face_facade.GlyphBitmap.rows,
                    0,
                    PixelFormat.Red,
                    PixelType.UnsignedByte,
                    face_facade.GlyphBitmap.buffer
                );

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                Character character = new Character(
                    texture,
                    new Vector2(face_facade.GlyphBitmap.width, face_facade.GlyphBitmap.rows),
                    new Vector2(face_facade.GlyphBitmapLeft, face_facade.GlyphBitmapTop),
                    face_facade.GlyphMetricHorizontalAdvance
                );

                characters.Add(c, character);
            }

            FT.FT_Done_Face(face);
            FT.FT_Done_FreeType(library);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 4 * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public static void Render(string text, float x, float y, float scale, Vector3 color, Vector4 bgColor)
        {
            GL.DepthMask(false);

            text_s.SetVector3("textColor", color);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);

            foreach (char c in text)
            {
                Character ch = characters[c];

                float xpos = x + ch.bearing.X * scale;
                float ypos = y - (ch.size.Y - ch.bearing.Y) * scale;

                float w = ch.size.X * scale;
                float h = ch.size.Y * scale;

                float[] vertices = new float[]
                {
                    xpos,     ypos + h, 0.0f, 0.0f,
                    xpos,     ypos,     0.0f, 1.0f,
                    xpos + w, ypos,     1.0f, 1.0f,
                    xpos + w, ypos + h, 1.0f, 0.0f
                };

                GL.BindTexture(TextureTarget.Texture2D, ch.texture_id);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float), vertices);

                GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
                x += ch.advance * scale;
            }

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.DepthMask(true);
        }

        public static void Render(string text, float x, float y, float scale, Vector3 color)
        {
            Render(text, x, y, scale, color, new Vector4(0.0f));
        }

        public static void Resize()
        {
            text_s.Use();
            text_s.SetMatrix4("projection", Matrix4.CreateOrthographicOffCenter(0.0f, window_size.X, 0.0f, window_size.Y, 0.0f, 1000.0f));
        }

        private static void ErrorCheck(FT_Error error)
        {
            if (error != FT_Error.FT_Err_Ok) Console.WriteLine("A Freetype Error Occured: " + error.ToString());
        }
    }
}