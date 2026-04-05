shader_type canvas_item;

// Controls how many color "steps" are available per color channel
uniform int color_depth : hint_range(2, 256) = 8;
// Controls the intensity of the dithering pattern
uniform float dither_spread : hint_range(0.0, 2.0) = 1.0;

void fragment() {
    // Classic 4x4 Bayer matrix for ordered dithering
    const float bayer_matrix[16] = float[](
        0.0 / 16.0,  8.0 / 16.0,  2.0 / 16.0, 10.0 / 16.0,
        12.0 / 16.0,  4.0 / 16.0, 14.0 / 16.0,  6.0 / 16.0,
        3.0 / 16.0, 11.0 / 16.0,  1.0 / 16.0,  9.0 / 16.0,
        15.0 / 16.0,  7.0 / 16.0, 13.0 / 16.0,  5.0 / 16.0
    );

    // Get the base color from the SubViewport
    vec4 tex_color = texture(TEXTURE, UV);
    
    // Calculate accurate pixel coordinates based on the texture resolution
    vec2 size = vec2(textureSize(TEXTURE, 0));
    vec2 pixel_coord = floor(UV * size);
    
    // Find the correct index in the 4x4 Bayer matrix
    int x = int(pixel_coord.x) % 4;
    int y = int(pixel_coord.y) % 4;
    
    // Center the dither value around 0.0 (-0.5 to 0.5)
    float dither_value = bayer_matrix[y * 4 + x] - 0.5;
    
    // Calculate the color banding steps
    float steps = float(color_depth - 1);
    
    // Apply the dither offset and snap to the nearest color step
    vec3 dithered_color = round(tex_color.rgb * steps + (dither_value * dither_spread)) / steps;
    
    COLOR = vec4(dithered_color, tex_color.a);
}