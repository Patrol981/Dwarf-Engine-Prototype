#version 330

out vec4 outputColor;

in vec2 cubeTexCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    outputColor = mix(texture(texture0, cubeTexCoord), texture(texture1, cubeTexCoord), 0.2);
}