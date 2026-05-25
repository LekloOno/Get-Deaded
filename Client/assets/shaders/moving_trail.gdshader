shader_type spatial;
render_mode cull_disabled, depth_draw_opaque, blend_mix;

instance uniform float near_edge = 0.0;
instance uniform float far_edge = 1.0;
uniform vec4 trail_color : source_color = vec4(1.0);
uniform vec4 emission_color : source_color = vec4(1.0);
uniform float emission_strength : hint_range(0.0, 32.0) = 8.0;

void vertex() {
    // VERTEX.x is -0.5 or +0.5 on the unit quad
    // remap to [near_edge, far_edge] in local space
    VERTEX.x = mix(near_edge, far_edge, VERTEX.x + 0.5);
}

void fragment() {
    ALBEDO = trail_color.rgb;
	EMISSION = emission_color.rgb * emission_strength;
    ALPHA  = trail_color.a;
}
