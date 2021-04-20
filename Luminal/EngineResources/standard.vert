#version 330 core

layout (location=0) in vec3 aPos;
layout (location=1) in vec4 aColour;
layout (location=2) in vec2 aUV;

out vec4 Colour;
out vec2 UV;

uniform vec2 ScreenSize;
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main() {
	gl_Position = Model * View * Projection * vec4(aPos, 1.0f);
	Colour = aColour;
	UV = aUV;
}