#version 450 core

out vec4 FragColour;

in vec3 UVWs;

uniform samplerCube Cubemap;

void main()
{
	FragColour = texture(Cubemap, UVWs);
}