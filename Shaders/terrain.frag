#version 330 core

// flat in vec3 pass_colour;
out vec4 vFragColor;


in vec3 vFragPos;
in vec3 vNormal;
in vec2 TexCoord;

uniform vec3 uViewPos;
uniform vec3 uDiffuse;


void main(void) {

	// out_colour = vec4(pass_colour, 1.0);
	vec3 norm = normalize(vNormal);
    vec3 viewDir = normalize(uViewPos - vFragPos);

    float cc = max(abs(dot(viewDir,norm)), 0.3f);
    vec3 res = vec3(cc*uDiffuse);
    vFragColor = vec4(res, 1.0);
    // vFragColor = texture(texture0, TexCoord) * vec4(res, 1.0);
}