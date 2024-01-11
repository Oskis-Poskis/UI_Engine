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

        public static int active_frame;
        public static bool any_frame_hovered;
        public static float border_width_x_dc, border_width_y_dc;
        public static float header_height_dc;
        public static float font_pixel_size_dc;

        public static void Initialize()
        {
            frame1 = new Frame(new(-1.0f, 0.5f), new(0.0f, -0.5f), "Frame 1");
            frame2 = new Frame(new( 0.0f, 0.5f), new(1.0f, -0.5f), "Frame 2");

            Texture testimage = Texture.LoadFromFile($"{base_directory}Resources/Images/checkerboard.png", out Vector2 ImageSize);
            ImageComponent imgcomp = new ImageComponent(testimage.Handle)
            {
                aspect_mode = ImageAspectMode.FillWidth,
                image_size  = ImageSize
            };

            TextEditor editor = new TextEditor();
            
            frame1.AddComponent(imgcomp);
            frame2.AddComponent(editor);

            frames = new List<Frame>
            {
                frame1,
                frame2
            };

            frame_s.Use();
            frame_s.SetVector3("BorderColor", border_color);
            frame_s.SetVector3("HeaderColor", header_color);
            frame_s.SetFloat("BorderRadius",  border_radius);
            frame_s.SetFloat("BorderWidth",   border_width);
            frame_s.SetFloat("HeaderHeight",  header_height);

            image_s.Use();
            image_s.SetFloat("BorderRadius", border_radius);
            image_s.SetFloat("BorderWidth",  border_width);
            
            texteditor_s.Use();
            texteditor_s.SetFloat("BorderRadius", border_radius);
            texteditor_s.SetFloat("BorderWidth",  border_width);
            texteditor_s.SetVector3("GutterColor", texteditor_gutter_bg);
            texteditor_s.SetVector3("BackgroundColor", texteditor_bg);

            border_width_x_dc = MathHelper.MapRange(border_width,  0.0f, window_size.X, 0.0f, 2.0f);
            border_width_y_dc = MathHelper.MapRange(border_width,  0.0f, window_size.Y, 0.0f, 2.0f);
            header_height_dc  = MathHelper.MapRange(header_height, 0.0f, window_size.Y, 0.0f, 2.0f);
        }

        public static void RenderFrames()
        {
            any_frame_hovered = false;
            
            foreach (Frame frame in frames)
            {
                if (frame.IsFrameHovered())
                {
                    if(mouse_state.IsButtonPressed(MouseButton.Left))
                    {
                        if(active_frame > -1)
                        {
                            if (!frames[active_frame].IsFrameHovered()) active_frame = frames.IndexOf(frame);
                        }
                        else active_frame = frames.IndexOf(frame);
                    }
                    any_frame_hovered = true;
                }
            }

            if (!any_frame_hovered && mouse_state.IsButtonPressed(MouseButton.Left)) active_frame = -1;

            foreach (Frame frame in frames) if (frames.IndexOf(frame) != active_frame) frame.Render();
            if (active_frame > -1)
            {
                frames[active_frame].CheckInteraction();
                hover_type = frames[active_frame].hover_type;
                frames[active_frame].Render();
            }
            else hover_type = HoverType.None;
        }

        public static void Resize()
        {
            frame_s.Use();
            frame_s.SetVector2("Resolution", window_size);

            image_s.Use();
            image_s.SetVector2("Resolution", window_size);

            texteditor_s.Use();
            texteditor_s.SetVector2("Resolution", window_size);
            
            border_width_x_dc = MathHelper.MapRange(border_width,  0.0f, window_size.X, 0.0f, 2.0f);
            border_width_y_dc = MathHelper.MapRange(border_width,  0.0f, window_size.Y, 0.0f, 2.0f);
            header_height_dc  = MathHelper.MapRange(header_height, 0.0f, window_size.Y, 0.0f, 2.0f);
            font_pixel_size_dc   = MathHelper.MapRange(font_pixel_size, 0.0f, window_size.Y, 0.0f, 2.0f);

            foreach(Frame frame in frames)
            {
                frame.Resize();
            }
        }
    }    
}