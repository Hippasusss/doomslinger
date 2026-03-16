shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float width : hint_range(0.0, 10.0) = 1.0;

void fragment() {
    float size = width * fwidth(UV.x); // Dynamic width based on screen resolution
    vec4 sprite_color = texture(TEXTURE, UV);
    
    // Check neighbors
    float alpha = sprite_color.a;
    alpha += texture(TEXTURE, UV + vec2(size, 0.0)).a;
    alpha += texture(TEXTURE, UV + vec2(-size, 0.0)).a;
    alpha += texture(TEXTURE, UV + vec2(0.0, size)).a;
    alpha += texture(TEXTURE, UV + vec2(0.0, -size)).a;
    
    // If the neighbor has color but the current pixel is transparent, draw outline
    vec4 final_color = mix(vec4(0.0), outline_color, clamp(alpha, 0.0, 1.0));
    
    // Place original sprite over the outline
    COLOR = mix(final_color, sprite_color, sprite_color.a);
}