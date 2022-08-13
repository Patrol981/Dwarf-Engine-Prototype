#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform sampler2D texture1;
uniform vec3 uDiffuse;
uniform vec3 viewpos;

void main() {
	vec3 norm = normalize(normal);
    vec3 viewDir = normalize(viewpos - fragPos);

    float cc = max(abs(dot(viewDir,norm)), 0.3f);
    vec3 res = vec3(cc*uDiffuse);

	//outputColor = texture(texture1, texCoord);
    outputColor = texture(texture1, texCoord) * vec4(res, 1.0);
}