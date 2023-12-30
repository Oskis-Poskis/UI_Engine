using OpenTK.Windowing.GraphicsLibraryFramework;

using WindowTemplate;
using static Config;
using static Engine.AppWindow;

namespace UI
{
    public class FrameManager
    {
        private static Frame frame1, frame2, frame3;
        public  static List<Frame> frames;
        public  static HoverType hover_type;
        public  static int active_window;
        public  static bool any_window_hovered;

        public static void Initialize()
        {
            frame1 = new Frame(new(-0.5f, 0.5f), new(0.5f, -0.5f), "Frame 1");
            frame2 = new Frame(new(-0.5f, 0.5f), new(0.5f, -0.5f), "Frame 2");
            frame3 = new Frame(new(-0.5f, 0.5f), new(0.5f, -0.5f), "Frame 3");

            frames = new List<Frame>
            {
                frame1,
                // frame2,
                // frame3
            };

            window_s.Use();
            window_s.SetVector3("BorderColor", border_color);
            window_s.SetVector3("HeaderColor", header_color);
            window_s.SetFloat("BorderRadius",  border_radius);
            window_s.SetFloat("BorderWidth",   border_width);
            window_s.SetFloat("HeaderHeight",  header_height);
        }

        public static void RenderFrames()
        {
            any_window_hovered = false;
            
            foreach (Frame frame in frames)
            {
                if (frame.IsFrameHovered())
                {
                    if (HostWindow.mouse_state.IsButtonPressed(MouseButton.Left) && hover_type == HoverType.None) active_window = frames.IndexOf(frame);
                    any_window_hovered = true;
                }
            }
            
            frames[active_window].CheckInteraction();
            hover_type = frames[active_window].hover_type;

            frames[active_window].Render();
            foreach (Frame frame in frames) if (frames.IndexOf(frame) != active_window) frame.Render();

            if (!any_window_hovered) hover_type = HoverType.None;
        }

        public static void Resize()
        {
            window_s.Use();
            window_s.SetVector2("Resolution", HostWindow.window_size);

            foreach(Frame frame in frames)
            {
                frame.Resize();
            }
        }
    }    
}