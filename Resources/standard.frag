#version 450 core

uniform sampler2D boris;

in vec2 Aux;

out vec4 Colour;

void main()
{
	vec4 tex = texture(boris, Aux);
	Colour = vec4(tex.xyz, 1.0);
}