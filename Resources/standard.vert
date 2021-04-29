#version 450 core

layout (location=0) in vec3 aPosition;
layout (location=1) in vec2 aUV;

out vec3 Pos;
out vec2 Aux;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	gl_Position = Projection * View * Model * vec4(aPosition, 1.0f);
	Pos = aPosition;
	Aux = aUV;
}