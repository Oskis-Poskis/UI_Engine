using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Engine.Common;
using static Engine.AppWindow;
using static WindowTemplate.HostWindow;

using WindowTemplate;
using static Config;

namespace UI
{
    public class FrameManager
    {
        private static Frame frame1, frame2;
        public static List<Frame> frames;
        public static HoverType hover_type;
        public static int active_window;
        public static bool any_window_hovered;
        public static float border_width_x_dc, border_width_y_dc;
        public static float header_height_dc;

        public static void Initialize()
        {
            frame1 = new Frame(new(-0.5f, 0.5f), new(0.5f, -0.5f), "Frame 1");
            frame2 = new Frame(new(-0.5f, 0.5f), new(0.5f, -0.5f), "Frame 2");

            Texture testimage = Texture.LoadFromFile($"{base_directory}Resources/Images/checkerboard.png", out Vector2 ImageSize);
            ImageComponent imgcomp = new ImageComponent(testimage.Handle)
            {
                aspect_mode = ImageAspectMode.FillWidth,
                image_size  = ImageSize
            };
            
            frame1.AddComponent(imgcomp);
            // frame2.AddComponent(imgcomp);
            // frame1.AddComponent(imgcomp);

            frames = new List<Frame>
            {
                frame1
            };

            window_s.Use();
            window_s.SetVector3("BorderColor", border_color);
            window_s.SetVector3("HeaderColor", header_color);
            window_s.SetFloat("BorderRadius",  border_radius);
            window_s.SetFloat("BorderWidth",   border_width);
            window_s.SetFloat("HeaderHeight",  header_height);

            image_s.Use();
            image_s.SetFloat("BorderRadius", border_radius);
            image_s.SetFloat("BorderWidth",  border_width);

            border_width_x_dc = MathHelper.MapRange(border_width,  0.0f, window_size.X, 0.0f, 2.0f);
            border_width_y_dc = MathHelper.MapRange(border_width,  0.0f, window_size.Y, 0.0f, 2.0f);
            header_height_dc  = MathHelper.MapRange(header_height, 0.0f, window_size.Y, 0.0f, 2.0f);
        }

        public static void RenderFrames()
        {
            any_window_hovered = false;
            
            foreach (Frame frame in frames)
            {
                if (frame.IsFrameHovered())
                {
                    if (mouse_state.IsButtonPressed(MouseButton.Left) && hover_type == HoverType.None) active_window = frames.IndexOf(frame);
                    any_window_hovered = true;
                }
            }
            
            frames[active_window].CheckInteraction();
            hover_type = frames[active_window].hover_type;

            foreach (Frame frame in frames) if (frames.IndexOf(frame) != active_window) frame.Render();
            frames[active_window].Render();

            if (!any_window_hovered) hover_type = HoverType.None;
        }

        public static void Resize()
        {
            window_s.Use();
            window_s.SetVector2("Resolution", window_size);

            image_s.Use();
            image_s.SetVector2("Resolution", window_size);
            
            border_width_x_dc = MathHelper.MapRange(border_width,  0.0f, window_size.X, 0.0f, 2.0f);
            border_width_y_dc = MathHelper.MapRange(border_width,  0.0f, window_size.Y, 0.0f, 2.0f);
            header_height_dc  = MathHelper.MapRange(header_height, 0.0f, window_size.Y, 0.0f, 2.0f);

            foreach(Frame frame in frames)
            {
                frame.Resize();
            }
        }
    }    
}