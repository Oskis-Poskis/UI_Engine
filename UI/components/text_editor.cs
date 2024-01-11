using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Config;
using static Engine.AppWindow;
using WindowTemplate;

namespace UI
{
    public class TextEditor : FrameComponent
    {
        private float[] bg_vertices;
        private float zoom = 0.66f;
        private int line_count = 32;

        public TextEditor()
        {
            type = ComponentType.TextEditor;

            component_vao = GL.GenVertexArray();
            GL.BindVertexArray(component_vao);

            component_vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, component_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        }

        float w, h;
        public override Vector2 Render(Vector4 Dimensions)
        {
            bg_vertices = new float[]
            {
                Dimensions.X, Dimensions.Y, // Top Left
                Dimensions.X, Dimensions.W, // Bottom Left
                Dimensions.Z, Dimensions.W, // Bottom Right
                Dimensions.Z, Dimensions.Y, // Top Right
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, component_vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, 8 * sizeof(float), bg_vertices);

            texteditor_s.Use();
            texteditor_s.SetFloat("GutterWidth", TextManager.characters['0'].size.X * zoom * 2);
            GL.BindVertexArray(component_vao);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            text_s.Use();
            for (int i = 0; i < line_count; i++)
            {
                TextManager.Render(
                    i.ToString(),
                    MathHelper.MapRange(Dimensions.X, -1.0f, 1.0f, 0.0f, HostWindow.window_size.X) + TextManager.characters['0'].size.X * zoom / 4.0f,
                    MathHelper.MapRange(Dimensions.Y, -1.0f, 1.0f, 0.0f, HostWindow.window_size.Y) - (font_pixel_size * zoom * i) - font_pixel_size * zoom - texteditor_linepadding - texteditor_linepadding * i,
                    zoom,
                    new(0.32f)
                );
            }

            return new Vector2(w, h);
        }

        public override void Scroll(float offsetX, float offsetY)
        {
            base.Scroll(offsetX, offsetY);

            zoom = MathHelper.Clamp(zoom + scroll_offset.Y * -0.025f, 0.2f, 100.0f);
            // Console.WriteLine(zoom);
        }

        public override void HostWindowResize()
        {

        }
    }
}