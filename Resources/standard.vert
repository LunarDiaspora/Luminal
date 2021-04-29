#version 450 core

layout (location=0) in vec3 aPosition;

out vec3 Pos;

void main()
{
	gl_Position = vec4(aPosition, 1.0f);
	Pos = aPosition;
}