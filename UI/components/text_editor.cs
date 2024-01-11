using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Config;
using static Engine.AppWindow;
using WindowTemplate;


namespace UI
{
    public class TextEditor : FrameComponent
    {
        private int gutter_vao, gutter_vbo;

        public TextEditor()
        {
            type = ComponentType.Image;

            gutter_vao = GL.GenVertexArray();
            GL.BindVertexArray(gutter_vao);

            gutter_vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, gutter_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 8 * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        float w, h;
        public override Vector2 Render(Vector4 Dimensions)
        {
            text_s.Use();
            TextManager.Render(
                "test",
                MathHelper.MapRange(Dimensions.X, -1.0f, 1.0f, 0.0f, HostWindow.window_size.X),
                MathHelper.MapRange(Dimensions.W, -1.0f, 1.0f, 0.0f, HostWindow.window_size.Y),
                0.55f,
                Vector3.One
            );

            GL.BindVertexArray(gutter_vao);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            return new Vector2(w, h);
        }        

        public override void HostWindowResize()
        {

        }
    }
}