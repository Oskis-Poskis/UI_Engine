using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Engine.AppWindow;
using WindowTemplate;

using static Config;

namespace UI
{
    public enum ComponentType
    {
        Image  = 0,
        Button = 1,
    }

    public enum ImageAspectMode
    {
        Fill       = 0,
        FillWidth  = 1,
        FillHeight = 2
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
        public Vector2 image_size;
        public ImageAspectMode aspect_mode = ImageAspectMode.Fill;

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

        float dist_x, dist_y;
        float aspect, aspect2;
        public override void Render(Vector4 Dimensions)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture_id);

            dist_x = MathHelper.Abs(Dimensions.X * 0.5f + 0.5f - (Dimensions.Z * 0.5f + 0.5f));
            dist_y = MathHelper.Abs(Dimensions.Y * 0.5f + 0.5f - (Dimensions.W * 0.5f + 0.5f));
            bool even_aspect = image_size.X / image_size.Y == 1.0f ? true : false;

            switch (aspect_mode) 
            {
                case ImageAspectMode.Fill:
                    component_vertices = new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f, // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 0.0f, // Bottom Left
                        Dimensions.Z, Dimensions.W, 1.0f, 0.0f, // Bottom Right
                        Dimensions.Z, Dimensions.Y, 1.0f, 1.0f  // Top Right
                    };
                break;

                case ImageAspectMode.FillWidth:
                    if (!even_aspect) aspect = dist_y / dist_x / HostWindow.window_aspect * 2.0f;
                    else aspect = 1.0f / (dist_x / dist_y) / HostWindow.window_aspect;
                    component_vertices = new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f,                 // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 1.0f - aspect, // Bottom Left
                        Dimensions.Z, Dimensions.W, 1.0f, 1.0f - aspect, // Bottom Right
                        Dimensions.Z, Dimensions.Y, 1.0f, 1.0f                  // Top Right
                    };

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[3] { frame_color.X, frame_color.Y, frame_color.Z });
                    break;
                
                case ImageAspectMode.FillHeight: 
                    if (!even_aspect) aspect = 1.0f / (dist_y / dist_x) * HostWindow.window_aspect / 2.0f;
                    else aspect = dist_x / dist_y / (1.0f / HostWindow.window_aspect);
                    component_vertices =  new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f,   // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 0.0f,   // Bottom Left
                        Dimensions.Z, Dimensions.W, aspect, 0.0f, // Bottom Right
                        Dimensions.Z, Dimensions.Y, aspect, 1.0f  // Top Right
                    };
                    
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[3] { frame_color.X, frame_color.Y, frame_color.Z });
                    break;
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, component_VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, 16 * sizeof(float), component_vertices);

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