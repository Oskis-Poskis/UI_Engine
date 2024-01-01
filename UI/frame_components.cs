using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Engine.AppWindow;

namespace UI
{
    public enum ComponentType
    {
        Image  = 0,
        Button = 1,
    }

    public class FrameComponent
    {
        public int component_VAO, component_VBO;
        public float[] component_vertices = new float[16];
        public ComponentType type;

        public virtual void Initialize() { }
        public virtual void Render(Vector4 Dimensions){ }
        public virtual void Resize() { }
    }

    public class ImageComponent : FrameComponent
    {
        public int texture_id;

        public ImageComponent(int TextureID)
        {
            texture_id = TextureID;
            type = ComponentType.Image;

            component_VAO = GL.GenVertexArray();
            GL.BindVertexArray(component_VAO);

            component_VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, component_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 16 * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public override void Render(Vector4 Dimensions)
        {
            component_vertices = new float[]
            {
                Dimensions.X, Dimensions.Y, -1.0f,  1.0f, // Top Left
                Dimensions.X, Dimensions.W, -1.0f, -1.0f, // Bottom Left
                Dimensions.Z, Dimensions.W,  1.0f, -1.0f, // Bottom Right
                Dimensions.Z, Dimensions.Y,  1.0f,  1.0f  // Top Right
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, component_VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, 16 * sizeof(float), component_vertices);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture_id);

            image_s.Use();
            GL.BindVertexArray(component_VAO);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        public override void Resize()
        {

        }
    }

    public class ButtonComponent : FrameComponent
    {

    }
}