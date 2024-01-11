#version 330 core
in vec2 uvs;
out vec4 color;

uniform vec4 PosAndSize;
uniform vec2 Resolution;
uniform vec4 DoRoundCorner;

uniform vec3 GutterColor;
uniform vec3 BackgroundColor;

uniform float BorderRadius;
uniform float BorderWidth;
uniform float GutterWidth;

float sdRoundedBox(in vec2 p, in vec2 b, in vec4 r)
{
    r.xy = (p.x > 0.0) ? r.xy : r.zw;
    r.x  = (p.y > 0.0) ? r. x  : r.y;
    vec2 q = abs(p) - b + r.x;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}

float sdBox(in vec2 p, in vec2 b)
{
    vec2 d = abs(p) - b;
    return length(max(d,0.0)) + min(max(d.x,d.y), 0.0);
}

void main()
{    
    float aspect = (Resolution.x / Resolution.y);
    vec2 uv = uvs;
    uv.x *= aspect;

    float border_radius = (BorderRadius / Resolution.y) * 2;
    float border_width  = (BorderWidth  / Resolution.y) * 2;
    float gutter_width  = (GutterWidth  / Resolution.y) * 2;

    float dist = sdRoundedBox(uv - vec2(PosAndSize.x * aspect, PosAndSize.y),
                                   vec2(PosAndSize.z * aspect, PosAndSize.w),
                                   DoRoundCorner * border_radius);

    float borders = (dist >= -border_width && dist < 0.0) ? 1.0 : 0.0;
    float mask = (dist < 0.0 ? 1.0 : 0.0) - borders;

    float gutter_mask = sdBox(uv - vec2(PosAndSize.x * aspect + gutter_width, PosAndSize.y),
                                   vec2(PosAndSize.z * aspect, PosAndSize.w)) < 0.0 ? 0.0 : 1.0;
    

    color = vec4(mix(BackgroundColor, GutterColor, gutter_mask), mask);
}  