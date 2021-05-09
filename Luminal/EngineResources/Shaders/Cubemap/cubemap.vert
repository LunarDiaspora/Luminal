#version 450 core

layout (location=0) in vec3 aPos;

out vec3 UVWs;

uniform mat4 Projection;
uniform mat4 View;

void main()
{
	UVWs = aPos;
	gl_Position = Projection * View * vec4(aPos, 1.0f);
}