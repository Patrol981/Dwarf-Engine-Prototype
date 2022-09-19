#version 330 core

out vec4 vFragColor;

in vec3 vFragPos;
in vec3 vNormal;

uniform vec3 uViewPos;
uniform vec3 uDiffuse;

void main()
{
    vec3 norm = normalize(vNormal);
    vec3 viewDir = normalize(uViewPos - vFragPos);

    float cc = max(abs(dot(viewDir,norm)), 0.3f);
    vec3 res = vec3(cc*uDiffuse);
    vFragColor = vec4(res, 1);
}