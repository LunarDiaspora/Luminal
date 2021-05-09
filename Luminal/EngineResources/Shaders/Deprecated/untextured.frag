#version 330 core

in vec4 Colour;

uniform sampler2D Texture;

out vec4 FragColour;

void main() {
	FragColour = Colour;
	//FragColour = vec4(1f, 0f, 0f, 1f);
}