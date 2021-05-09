#version 330 core

layout (location=0) in vec2 aPos;
layout (location=1) in vec2 aUV;
layout (location=2) in vec4 aColour;

out vec4 Colour;
out vec2 UV;

uniform vec2 ScreenSize;

uniform mat4 Projection;

void main() {
	gl_Position = Projection * vec4(aPos, 0.0f, 1.0f);
	Colour = aColour;
	UV = aUV;
}