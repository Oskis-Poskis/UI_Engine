using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

using WindowTemplate;
using WindowTemplate.Common;

using UI;
using static Config;

using System.Reflection;
using System.Runtime.InteropServices;

namespace Engine
{
    public class AppWindow : HostWindow
    {
        public AppWindow(NativeWindowSettings settings, bool UseWindowState) : base(settings, UseWindowState)
        {
            texteditor_s = new Shader($"{base_directory}Resources/Shaders/base.vert", $"{base_directory}Resources/Shaders/text_editor.frag");
            image_s      = new Shader($"{base_directory}Resources/Shaders/base.vert", $"{base_directory}Resources/Shaders/image.frag");
            frame_s      = new Shader($"{base_directory}Resources/Shaders/base.vert", $"{base_directory}Resources/Shaders/frame.frag");
            text_s       = new Shader($"{base_directory}Resources/Shaders/text.vert", $"{base_directory}Resources/Shaders/text.frag");
        }

        PolygonMode draw_mode = PolygonMode.Fill;
        
        public static StatCounter stats = new();
        public static Shader texteditor_s, frame_s, image_s, text_s;

        public static Vector2 cursor_pos;
        public static bool left_down, left_press;

        protected unsafe override void OnLoad()
        {
            base.OnLoad();

            // Freetype also fixes dll loading, no need for this
            // DllResolver.InitLoader();

            GLFW.SetScrollCallback(WindowPtr, Scrolling);
            GL.LineWidth(line_width);
            GL.PointSize(point_size);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            TextManager.Initialize();
            FrameManager.Initialize();

            Maximized += (sender) =>
            {
                FrameManager.Resize();
                TextManager.Resize();
            };
            
            Resize += (sender) =>
            {
                FrameManager.Resize();
                TextManager.Resize();
            };
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            stats.Count(args);

            left_down  = mouse_state.IsButtonDown(MouseButton.Left);
            left_press = mouse_state.IsButtonPressed(MouseButton.Left);
            cursor_pos = new Vector2(
                MathHelper.MapRange(mouse_pos.X, 0.0f, window_size.X, -1.0f,  1.0f),
                MathHelper.MapRange(mouse_pos.Y, 0.0f, window_size.Y,  1.0f, -1.0f)
            );

            Cursor = FrameManager.hover_type switch
            {
                HoverType.Horizontal => MouseCursor.HResize,
                HoverType.Vertical   => MouseCursor.VResize,
                HoverType.Move       => MouseCursor.Default,
                HoverType.None       => MouseCursor.Default
            };
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color4.Chartreuse);
            GL.PolygonMode(MaterialFace.FrontAndBack, draw_mode);

            // Render UI
            GL.Disable(EnableCap.DepthTest);
            TextManager.Render($"vendor: {GL.GetString(StringName.Vendor)}", 15, window_size.Y - font_pixel_size, 0.55f, new Vector3(1.0f));
            TextManager.Render($"FPS: {stats.fps:0.0}", 15, window_size.Y - font_pixel_size * 2.5f, 0.55f, new Vector3(1.0f));
            TextManager.Render($"ms: {stats.ms:0.0}", 15, window_size.Y - font_pixel_size * 3.25f, 0.55f, new Vector3(1.0f));
            TextManager.Render($"active_frame: {FrameManager.active_frame}", 15, window_size.Y - font_pixel_size * 4.75f, 0.55f, new Vector3(1.0f));
            TextManager.Render($"any_frame_hovered: {FrameManager.any_frame_hovered}", 15, window_size.Y - font_pixel_size * 5.5f, 0.55f, new Vector3(1.0f));
            for (int i = 0; i < 16; i++)
            {
                TextManager.Render("The quick brown fox jumped over the lazy dog", 15, 17 * i + 15, 0.5f, Vector3.One);
            }
            FrameManager.RenderFrames();
            GL.Enable(EnableCap.DepthTest);
            
            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
    
            switch (e.Key)
            {
                case Keys.Escape:
                    Close();
                    break;

                case Keys.D1:
                    draw_mode = PolygonMode.Fill;
                    break;

                case Keys.D2:
                    draw_mode = PolygonMode.Line;
                    break;

                case Keys.D3:
                    draw_mode = PolygonMode.Point;
                    break;
            }
        }
        
        private static unsafe void Scrolling(Window* window, double offsetX, double offsetY)
        {
            if (FrameManager.active_frame > -1)
            {
                if (FrameManager.frames[FrameManager.active_frame].IsFrameHovered())
                {
                    foreach (FrameComponent component in FrameManager.frames[FrameManager.active_frame].components)
                    {
                        component.Scroll((float)offsetX,(float)offsetY);
                    }
                }
            }
        }

        public static class DllResolver
        {
            static DllResolver()
            {
                NativeLibrary.SetDllImportResolver(typeof(FreeTypeSharp.FreeTypeLibrary).Assembly, DllImportResolver);
            }

            public static void InitLoader() { }

            public static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
            {
                if (OperatingSystem.IsLinux())
                {
                    if (NativeLibrary.TryLoad("/lib/x86_64-linux-gnu/libdl.so.2", assembly, searchPath, out IntPtr lib))
                    {
                        Console.WriteLine("libdl.so.2 exists");
                        return lib;
                    }
                }

                return IntPtr.Zero;
            }
        }
    }
}