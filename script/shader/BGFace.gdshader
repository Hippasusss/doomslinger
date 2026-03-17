shader_type canvas_item;

void fragment() {
	vec4 color = texture(TEXTURE, UV) * COLOR ;
	float fade = clamp((1.0 - UV.y) / 0.2, 0.0, 1.0);
	color.a *= fade;
    color.rgb = color.rgb * vec3(2);
	COLOR = color ;
}
