#version 330 core
in vec2 TexCoords;
out vec4 color;

uniform sampler2D text;
uniform vec3 textColor;
uniform vec4 backgroundColor;

void main()
{    
    float d = texture(text, TexCoords).r;
    float aaf = fwidth(d);
    
    if (backgroundColor.a == 0.0)
    {
        float alpha = smoothstep(0.5 - aaf, 0.5 + aaf, d);
        color = vec4(textColor, alpha);
    }
    else
    {
        float alpha = smoothstep(0.5 - aaf, 0.5 + aaf, d);
        color = vec4(mix(textColor.rgb, backgroundColor.rgb, alpha), 1.0);
    }
}  