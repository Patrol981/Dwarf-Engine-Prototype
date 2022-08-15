#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

out vec2 texCoord;
out vec3 normal;
out vec3 fragPos;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main(void)
{
    texCoord = aTexCoord;
    normal = mat3(transpose(inverse(uModel))) * aNormal;
    fragPos = vec3(uModel * vec4(aPosition, 1.0));
    gl_Position =  vec4(aPosition, 1.0) * uModel * uView * uProjection;
}