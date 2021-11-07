#version 400 core

layout (location=0) in vec3 in_vec_Pos;
layout (location=1) in vec2 in_vec_UV;
layout (location=2) in vec3 in_vec_Normal;

void main()
{
    gl_Position = vec4(in_vec_Pos, 1.0f);
}