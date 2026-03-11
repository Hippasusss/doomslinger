shader_type canvas_item;

uniform float alpha_override : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    // Use the modulate color's RGB.
    COLOR.rgb = COLOR.rgb;
    // Set alpha to alpha_override if the texture is not transparent, otherwise 0.
    if (tex_color.a > 0.01) {
        COLOR.a = alpha_override;
    } else {
        COLOR.a = 0.0;
    }
}
