#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
// layout(location = 3) in vec4 aColor;

out vec3 vFragPos;
out vec3 vNormal;

//flat out vec3 pass_colour;//The "flat" qualifier stops the colour from being interpolated over the triangles.

//uniform vec3 lightDirection;
//uniform vec3 lightColour;
//uniform vec2 lightBias;

// uniform mat4 projectionViewMatrix;

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

//simple diffuse lighting
//vec3 calculateLighting() {
	// vec3 normal = aNormal.xyz * 2.0 - 1.0;//required just because of the format the normals were stored in (0 - 1)
	//float brightness = max(dot(-lightDirection, normal), 0.0);
	//return (lightColour * lightBias.x) + (brightness * lightColour * lightBias.y);
//}

void main(void) {
	vFragPos = vec3(uModel * vec4(aPosition, 1.0));
    vNormal = mat3(transpose(inverse(uModel))) * aNormal;
    gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
	//gl_Position = projectionViewMatrix * vec4(in_position, 1.0);
	
	//vec3 lighting = calculateLighting();
	//pass_colour = in_colour.rgb * lighting;
}