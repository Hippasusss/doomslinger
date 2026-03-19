








shader_type canvas_item;
uniform float divider = 3.0;
uniform float boost = 0.3;

void fragment() 
{
    vec4 tex_colour = texture(TEXTURE, UV) * COLOR;

    float gray = dot(tex_colour.rgb, vec3(1, 1, 1));
    gray = gray/divider + boost;

    COLOR = vec4(vec3(gray), tex_colour.a);
}

