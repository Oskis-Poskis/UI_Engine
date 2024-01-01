using OpenTK.Mathematics;

public static class Config
{
    public struct Character
    {
        public int     texture_id;
        public Vector2 size;
        public Vector2 bearing;
        public float   advance;

        public Character(int TextureID, Vector2 Size, Vector2 Bearing, int Advance)
        {
            texture_id = TextureID;
            size       = Size;
            bearing    = Bearing;
            advance    = Advance;
        }
    }

    public enum InteractionType
    {
        None         = 0,
        LeftResize   = 1,
        RightResize  = 2,
        TopResize    = 3,
        BottomResize = 4,
        Move         = 5
    }

    public enum HoverType
    {
        Vertical   = 0,
        Horizontal = 1,
        Move       = 2,
        None       = 3
    }

    public static readonly Vector3 selected_frame_color = new(0.15f);
    public static readonly Vector3 frame_color          = new(0.125f);
    public static readonly Vector3 border_color         = new(1, 0, 0);
    public static readonly Vector3 header_color         = new(0.1f);

    public static readonly float margin         = 8;
    public static readonly float border_radius  = 8;
    public static readonly float border_width   = 4;

    public static readonly float header_height  = 25;
    public static readonly uint font_pixel_size = 32;

    public static readonly float line_width     = 2;
    public static readonly float point_size     = 8;
}