shader_type canvas_item;
render_mode blend_mix;

instance uniform float intensity : hint_range(0.0, 1.0) = 1.0;
uniform vec4 vignette_color : source_color = vec4(0.6, 0.0, 0.0, 1.0);
uniform float radius : hint_range(0.0, 1.0) = 1.0;
uniform float softness : hint_range(0.01, 1.0) = 1.0;
uniform float pulse_speed : hint_range(0.5, 10.0) = 6.0;
uniform float pulse_intensity : hint_range(0.0, 1.0) = 0.15;

void fragment() {
    vec2 uv = SCREEN_UV - 0.5;
    uv.x *= SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
    float dist = length(uv);
    float vignette = smoothstep(radius, radius - softness, dist);
    COLOR = vignette_color;
	float pulse_factor = ((1.0 - pulse_intensity) + pulse_intensity * sin(TIME * pulse_speed));
    COLOR.a = (1.0 - vignette) * intensity * pulse_factor;
}