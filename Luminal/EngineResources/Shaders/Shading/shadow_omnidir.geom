#version 450 core

// Ooh, a geometry shader!
// How spicy.

layout (triangles) in;
layout (triangle_strip, max_vertices=18) out;

uniform mat4 ShadowMatrices[6];

out vec4 FragPos;

void main()
{
	for (int face=0; face<6; face++)
	{
		gl_Layer = face;
		for (int i=0; i<3; i++)
		{
			FragPos = gl_in[i].gl_Position;
			gl_Position = ShadowMatrices[face] * FragPos;
			EmitVertex();
		}
		EndPrimitive();
	}
}