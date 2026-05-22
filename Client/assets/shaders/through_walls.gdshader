shader_type spatial;
render_mode cull_front, unshaded, depth_test_disabled, blend_mix;

stencil_mode read, compare_not_equal, 1;

// GLOBAL
global uniform vec4 enemy_color;
global uniform float enemy_outlines_thickness;
global uniform float enemy_outlines_opacity;

instance uniform float alpha_override = 1.;
instance uniform float outlines_alpha_override = 1.;

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;

void vertex() {
    vec4 clip_position = PROJECTION_MATRIX * (MODELVIEW_MATRIX * vec4(VERTEX, 1.0));
    vec3 clip_normal = mat3(PROJECTION_MATRIX) * (mat3(MODELVIEW_MATRIX) * NORMAL);
    vec2 offset = normalize(clip_normal.xy) / VIEWPORT_SIZE * clip_position.w * enemy_outlines_thickness * 2.0;
    clip_position.xy += offset;
    POSITION = clip_position;
}

void fragment() {
    vec3 c = enemy_color.rgb;
    // manual sRGB -> linear
    ALBEDO = mix(c / 12.92, pow((c + 0.055) / 1.055, vec3(2.4)), step(0.04045, c));
	
	ALPHA = enemy_outlines_opacity * outlines_alpha_override;
}