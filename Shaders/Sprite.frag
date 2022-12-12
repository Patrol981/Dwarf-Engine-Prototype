
#version 330 core
in vec2 vTexCoord;
out vec4 vFragColor;

uniform sampler2D texture0;
uniform vec3 spriteColor;

void main()
{
    vFragColor = vec4(spriteColor, 1.0) * texture(texture0, vTexCoord);
}