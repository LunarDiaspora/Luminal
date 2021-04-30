#version 330 core

layout (location=0) in vec3 aPosition;
layout (location=1) in vec3 aNormal;
layout (location=2) in vec2 aUV;

out vec3 VertexPosition;
out vec2 UV;
out vec3 Normal;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	gl_Position = Projection * View * Model * vec4(aPosition, 1.0f);
	VertexPosition = vec3(Model * vec4(aPosition, 1.0f));
	UV = aUV;
	Normal = mat3(transpose(inverse(Model))) * aNormal;
}