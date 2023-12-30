using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

using WindowTemplate;
using WindowTemplate.Common;

using static Config;
using static Engine.AppWindow;

namespace UI
{
    public class Frame
    {
        public string title = "Funny title teehee";
        private int VAO, VBO;
        private float[] frame_verts;
        public bool is_resizing = false, is_moving = false;
        
        public HoverType hover_type;
        public InteractionType frame_interaction = InteractionType.None;

        float margin_x_dc, margin_y_dc;
        Vector2 top_left, bottom_right;
        Vector4 pos_and_size, rounded_corner;

        public Frame(Vector2 TopLeft, Vector2 BottomRight, string Title)
        {
            title = Title;
            top_left = TopLeft;
            bottom_right = BottomRight;

            LoadSavedData();
            CreateFrameData();
            pos_and_size = CalculatePosAndSize();
            rounded_corner = CalculateRoundedCorners();

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, frame_verts.Length * sizeof(float), frame_verts.ToArray(), BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        }

        private void LoadSavedData()
        {
            
        }

        float scaling = 0.6f;
        public void Render()
        {
            // Render UI background
            window_s.Use();
            window_s.SetVector3("FrameColor", FrameManager.active_window == FrameManager.frames.IndexOf(this) ? selected_frame_color : frame_color);
            window_s.SetVector4("PosAndSize", pos_and_size);
            window_s.SetVector4("DoRoundCorner", rounded_corner);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            GL.BindVertexArray(0);

            // Render Text
            text_s.Use();
            TextManager.Render(
                title,
                HelperClass.MapRange(top_left.X, -1.0f, 1.0f, 0.0f, HostWindow.window_size.X) + (header_height - font_pixel_size * scaling) / 2.0f,
                HelperClass.MapRange(top_left.Y, -1.0f, 1.0f, 0.0f, HostWindow.window_size.Y) -  header_height + (header_height - font_pixel_size * scaling) / 2.0f,
                scaling,
                new Vector3(1.0f)
            );
        }

        float offsetx, offsety;
        Vector4 StoredPosition;
        public void CheckInteraction()
        {
            // Move Window
            if ((RectHoverCheck(
                new Vector2(top_left.X,     top_left.Y - margin_y_dc),
                new Vector2(bottom_right.X, top_left.Y - HelperClass.MapRange(header_height, 0.0f, HostWindow.window_size.Y, 0.0f, 2.0f))) |
                frame_interaction == InteractionType.Move && !is_resizing) | (HostWindow.keyboard_state.IsKeyDown(Keys.LeftAlt) && IsFrameHovered()))
            {
                hover_type = HoverType.Move;
                if (left_press)
                {
                    offsetx = cursor_pos.X - top_left.X;
                    offsety = cursor_pos.Y - top_left.Y;
                    StoredPosition = new Vector4(top_left.X, top_left.Y, bottom_right.X, bottom_right.Y);
                    is_moving = true;
                }
                if (left_down && is_moving)
                {
                    float new_topleft_x = cursor_pos.X - offsetx;
                    float new_topleft_y = cursor_pos.Y - offsety;
                    float new_bottomright_x = new_topleft_x + (StoredPosition.Z - StoredPosition.X);
                    float new_bottomright_y = new_topleft_y + (StoredPosition.W - StoredPosition.Y);

                    if (new_topleft_x >= -1.0f)
                    {
                        top_left.X = new_topleft_x;

                        CreateFrameData();
                        UpdateBuffer();

                        frame_interaction = InteractionType.Move;
                    }
                    else top_left.X = -1.0f;

                    if (new_topleft_y <= 1.0f)
                    {
                        top_left.Y = new_topleft_y;

                        CreateFrameData();
                        UpdateBuffer();

                        frame_interaction = InteractionType.Move;
                    }
                    else top_left.Y = 1.0f;

                    if (new_bottomright_x <= 1.0f)
                    {
                        bottom_right.X = new_bottomright_x;

                        CreateFrameData();
                        UpdateBuffer();

                        frame_interaction = InteractionType.Move;
                    }
                    else bottom_right.X = 1.0f;

                    if (new_bottomright_y >= -1.0f)
                    {
                        bottom_right.Y = new_bottomright_y;

                        CreateFrameData();
                        UpdateBuffer();

                        frame_interaction = InteractionType.Move;
                    }
                    else bottom_right.Y = -1.0f;
                }
                else
                {
                    frame_interaction = InteractionType.None;
                    is_moving = false;
                }
            }

            // Left Edge
            else if (RectHoverCheck(
                new Vector2(top_left.X - margin_x_dc, top_left.Y),
                new Vector2(top_left.X + margin_y_dc, bottom_right.Y)) | frame_interaction == InteractionType.LeftResize)
            {
                hover_type = HoverType.Horizontal;
                if (left_press)
                {
                    offsetx = cursor_pos.X - top_left.X;
                    is_resizing = true;
                }
                if (left_down && is_resizing)
                {
                    top_left.X = MathHelper.Clamp(cursor_pos.X - offsetx, -1.0f, bottom_right.X - margin_x_dc * 2);
                    CreateFrameData();
                    UpdateBuffer();

                    frame_interaction = InteractionType.LeftResize;
                }
                else
                {
                    frame_interaction = InteractionType.None;
                    is_resizing = false;
                }
            }

            // Right Edge
            else if (RectHoverCheck(
                new Vector2(bottom_right.X - margin_x_dc, top_left.Y),
                new Vector2(bottom_right.X + margin_y_dc, bottom_right.Y)) | frame_interaction == InteractionType.RightResize)
            {
                hover_type = HoverType.Horizontal;
                if (left_press)
                {
                    offsetx = cursor_pos.X - bottom_right.X;
                    is_resizing = true;
                }
                if (left_down && is_resizing)
                {
                    bottom_right.X = MathHelper.Clamp(cursor_pos.X - offsetx, top_left.X + margin_x_dc * 2, 1.0f);
                    CreateFrameData();
                    UpdateBuffer();

                    frame_interaction = InteractionType.RightResize;
                }
                else
                {
                    frame_interaction = InteractionType.None;
                    is_resizing = false;
                }
            }

            // Top Edge
            else if (RectHoverCheck(
                new Vector2(top_left.X,     top_left.Y + margin_y_dc),
                new Vector2(bottom_right.X, top_left.Y - margin_y_dc)) | frame_interaction == InteractionType.TopResize)
            {
                hover_type = HoverType.Vertical;
                if (left_press)
                {
                    offsety = cursor_pos.Y - top_left.Y;
                    is_resizing = true;
                }
                if (left_down && is_resizing)
                {
                    top_left.Y = MathHelper.Clamp(cursor_pos.Y - offsety, bottom_right.Y + HelperClass.MapRange(header_height, 0.0f, HostWindow.window_size.Y, 0.0f, 2.0f), 1.0f);
                    CreateFrameData();
                    UpdateBuffer();

                    frame_interaction = InteractionType.TopResize;
                }
                else
                {
                    frame_interaction = InteractionType.None;
                    is_resizing = false;
                }
            }

            // Bottom Edge
            else if (RectHoverCheck(
                new Vector2(top_left.X,     bottom_right.Y + margin_y_dc),
                new Vector2(bottom_right.X, bottom_right.Y - margin_y_dc)) | frame_interaction == InteractionType.BottomResize)
            {
                hover_type = HoverType.Vertical;
                if (left_press)
                {
                    offsety = cursor_pos.Y - bottom_right.Y;
                    is_resizing = true;
                }
                if (left_down && is_resizing)
                {
                    bottom_right.Y = MathHelper.Clamp(cursor_pos.Y - offsety, -1.0f, top_left.Y - HelperClass.MapRange(header_height, 0.0f, HostWindow.window_size.Y, 0.0f, 2.0f));
                    CreateFrameData();
                    UpdateBuffer();

                    frame_interaction = InteractionType.BottomResize;
                }
                else
                {
                    frame_interaction = InteractionType.None;
                    is_resizing = false;
                }
            }
            else hover_type = HoverType.None;

            pos_and_size = CalculatePosAndSize();
            rounded_corner = CalculateRoundedCorners();   
        }

        public void Resize()
        {
            margin_x_dc = HelperClass.MapRange(margin, 0.0f, HostWindow.window_size.X, 0.0f, 2.0f);
            margin_y_dc = HelperClass.MapRange(margin, 0.0f, HostWindow.window_size.Y, 0.0f, 2.0f);

            CreateFrameData();
            UpdateBuffer();
        }

        public void CreateFrameData()
        {
            frame_verts = new float[]
            {
                top_left.X,     top_left.Y,
                bottom_right.X, top_left.Y,
                bottom_right.X, bottom_right.Y,
                top_left.X,     bottom_right.Y,
            };
        }

        public void UpdateBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, frame_verts.Length * sizeof(float), frame_verts.ToArray());
        }

        public bool IsFrameHovered()
        {
            return RectHoverCheck(new Vector2(top_left.X - margin_x_dc, top_left.Y + margin_y_dc),
                                  new Vector2(bottom_right.X + margin_x_dc, bottom_right.Y - margin_y_dc)) |
                                  !(frame_interaction == InteractionType.None);
        }

        private Vector4 CalculatePosAndSize()
        {
            return new Vector4
            (
                (top_left.X + bottom_right.X) / 2.0f,
                (top_left.Y + bottom_right.Y) / 2.0f,
                MathHelper.Abs(bottom_right.X - top_left.X) / 2.0f,
                MathHelper.Abs(bottom_right.Y - top_left.Y) / 2.0f
            );
        }

        private Vector4 CalculateRoundedCorners()
        {
            return new Vector4
            (
                (bottom_right.X ==  1.0f) | (top_left.Y     ==  1.0f) ? 0.0f : 1.0f,
                (bottom_right.X ==  1.0f) | (bottom_right.Y == -1.0f) ? 0.0f : 1.0f,
                (top_left.X     == -1.0f) | (top_left.Y     ==  1.0f) ? 0.0f : 1.0f,
                (top_left.X     == -1.0f) | (bottom_right.Y == -1.0f) ? 0.0f : 1.0f
            );
        }

        public bool RectHoverCheck(Vector2 TopLeft, Vector2 BottomRight)
        {
            return cursor_pos.X > TopLeft.X && cursor_pos.X < BottomRight.X &&
                   cursor_pos.Y < TopLeft.Y && cursor_pos.Y > BottomRight.Y;
        }
    }
}