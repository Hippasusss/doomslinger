shader_type canvas_item;

varying vec4 base_color;

void fragment() {
	base_color = texture(TEXTURE, UV) * COLOR;
	COLOR = vec4(0.0, 0.0, 0.0, base_color.a);
}

void light() {
	LIGHT = base_color * LIGHT_COLOR * LIGHT_ENERGY;
    LIGHT.a = base_color.a;
}
