shader_type canvas_item;

uniform float alpha_override : hint_range(0.0, 1.0) = 1.0;
uniform float pixel_size : hint_range(1.0, 8.0) = 2.0;
uniform float warp_amount : hint_range(0.0, 0.1) = 0.02;
uniform float scanline_strength : hint_range(0.0, 0.6) = 0.18;
uniform float noise_intensity : hint_range(0.0, 0.25) = 0.04;
uniform float chroma_offset : hint_range(0.0, 0.02) = 0.003;
uniform float flicker_strength : hint_range(0.0, 0.25) = 0.06;
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.35;

varying vec4 v_color;

float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453123);
}

vec2 crt_warp(vec2 uv) {
    vec2 centered = uv * 2.0 - 1.0;
    float radius = dot(centered, centered);
    centered *= 1.0 + radius * warp_amount;
    return centered * 0.5 + 0.5;
}

void vertex() {
    v_color = COLOR;
}

void fragment() {
    vec2 warped_uv = crt_warp(UV);
    bool outside_uv =
        warped_uv.x < 0.0 || warped_uv.x > 1.0 ||
        warped_uv.y < 0.0 || warped_uv.y > 1.0;
    vec4 alpha_source = outside_uv ? vec4(0.0) : texture(TEXTURE, warped_uv);

    vec2 texture_res = vec2(textureSize(TEXTURE, 0));
    vec2 snapped_uv = (floor(warped_uv * texture_res / pixel_size) + 0.5) / (texture_res / pixel_size);

    float line_noise = (random(vec2(floor(TIME * 20.0), floor(snapped_uv.y * 80.0))) - 0.5) * 0.01;
    snapped_uv.x += line_noise;

    float r = texture(TEXTURE, snapped_uv + vec2(chroma_offset, 0.0)).r;
    float g = texture(TEXTURE, snapped_uv).g;
    float b = texture(TEXTURE, snapped_uv - vec2(chroma_offset, 0.0)).b;
    vec4 tex_color = vec4(r, g, b, alpha_source.a);

    float scanline = sin(snapped_uv.y * texture_res.y * 3.14159);
    tex_color.rgb *= 1.0 - ((scanline * 0.5 + 0.5) * scanline_strength);

    float noise = (random(snapped_uv + vec2(TIME * 3.1, TIME * 1.7)) - 0.5) * noise_intensity;
    tex_color.rgb += noise;

    float flicker = 1.0 - flicker_strength + random(vec2(floor(TIME * 24.0), 0.0)) * flicker_strength;
    tex_color.rgb *= flicker;

    vec2 centered = warped_uv * (1.0 - warped_uv.yx);
    float vignette = pow(clamp(centered.x * centered.y * 18.0, 0.0, 1.0), 0.35);
    tex_color.rgb *= mix(1.0 - vignette_strength, 1.0, vignette);

    COLOR.rgb = tex_color.rgb * v_color.rgb;
    COLOR.a = tex_color.a * alpha_override * v_color.a;
    if (outside_uv || alpha_source.a <= 0.01) {
        COLOR = vec4(0.0);
    }
}
