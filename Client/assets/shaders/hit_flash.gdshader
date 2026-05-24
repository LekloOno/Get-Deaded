shader_type spatial;
render_mode blend_add, unshaded, cull_disabled, depth_draw_never;

instance uniform float intensity : hint_range(0.0, 1.0) = 10.0;
instance uniform vec3 flash_color : source_color = vec3(1.0, 1.0, 1.0);
uniform float sharpening : hint_range(0.5, 8.0) = 2.0;

uniform float grid_scale : hint_range(1.0, 20.0) = 6.0;
uniform float line_width : hint_range(0.01, 0.5) = 0.08;
uniform float core_radius : hint_range(0.0, 1.0) = 0.2; 

void fragment() {
    vec2 centered = UV * 2.0 - 1.0;
    float dist = length(centered);
    float falloff = smoothstep(1.0, 0.0, dist);
	falloff = pow(falloff, sharpening);
	
	vec2  grid_uv   = centered * grid_scale;
    vec2 grid_frac = fract(grid_uv);
	vec2 line_dist = min(grid_frac, 1.0 - grid_frac);
    float grid_line = 1.0 - smoothstep(0.0, line_width, min(line_dist.x, line_dist.y));
	float core = smoothstep(core_radius, 0.0, dist);
	float alpha = max(grid_line, core) * falloff * intensity;
	
    ALBEDO = flash_color;
    ALPHA = alpha;
}