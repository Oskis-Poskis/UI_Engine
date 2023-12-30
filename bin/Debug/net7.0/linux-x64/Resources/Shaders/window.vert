#version 330 core
layout(location = 0) in vec2 aPosition;
out vec2 uvs;

uniform int zOffset;

void main()
{
    uvs = aPosition;
    gl_Position = vec4(aPosition, zOffset, 1.0);
}
