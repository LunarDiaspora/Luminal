#version 330 core

in vec4 Colour;
in vec2 UV;
in vec3 FragPosition;
in vec3 Normal;

uniform sampler2D Texture;

out vec4 FragColour;

uniform vec3 AmbientColour;
uniform vec3 DiffuseColour;
uniform vec3 ObjectColour;

uniform vec3 LightPosition; // Temporary.

void main() {
	float ambStrength = 0.1f;
	vec3 ambient = AmbientColour * ambStrength;

	vec3 norm = normalize(-Normal);
	vec3 lightDir = normalize(LightPosition - FragPosition);
	float difference = max(dot(norm, lightDir), 0.0f);

	vec3 diffuse = (difference) * DiffuseColour;

	vec3 result = (ambient+diffuse) * ObjectColour;

	FragColour = vec4(result, 1.0f);
}