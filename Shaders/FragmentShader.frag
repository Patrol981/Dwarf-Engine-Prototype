#version 330 core

out vec4 vFragColor;

in vec2 texCoord;
in vec3 vFragPos;
in vec3 vNormal;

uniform vec3 uViewPos;
uniform vec3 uDiffuse;
uniform sampler2D texture1;

const int toonColorLevels = 4;
const float toonScaleFactor = 1.0f / toonColorLevels;

void main()
{
    vec3 norm = normalize(vNormal);
    vec3 viewDir = normalize(uViewPos - vFragPos);

    vec4 diffColor = vec4(0,0,0,0);

    float cc = max(abs(dot(viewDir,norm)), 0.3f);
    vec3 res = vec3(cc*uDiffuse);
    vFragColor = texture(texture1, texCoord) * vec4(res, 1);
}