shader_type canvas_item;

uniform float alpha_override : hint_range(0.0, 1.0) = 1.0;

varying vec4 v_color;

void vertex() {
    v_color = COLOR;
}

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    COLOR.rgb = v_color.rgb;
    
    if (tex_color.a > 0.01) {
        COLOR.a = alpha_override * v_color.a;
    } else {
        COLOR.a = 0.0;
    }
}
