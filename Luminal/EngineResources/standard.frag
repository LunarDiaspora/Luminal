#version 330 core

in vec4 Colour;
in vec2 UV;

uniform sampler2D Texture;

out vec4 FragColour;

void main() {
	FragColour = texture(Texture, UV) * Colour;
}