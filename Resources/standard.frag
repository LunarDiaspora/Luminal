#version 330 core

#define ONE 1.0f
#define MAXIMUM_POINT_LIGHTS 1000

#define AMBIENT_STRENGTH    0.1f
#define SPECULAR_STRENGTH   0.5f

#define LIGHT_CONSTANT ONE

in vec2 UV;
in vec3 Normal;
in vec3 FragPosition;

out vec4 oColour;

uniform vec3 AmbientColour;
uniform vec3 ObjectColour;

uniform vec3 ViewPosition;

struct Material
{
	vec3 Albedo;
	vec3 Specular;

	sampler2D AlbedoMap;
	sampler2D SpecularMap;

	bool UseAlbedoMap;
	bool UseSpecularMap;

	float Shininess;
};

struct PointLight
{
	vec3 Position;
	vec3 Colour;

	float Linear;
	float Quadratic;
};

uniform Material aMaterial;

uniform PointLight aPoints[MAXIMUM_POINT_LIGHTS];

uniform int PointLightCount;

float Brightness(vec3 v)
{
	return (v.x + v.y + v.z) / 3f;
}

vec3 GetTextureOrAlbedo(vec2 uv)
{
	if (aMaterial.UseAlbedoMap)
	{
		return vec3(texture(aMaterial.AlbedoMap, uv));
	} else
	{
		return aMaterial.Albedo;
	}
}

vec3 DoPointLight(PointLight light, vec3 viewDirection)
{
	vec3 normal = normalize(-Normal);

	vec3 lightDirection = normalize(light.Position - FragPosition);

	float difference = max(dot(normal, lightDirection), 0.0f);

	vec3 diffuse = difference * light.Colour;

	vec3 reflectionDirection = reflect(-lightDirection, normal);

	float specular = pow(max(dot(viewDirection, reflectionDirection), 0.0), aMaterial.Shininess);

	float distanceFromCamera = length(light.Position - FragPosition);

	float attenuation = 1.0f / (LIGHT_CONSTANT + light.Linear * distanceFromCamera +
								light.Quadratic * (distanceFromCamera * distanceFromCamera));

	float specularWeight = 1.0f;

	if (aMaterial.UseSpecularMap)
	{
		specularWeight = Brightness(vec3(texture(aMaterial.SpecularMap, UV)));
	}

	vec3 specularColour = aMaterial.Specular * (specular * specularWeight);

	vec3 result = (diffuse + (specularColour * SPECULAR_STRENGTH));

	result += GetTextureOrAlbedo(UV);

	result *= attenuation;

	return result;
}

void main()
{
	vec3 ambient = AmbientColour * AMBIENT_STRENGTH;

	vec3 viewDirection = normalize(ViewPosition - FragPosition);

	vec3 result = ambient * GetTextureOrAlbedo(UV);

	for (int i=0; i<PointLightCount; i++)
	{
		PointLight light = aPoints[i];
		result += DoPointLight(light, viewDirection);
	}

	oColour = vec4(result, 1.0f);
}