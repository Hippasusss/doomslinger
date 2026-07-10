shader_type canvas_item;

uniform vec4 cutout_0;
uniform vec4 cutout_1;
uniform vec4 cutout_2;
uniform vec4 cutout_3;
uniform vec4 cutout_4;
uniform vec4 cutout_5;
uniform vec4 cutout_6;
uniform vec4 cutout_7;
uniform vec4 border_color;
uniform float border_width_uv;

bool inside(vec4 c, vec2 uv) {
    return c.z > 0.0 && uv.x >= c.x && uv.x <= c.x + c.z && uv.y >= c.y && uv.y <= c.y + c.w;
}

bool on_edge(vec4 c, vec2 uv, float bw) {
    bool hit = false;
    if (c.x > 0.0 && uv.x < c.x + bw) hit = true;
    if (c.x + c.z < 1.0 && uv.x > c.x + c.z - bw) hit = true;
    if (c.y > 0.0 && uv.y < c.y + bw) hit = true;
    if (c.y + c.w < 1.0 && uv.y > c.y + c.w - bw) hit = true;
    return hit;
}

void fragment() {
    vec2 uv = UV;
    vec4 active = vec4(0.0);

    if (active.z == 0.0 && inside(cutout_0, uv)) active = cutout_0;
    if (active.z == 0.0 && inside(cutout_1, uv)) active = cutout_1;
    if (active.z == 0.0 && inside(cutout_2, uv)) active = cutout_2;
    if (active.z == 0.0 && inside(cutout_3, uv)) active = cutout_3;
    if (active.z == 0.0 && inside(cutout_4, uv)) active = cutout_4;
    if (active.z == 0.0 && inside(cutout_5, uv)) active = cutout_5;
    if (active.z == 0.0 && inside(cutout_6, uv)) active = cutout_6;
    if (active.z == 0.0 && inside(cutout_7, uv)) active = cutout_7;

    if (active.z > 0.0 && on_edge(active, uv, border_width_uv)) {
        COLOR = border_color;
    } else if (active.z > 0.0) {
        COLOR.a = 0.0;
    }
}
