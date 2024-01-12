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
    
    float alpha = smoothstep(0.5 - aaf, 0.5 + aaf, d);
    color = vec4(textColor, alpha);
}  