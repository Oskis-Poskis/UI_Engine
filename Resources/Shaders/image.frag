#version 330 core
in vec2 uvs;
in vec2 rel_uvs;
out vec4 color;

uniform vec4  PosAndSize;
uniform vec2  Resolution;
uniform vec4  DoRoundCorner;

uniform float BorderRadius;
uniform float BorderWidth;

uniform sampler2D image;

float sdRoundedBox(in vec2 p, in vec2 b, in vec4 r)
{
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x  = (p.y > 0.0) ? r. x  : r.y;
    vec2 q = abs(p) - b + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}

void main()
{    
    float aspect = (Resolution.x / Resolution.y);
    vec2 uv = uvs;
    uv.x *= aspect;

    float border_radius = (BorderRadius / Resolution.y) * 2;
    float border_width  = (BorderWidth  / Resolution.y) * 2;

    float dist = sdRoundedBox(uv - vec2(PosAndSize.x * aspect, PosAndSize.y),
                                   vec2(PosAndSize.z * aspect, PosAndSize.w),
                                   DoRoundCorner * border_radius);

    float borders = (dist >= -border_width && dist < 0.0) ? 1.0 : 0.0;
    float mask = (dist < 0.0 ? 1.0 : 0.0) - borders;

    color = vec4(texture(image, rel_uvs * 0.5 + 0.5).rgb, mask);
}  