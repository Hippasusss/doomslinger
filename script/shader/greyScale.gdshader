








shader_type canvas_item;

void fragment() 
{
    vec4 tex_colour = texture(TEXTURE, UV) * COLOR;

    float gray = dot(tex_colour.rgb, vec3(1, 1, 1));
    gray = gray/5.0 + 0.3;

    COLOR = vec4(vec3(gray), tex_colour.a);
}

