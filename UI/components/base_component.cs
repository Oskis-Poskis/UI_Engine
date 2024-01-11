using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using static Config;
using static Engine.AppWindow;
using WindowTemplate;

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
        public int component_vao, component_vbo;
        public ComponentType type;

        public virtual void Initialize() { }
        public virtual Vector2 Render(Vector4 Dimensions) { return Vector2.Zero; }
        public virtual void FrameResize() { }
        public virtual void HostWindowResize() { }
    }
}