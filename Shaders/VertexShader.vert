#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out vec3 vFragPos;
out vec3 vNormal;
out vec2 vTexCoord;

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

void main(void) {
    vFragPos = vec3(uModel * vec4(aPosition, 1.0));
    vNormal = mat3(transpose(inverse(uModel))) * aNormal;
    vTexCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
    
    // gl_Position = uProjection * uView * vec4(vFragPos, 1.0);
}