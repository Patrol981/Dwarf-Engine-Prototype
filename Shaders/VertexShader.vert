#version 330 core

const int MAX_JOINTS = 50;
const int MAX_WEIGHTS = 3;

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

// 3,4 reseverd for texture attribs

layout (location = 5) in ivec3 aJointIdices;
layout (location = 6) in vec3 aWeights;

out vec2 texCoord;
out vec3 vNormal;
out vec3 vFragPos;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

uniform mat4 uJointTransforms[MAX_JOINTS];


void main(void) {
    texCoord = aTexCoord;
    vNormal = mat3(transpose(inverse(uModel))) * aNormal;
    vFragPos = vec3(uModel * vec4(aPosition, 1.0));

    vec4 totalLocalPos = vec4(0.0);

    for(int i=0; i<MAX_WEIGHTS; i++) {
        vec4 localPosition = uJointTransforms[aJointIdices[i]] * vec4(aPosition, 1.0);
        totalLocalPos += localPosition * aWeights[i];
    }

    if(totalLocalPos == vec4(0.0)) {
        gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
    } else {
        gl_Position = totalLocalPos * uModel * uView * uProjection;
    }
}