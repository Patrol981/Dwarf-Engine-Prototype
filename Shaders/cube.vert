#version 330 core
in vec3 aCubePosition;
in vec2 aCubeTexCoord;

out vec2 cubeTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    cubeTexCoord = aCubeTexCoord;

    gl_Position = vec4(aCubePosition, 1.0) * model * view * projection;
}