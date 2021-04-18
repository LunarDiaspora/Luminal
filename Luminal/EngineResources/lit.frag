#version 330 core

#define ONE 1.0f

in vec4 Colour;
in vec2 UV;
in vec3 FragPosition;
in vec3 Normal;

out vec4 FragColour;

uniform vec3 AmbientColour;
uniform vec3 ObjectColour;

uniform vec3 ViewPosition;

struct Material
{
	
	vec3 Albedo;

	vec3 Specular;

	float Shininess;

};

struct PointLight
{
	
	vec3 Position;

	vec3 Colour;

	float Intensity;

	// TODO: Automatically determine linear and quadratic terms for a specific light.
	float Linear;

	float Quadratic;

};

#define MAX_POINTS 100
#define AMBIENT_STRENGTH    0.1f
#define SPECULAR_STRENGTH   0.5f

#define LIGHT_CONSTANT ONE

uniform PointLight Points[MAX_POINTS];
uniform int PointCount;

uniform Material Mat;

vec3 CalculatePointLight(PointLight light, vec3 viewDirection, Material mat)
{

	vec3 normal = normalize(-Normal);

	vec3 lightDirection = normalize(light.Position - FragPosition);

	float difference = max(dot(normal, lightDirection), 0.0f);

	vec3 diffuse = difference * light.Colour;

	vec3 reflectionDirection = reflect(-lightDirection, normal);

	float specular = pow(max(dot(viewDirection, reflectionDirection), 0.0), mat.Shininess);

	float distanceFromCamera = length(light.Position - FragPosition);

	float attenuation = 1.0f / (LIGHT_CONSTANT + light.Linear * distanceFromCamera +
								light.Quadratic * (distanceFromCamera * distanceFromCamera));

	vec3 specularColour = Mat.Specular * specular;

	vec3 result = (diffuse + (specularColour * SPECULAR_STRENGTH));

	result += Mat.Albedo;

	result *= attenuation;

	return result;

}

void main()
{

	vec3 ambient = AmbientColour * AMBIENT_STRENGTH;

	vec3 viewDirection = normalize(ViewPosition - FragPosition);

	vec3 result = ambient * Mat.Albedo;

	for (int i=0; i<PointCount; i++)
	{
		PointLight light = Points[i];
		result += CalculatePointLight(light, viewDirection, Mat);
	}

	FragColour = vec4(result, 1.0f);

}