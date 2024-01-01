#version 330 core
layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec2 aUVs;

out vec2 uvs;
out vec2 rel_uvs;

void main()
{
    uvs = aPosition;
    rel_uvs = aUVs;
    gl_Position = vec4(aPosition, 0.0, 1.0);
}
