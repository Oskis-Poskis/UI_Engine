using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Config;
using static Engine.AppWindow;
using WindowTemplate;

namespace UI
{
    public class ImageComponent : FrameComponent
    {
        public float[] image_vertices;
        public int texture_id;
        public Vector2 image_size;
        public ImageAspectMode aspect_mode = ImageAspectMode.Fill;

        public ImageComponent(int TextureID)
        {
            texture_id = TextureID;
            type = ComponentType.Image;

            component_vao = GL.GenVertexArray();
            GL.BindVertexArray(component_vao);

            component_vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, component_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 16 * sizeof(float), nint.Zero, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        float w, h;
        float aspect;
        public override Vector2 Render(Vector4 Dimensions)
        {
            w = MathHelper.Abs(Dimensions.X * 0.5f + 0.5f - (Dimensions.Z * 0.5f + 0.5f));
            h = MathHelper.Abs(Dimensions.Y * 0.5f + 0.5f - (Dimensions.W * 0.5f + 0.5f));
            bool even_aspect = image_size.X / image_size.Y == 1.0f ? true : false;

            switch (aspect_mode) 
            {
                case ImageAspectMode.Fill:
                    image_vertices = new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f, // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 0.0f, // Bottom Left
                        Dimensions.Z, Dimensions.W, 1.0f, 0.0f, // Bottom Right
                        Dimensions.Z, Dimensions.Y, 1.0f, 1.0f  // Top Right
                    };
                break;

                case ImageAspectMode.FillWidth:
                    if (!even_aspect) aspect = h / w / HostWindow.window_aspect * 2.0f;
                    else              aspect = 1.0f / (w / h) / HostWindow.window_aspect;
                    image_vertices = new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f,          // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 1.0f - aspect, // Bottom Left
                        Dimensions.Z, Dimensions.W, 1.0f, 1.0f - aspect, // Bottom Right
                        Dimensions.Z, Dimensions.Y, 1.0f, 1.0f           // Top Right
                    };
                    break;
                
                case ImageAspectMode.FillHeight: 
                    if (!even_aspect) aspect = 1.0f / (h / w) * HostWindow.window_aspect / 2.0f;
                    else              aspect = w / h / (1.0f / HostWindow.window_aspect);
                    image_vertices =  new float[]
                    {
                        Dimensions.X, Dimensions.Y, 0.0f, 1.0f,   // Top Left
                        Dimensions.X, Dimensions.W, 0.0f, 0.0f,   // Bottom Left
                        Dimensions.Z, Dimensions.W, aspect, 0.0f, // Bottom Right
                        Dimensions.Z, Dimensions.Y, aspect, 1.0f  // Top Right
                    };
                    break;
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, component_vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, 16 * sizeof(float), image_vertices);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture_id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[3] { frame_color.X, frame_color.Y, frame_color.Z });

            image_s.Use();
            GL.BindVertexArray(component_vao);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            return new Vector2(w, h);
        }

        public override void HostWindowResize()
        {

        }
    }
}