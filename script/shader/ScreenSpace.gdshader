shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform float dither_size : hint_range(1.0, 10.0, 1.0) = 1.0;
uniform float color_levels : hint_range(2.0, 256.0, 1.0) = 8.0;
uniform float dither_strength : hint_range(0.0, 1.0) = 0.1;
uniform float gradient_threshold : hint_range(0.0, 0.1) = 0.01;

void fragment() {
	vec4 screen_color = texture(screen_texture, SCREEN_UV);
	
	// Determine the pixel position for dithering
	vec2 pixel_pos = floor(FRAGCOORD.xy / dither_size);
	
	// 4x4 Bayer Matrix
	const float bayer[16] = float[](
		0.0 / 16.0, 8.0 / 16.0, 2.0 / 16.0, 10.0 / 16.0,
		12.0 / 16.0, 4.0 / 16.0, 14.0 / 16.0, 6.0 / 16.0,
		3.0 / 16.0, 11.0 / 16.0, 1.0 / 16.0, 9.0 / 16.0,
		15.0 / 16.0, 7.0 / 16.0, 13.0 / 16.0, 5.0 / 16.0
	);
	
	int x = int(mod(pixel_pos.x, 4.0));
	int y = int(mod(pixel_pos.y, 4.0));
	float dither_value = bayer[x + y * 4];
	
	// Calculate the local gradient to detect smooth transitions vs flat colors
	vec3 derivative = fwidth(screen_color.rgb);
	float delta = max(max(derivative.r, derivative.g), derivative.b);
	
	// Mask dithering: only apply it where there is a change in color (gradient)
	float gradient_mask = smoothstep(0.0, gradient_threshold, delta);
	
	// Apply dither before posterization, scaled by the gradient mask
	vec3 color = screen_color.rgb;
	color += (dither_value - 0.5) * dither_strength * gradient_mask;
	
	// Posterize to the specified color levels
	color = floor(color * (color_levels - 1.0) + 0.5) / (color_levels - 1.0);
	
	COLOR = vec4(color, screen_color.a);
}
