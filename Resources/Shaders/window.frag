#version 330 core
in vec2 uvs;

uniform vec4  PosAndSize;
uniform vec2  Resolution;
 
uniform vec3  FrameColor;
uniform vec3  HeaderColor;
uniform vec3  BorderColor;

uniform vec4  DoRoundCorner;
uniform float BorderRadius;
uniform float BorderWidth;
uniform float HeaderHeight;

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
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

void main()
{  
    float aspect = (Resolution.x / Resolution.y);
    vec2 uv = uvs;
    uv.x *= aspect;

    float border_radius = (BorderRadius / Resolution.y) * 2;
    float border_width  = (BorderWidth  / Resolution.y) * 2;
    float header_height = (HeaderHeight / Resolution.y) * 2;

    float dist = sdRoundedBox(uv - vec2(PosAndSize.x * aspect, PosAndSize.y),
                                   vec2(PosAndSize.z * aspect, PosAndSize.w),
                                   DoRoundCorner * border_radius);

    float aaf = fwidth(dist);
    float mask = smoothstep(aaf, 0.0, dist);

    // float mask = dist < 0.0 ? 1.0 : 0.0;
    float borders = (dist >= -border_width && dist < 0.01) ? 0.0 : 1.0;
    borders = min(borders, (sdBox(uv - vec2(PosAndSize.x * aspect, PosAndSize.y + PosAndSize.w - header_height + 0.00001), vec2(PosAndSize.z * aspect * 2.0, border_width / 2.0)) < 0.0) ? 0.0 : 1.0);
    float header_bar = (sdBox(uv - vec2(PosAndSize.x * aspect, PosAndSize.y + PosAndSize.w - header_height / 2.0 + 0.00001), vec2(PosAndSize.z * aspect * 2.0, header_height / 2.0)) < 0.0) ? 0.0 : 1.0;

    vec3 color = mix(HeaderColor, FrameColor, header_bar);
    color = mix(BorderColor, color, borders);

    gl_FragColor = vec4(color, mask); // mix(FrameColor, BorderColor, bw)
}