using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Engine.AppWindow;

namespace UI
{
    public class FrameComponent
    {
        public int component_VAO, component_VBO;
        public float[] component_vertices;

        public virtual void Initialize()
        {

        }

        public virtual void Render(Vector4 Dimensions)
        {

        }

        public virtual void Resize()
        {
            
        }
    }

    public class ImageComponent : FrameComponent
    {
        public ImageComponent()
        {
            component_vertices = new float[]
            {
                -0.25f, -0.25f,
                -0.25f,  0.25f,
                 0.25f,  0.25f,
                -0.25f, -0.25f,
                 0.25f,  0.25f,
                 0.25f, -0.25f
            };

            component_VAO = GL.GenVertexArray();
            GL.BindVertexArray(component_VAO);

            component_VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, component_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, component_vertices.Length * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        }

        public override void Render(Vector4 Dimensions)
        {
            image_s.Use();

            component_vertices = new float[]
            {
                Dimensions.X, Dimensions.W,
                Dimensions.X, Dimensions.Y,
                Dimensions.Z, Dimensions.Y,
                Dimensions.X, Dimensions.W,
                Dimensions.Z, Dimensions.Y,
                Dimensions.Z, Dimensions.W
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, component_VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, component_vertices.Length * sizeof(float), component_vertices);

            GL.BindVertexArray(component_VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public override void Resize()
        {

        }
    }
}