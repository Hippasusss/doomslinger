shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform float dither_size : hint_range(1.0, 10.0, 1.0) = 1.0;
uniform float color_levels : hint_range(2.0, 256.0, 1.0) = 8.0;
uniform float dither_strength : hint_range(0.0, 1.0) = 0.1;
uniform bool use_edge_mask = true;

void fragment() {
	vec4 screen_color = texture(screen_texture, SCREEN_UV);
	
	// Detect color changes by comparing with neighbors
	vec2 texel_size = SCREEN_PIXEL_SIZE;
	vec3 color_right = texture(screen_texture, SCREEN_UV + vec2(texel_size.x, 0.0)).rgb;
	vec3 color_down = texture(screen_texture, SCREEN_UV + vec2(0.0, texel_size.y)).rgb;
	
	// Calculate the difference/gradient
	float diff = distance(screen_color.rgb, color_right) + distance(screen_color.rgb, color_down);
	
	// Determine the pixel position for dithering
	vec2 pixel_pos = floor(FRAGCOORD.xy / dither_size);
	
	// 4x4 Dot Dither Matrix (Cluster Dot)
	const float dot_dither[16] = float[](
		12.0 / 16.0,  5.0 / 16.0,  6.0 / 16.0, 13.0 / 16.0,
		 4.0 / 16.0,  0.0 / 16.0,  1.0 / 16.0,  7.0 / 16.0,
		11.0 / 16.0,  3.0 / 16.0,  2.0 / 16.0, 10.0 / 16.0,
		15.0 / 16.0,  9.0 / 16.0,  8.0 / 16.0, 14.0 / 16.0
	);
	
	int x = int(mod(pixel_pos.x, 4.0));
	int y = int(mod(pixel_pos.y, 4.0));
	float dither_value = dot_dither[x + y * 4];
	
	// Apply dither before posterization
	vec3 color = screen_color.rgb;
	color += (dither_value - 0.5) * dither_strength;
	
	// Posterize to the specified color levels
	vec3 posterized = floor(color * (color_levels - 1.0) + 0.5) / (color_levels - 1.0);
	
	// Only apply the effect where there is a color change.
	// We use a small threshold to identify flat areas.
	float edge_mask = 1.0;
	if (use_edge_mask) {
		edge_mask = smoothstep(0.0, 0.001, diff);
	}
	vec3 final_color = mix(screen_color.rgb, posterized, edge_mask);
	
	COLOR = vec4(final_color, COLOR.a);
}
