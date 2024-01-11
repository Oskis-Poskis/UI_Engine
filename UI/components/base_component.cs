using OpenTK.Mathematics;

namespace UI
{
    public enum ComponentType
    {
        Image      = 0,
        Button     = 1,
        TextEditor = 2,
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
        public Vector2 scroll_offset;

        public virtual void Initialize() { }
        public virtual void FrameResize() { }
        public virtual void HostWindowResize() { }
     
        public virtual Vector2 Render(Vector4 Dimensions) { return Vector2.Zero; }
        public virtual void Scroll(float offsetX, float offsetY)
        {
            scroll_offset = new Vector2(offsetX, offsetY);
        }
    }
}