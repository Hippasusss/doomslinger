








shader_type canvas_item;

uniform float intensity : hint_range(0.0, 1.0) = 1.0;
varying vec4 modColor;

void vertex()
{
    modColor = COLOR;
}

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    
    float luminance = dot(tex_color.rgb, vec3(0.299, 0.587, 0.114));
    vec3 recolored = luminance * modColor.rgb * vec3(2);
    
    vec3 final_rgb = mix(tex_color.rgb, recolored, intensity);
    COLOR = vec4(final_rgb, tex_color.a);
}
