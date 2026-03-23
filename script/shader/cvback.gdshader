shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// Now these represent Screen Coordinates (0.0 is Top, 1.0 is Bottom of Window)
uniform float fade_start : hint_range(0.0, 1.0) = 0.7; 
uniform float fade_end : hint_range(0.0, 1.0) = 0.85;

void fragment() {
    vec4 c = texture(screen_texture, SCREEN_UV);

    // Un-premultiply alpha
    if (c.a > 0.001) {
        c.rgb /= c.a;
    } else {
        c.rgb = vec3(0.0);
    }

    // Use SCREEN_UV.y instead of UV.y
    // This creates a global "cutoff line" on the screen
    float mask = smoothstep(fade_start, fade_end, SCREEN_UV.y);
    float alpha_multiplier = 1.0 - mask;

    // Apply fade and re-multiply
    c.a *= alpha_multiplier;
    c.rgb *= c.a; 

    COLOR = c;
}
