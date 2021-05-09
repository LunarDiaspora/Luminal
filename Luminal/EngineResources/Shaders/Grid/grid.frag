#version 450 core

layout(location = 1) in vec3 nearPoint;
layout(location = 2) in vec3 farPoint;
layout(location = 0) out vec4 outColor;

uniform mat4 viewMat;
uniform mat4 projMat;

uniform float near;
uniform float gridFalloff;
uniform float falloffCoeff;

uniform float gridSize;

vec4 grid(vec3 fragPos3D, float scale, bool drawAxis) {
    vec2 coord = fragPos3D.xz * scale;
    vec2 derivative = fwidth(coord);
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / derivative;
    float line = min(grid.x, grid.y);
    float minimumz = min(derivative.y, 1.0);
    float minimumx = min(derivative.x, 1.0);
    vec4 color = vec4(0.2, 0.2, 0.2, 1.0 - min(line, 1.0));

    vec2 coordMajor = coord / 10.0;
    vec2 derivativeMajor = fwidth(coordMajor);
    vec2 gridMajor = abs(fract(coordMajor - 0.5) - 0.5) / derivativeMajor;
    float lineMajor = min(gridMajor.x, gridMajor.y);

    if (int(lineMajor) == 0)
        color.xyz = vec3(0.4);

    if(fragPos3D.x > -1.0 * minimumx && fragPos3D.x < 1.0 * minimumx && drawAxis)
        color.xyz = vec3(0.0, 0.0, 1.0);

    if(fragPos3D.z > -1.0 * minimumz && fragPos3D.z < 1.0 * minimumz && drawAxis)
        color.xyz = vec3(1.0, 0.1, 0.1);

    return color;
}

float computeDepth(vec3 pos) {
    vec4 clip_space_pos = projMat * viewMat * vec4(pos, 1.0);
    return clip_space_pos.z / clip_space_pos.w;
}

float computeLinearDepth(vec3 pos) {
    vec4 clip_space_pos = projMat * viewMat * vec4(pos.xyz, 1.0);
    float clip_space_depth = (clip_space_pos.z / clip_space_pos.w) * 2.0 - 1.0; // put back between -1 and 1
    float linearDepth = (2.0 * near * gridFalloff) / (gridFalloff + near - clip_space_depth * (gridFalloff - near));
    return linearDepth / gridFalloff; // normalize
}

void main() 
{
	float t = -nearPoint.y / (farPoint.y - nearPoint.y);
    vec3 fragPos3D = nearPoint + t * (farPoint - nearPoint);

    gl_FragDepth = computeDepth(fragPos3D) * 0.5 + 0.5;

    float linearDepth = computeLinearDepth(fragPos3D);

    float fade = max(0, (falloffCoeff - linearDepth));

    vec4 g = grid(fragPos3D, gridSize, true);
    g += grid(fragPos3D, gridSize / 5.0f, true);

	outColor = g * float(t > 0.0);

    outColor.a *= fade;
}