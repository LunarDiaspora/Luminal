#version 450 core

in vec4 FragPos;

uniform vec3 LightPos;
uniform float Far;

void main()
{
	float dist = length(FragPos.xyz - LightPos);
	dist /= Far;
	gl_FragDepth = dist;
}