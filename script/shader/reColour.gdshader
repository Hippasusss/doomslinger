








shader_type canvas_item;

uniform vec4 target_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float intensity : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    
    float luminance = dot(tex_color.rgb, vec3(0.299, 0.587, 0.114));
    vec3 recolored = luminance * target_color.rgb * vec3(2);
    
    vec3 final_rgb = mix(tex_color.rgb, recolored, intensity);
    COLOR = vec4(final_rgb, tex_color.a);
}
