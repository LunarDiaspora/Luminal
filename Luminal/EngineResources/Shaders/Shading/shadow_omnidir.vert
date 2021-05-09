#version 450 core

layout (location=0) in vec3 aPos;

uniform mat4 Model;

void main()
{
	gl_Position = Model * vec4(aPos, 1.0f);
}