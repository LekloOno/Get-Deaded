shader_type canvas_item;

uniform bool use_invert = false;
uniform vec4 flat_color : source_color = vec4(0.6, 0.6, 0.6, 1.0);
uniform float opacity : hint_range(0.0, 1.0) = 0.8;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear, repeat_disable;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    if (use_invert) {
        vec3 behind = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;
        vec3 inverted = vec3(1.0) - behind;
        COLOR = vec4(inverted, tex.a * opacity);
    } else {
        COLOR = vec4(flat_color.rgb, tex.a * flat_color.a * opacity);
    }
}