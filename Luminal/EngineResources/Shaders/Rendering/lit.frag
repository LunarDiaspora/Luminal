#version 330 core

#define ONE 1.0f

in vec4 Colour;
in vec2 UV;
in vec3 FragPosition;
in vec3 Normal;
in vec3 TrueNormal;

out vec4 FragColour;

uniform vec3 AmbientColour;
uniform vec3 ObjectColour;

uniform vec3 ViewPosition;

uniform bool BlinnPhong;
uniform float BlinnPhongMultiplier;

struct Material
{
    
    vec3 Albedo;

    vec3 Specular;

    float Shininess;

    bool UseAlbedoMap;

    sampler2D Albedo_Map;

    bool UseSpecularMap;

    sampler2D Specular_Map;

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

struct SpotLight
{
    vec3 Position;

    vec3 Angle;

    vec3 Colour;

    float CutOff;

    float OuterCutOff;

    float Linear;

    float Quadratic;

    float Intensity;
};

#define MAX_POINTS 100
#define MAX_SPOTS 50
#define AMBIENT_STRENGTH    0.1f
#define SPECULAR_STRENGTH   0.5f

#define GAMMA_CORRECTION 2.2f

#define LIGHT_CONSTANT ONE

uniform PointLight Points[MAX_POINTS];
uniform int PointCount;

uniform SpotLight Spots[MAX_SPOTS];
uniform int SpotCount;

uniform Material Mat;

float Brightness(vec3 v)
{
    return (v.x + v.y + v.z) / 3f;
}

vec3 GetTextureOrAlbedo(vec2 uv)
{
    if (Mat.UseAlbedoMap)
    {
        return vec3(texture(Mat.Albedo_Map, uv));
    } else
    {
        return Mat.Albedo;
    }
}

vec3 CalculatePointLight(PointLight light, vec3 viewDirection)
{

    vec3 normal = normalize(Normal);

    vec3 lightDirection = normalize(light.Position - FragPosition);

    float difference = max(dot(normal, lightDirection), 0.0f);

    vec3 diffuse = difference * light.Colour;

    float specular;

    if (BlinnPhong)
    {
        vec3 halfway = normalize(lightDirection + viewDirection);

        specular = pow(max(dot(viewDirection, halfway), 0.0), Mat.Shininess * BlinnPhongMultiplier);
    } else
    {
        vec3 reflectionDirection = reflect(-lightDirection, normal);

        specular = pow(max(dot(viewDirection, reflectionDirection), 0.0), Mat.Shininess);
    }

    float distanceFromCamera = length(light.Position - FragPosition);

    float attenuation = 1.0f / (LIGHT_CONSTANT + light.Linear * distanceFromCamera +
                                light.Quadratic * (distanceFromCamera * distanceFromCamera));

    float specularWeight = 1.0f;

    if (Mat.UseSpecularMap)
    {
        specularWeight = Brightness(vec3(texture(Mat.Specular_Map, UV)));
    }

    vec3 specularColour = (Mat.Specular * light.Colour) * (specular * specularWeight);

    vec3 result = (diffuse + (specularColour * SPECULAR_STRENGTH));

    result += GetTextureOrAlbedo(UV);

    result *= light.Colour;

    result *= attenuation;

    return result;

}

vec3 CalculateSpotLight(SpotLight light, vec3 viewDirection)
{
    vec3 lightDirection = normalize(light.Position - FragPosition);

    float theta = dot(lightDirection, normalize(-light.Angle));
    float epsilon = light.CutOff - light.OuterCutOff;
    float softIntensity = clamp((theta - light.OuterCutOff) / epsilon, 0.0, 1.0);
    softIntensity = clamp(1 - softIntensity, 0.0, 1.0);

    vec3 result = vec3(0.0f, 0.0f, 0.0f);

    if (theta > light.CutOff)
    {
        vec3 norm = normalize(Normal);
        float diff = max(dot(norm, lightDirection), 0.0);
        vec3 diffuse = light.Colour * diff * GetTextureOrAlbedo(UV);

        // Phong
        vec3 reflectDir = reflect(-lightDirection, norm);
        float spec = pow(max(dot(viewDirection, reflectDir), 0.0), Mat.Shininess);

        float specularWeight = 1.0f;

        if (Mat.UseSpecularMap)
        {
            specularWeight = Brightness(vec3(texture(Mat.Specular_Map, UV)));
        }

        vec3 specular = (Mat.Specular * light.Colour) * (spec * specularWeight);

        //diffuse *= softIntensity;
        //specular *= softIntensity;

        result = (diffuse + (specular * SPECULAR_STRENGTH));

        result += GetTextureOrAlbedo(UV);

        result *= light.Colour;

        result *= softIntensity;

        float distance = length(light.Position - FragPosition) / light.Intensity;
        float attenuation = 1.0 / (LIGHT_CONSTANT + light.Linear * distance + light.Quadratic * (distance));    

        result *= attenuation;
    }

    return result;
}

void main()
{

    vec3 ambient = AmbientColour * AMBIENT_STRENGTH;

    vec3 viewDirection = normalize(ViewPosition - FragPosition);

    vec3 result = ambient * GetTextureOrAlbedo(UV);

    for (int i=0; i<PointCount; i++)
    {
        PointLight light = Points[i];
        result += CalculatePointLight(light, viewDirection);
    }

    for (int i=0; i<SpotCount; i++)
    {
        SpotLight spot = Spots[i];
        result += CalculateSpotLight(spot, viewDirection);
    }

    result = pow(result, vec3(1.0f / GAMMA_CORRECTION));

    FragColour = vec4(result, 1.0f);

}