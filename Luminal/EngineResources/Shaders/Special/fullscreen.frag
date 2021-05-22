#version 330 core

in vec2 uv;

uniform sampler2D tex;

out vec4 FragColour;

void main()
{
	FragColour = texture(tex, uv);
}