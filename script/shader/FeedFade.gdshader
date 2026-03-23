shader_type canvas_item;

// This captures the screen behind the sprite

uniform float humanY = 400.0; 
uniform float fadeDistance = 100.0;


void fragment() {
    vec4 screen_color = COLOR;

    float screen_height = 1.0 / SCREEN_PIXEL_SIZE.y;

    float startUV = humanY / screen_height;
    float endUV = (humanY + fadeDistance) / screen_height;

    float mask = 1.0 - smoothstep(startUV, endUV, SCREEN_UV.y);

    //screen_color.a *= mask;

    COLOR = screen_color;
}
