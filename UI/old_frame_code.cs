/*
float frame_radius       = border_radius / HostWindow.window_aspect;
float frame_radius_y     = border_radius;
float angle_increment    = MathHelper.PiOver2 / (border_segments - 1);

Vector2[] circle_centers =
{
    new(BottomRight.X - frame_radius, TopLeft.Y -     frame_radius_y),  // Top Right
    new(BottomRight.X - frame_radius, BottomRight.Y + frame_radius_y),  // Bottom Right
    new(TopLeft.X +     frame_radius, BottomRight.Y + frame_radius_y),  // Bottom Left
    new(TopLeft.X +     frame_radius, TopLeft.Y -     frame_radius_y)   // Top Left
};

bool[] FlatCorner = new bool[]
{
    (BottomRight.X == 1.0f) | (TopLeft.Y == 1.0f),
    (BottomRight.X == 1.0f) | (BottomRight.Y == -1.0f),
    (TopLeft.X == -1.0f)    | (BottomRight.Y == -1.0f),
    (TopLeft.X == -1.0f)    | (TopLeft.Y == 1.0f)
};

frame_verts = new List<float>();
float[] arc_verts = new float[border_segments * 2];

for (int j = 0; j < 4; j++)
{
    if (!FlatCorner[j])
    {
        Vector2 circle_center = circle_centers[j];
        for (int i = 0; i < border_segments; i++)
        {
            arc_verts[i * 2]     = circle_center.X + frame_radius   * (float)Math.Cos(i * angle_increment - MathHelper.PiOver2 * j);
            arc_verts[i * 2 + 1] = circle_center.Y + frame_radius_y * (float)Math.Sin(i * angle_increment - MathHelper.PiOver2 * j);
            frame_verts.Insert(i * 2, arc_verts[i * 2 + 1]);
            frame_verts.Insert(i * 2, arc_verts[i * 2]);
        }
    }

    else
    {
        frame_verts.InsertRange(0, j switch
        {
            0 => new[] { BottomRight.X ==  1.0f ?  1.0f : BottomRight.X,  TopLeft.Y     ==  1.0f ?  1.0f : TopLeft.Y },
            1 => new[] { BottomRight.X ==  1.0f ?  1.0f : BottomRight.X,  BottomRight.Y ==  1.0f ? -1.0f : BottomRight.Y },
            2 => new[] { TopLeft.X     == -1.0f ? -1.0f : TopLeft.X,      BottomRight.Y == -1.0f ? -1.0f : BottomRight.Y },
            3 => new[] { TopLeft.X     == -1.0f ? -1.0f : TopLeft.X,      TopLeft.Y     ==  1.0f ?  1.0f : TopLeft.Y },
        });
    }
}
*/