#version 330 core
out vec4 outputColor;

in vec3 fragPos;
in vec3 normal;

uniform float time;
uniform vec3 viewPos; // Camera position
uniform vec3 lightPos; // Light position

void main()
{
    // Tie-dye pattern
    vec2 p = (gl_FragCoord.xy / 500.0) - 0.5;
    float sx = 0.3 * (p.x + 0.8);
    float sy = 0.3 * (p.y + 0.2);
    float r = (sin(sx * 10.0 + cos(time / 2.0) * 5.0) + 1.0) / 2.0;
    float g = (cos(sy * 10.0 + sin(time / 1.5) * 5.0) + 1.0) / 2.0;
    float b = (sin(sqrt(sx*sx + sy*sy) * 20.0 + time) + 1.0) / 2.0;
    vec3 tieDyeColor = vec3(r, g, b);

    // Lighting
    float ambientStrength = 0.2;
    vec3 ambient = ambientStrength * vec3(1.0, 1.0, 1.0);

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(1.0, 1.0, 1.0);

    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * vec3(1.0, 1.0, 1.0);
    
    vec3 lighting = ambient + diffuse + specular;
    vec3 result = tieDyeColor * lighting;

    outputColor = vec4(result, 1.0);
} 