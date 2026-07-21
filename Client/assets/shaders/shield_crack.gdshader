shader_type canvas_item;
render_mode blend_add, unshaded;

uniform vec4 crack_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec2 impact_uv = vec2(0.5, 0.5);   // center of the clear hole / crack ring, in UV space
uniform float seed = 0.0;                  // crack pattern seed
uniform float grow_time = 0.06;
uniform float fade_time = 0.4;

uniform float effect_time = 999.0;         // avoid TIME floating point divergence

uniform float cell_density = 7.0;
uniform float line_thickness = 0.035;
uniform float outer_radius = 0.75;
uniform float hole_radius : hint_range(0.0, 1.0) = 0.22;
uniform float edge_softness = 0.08;

uniform float halo_width : hint_range(0.2, 6.0) = 3.0;
uniform float core_intensity = 1.4;
uniform float halo_intensity = 0.6;

vec2 hash2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)) + seed, dot(p, vec2(269.5, 183.3)) + seed);
    return fract(sin(p) * 43758.5453123);
}

float crack_edge(vec2 uv) {
    vec2 p = uv * cell_density;
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float d1 = 8.0;
    float d2 = 8.0;

    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 neighbor = vec2(float(x), float(y));
            vec2 point = hash2(ip + neighbor);
            vec2 diff = neighbor + point - fp;
            float dist = dot(diff, diff);
            if (dist < d1) {
                d2 = d1;
                d1 = dist;
            } else if (dist < d2) {
                d2 = dist;
            }
        }
    }

    return sqrt(d2) - sqrt(d1);
}

vec4 process(vec2 uv, vec2 screen_pixel_size) {
	float total_time = grow_time + fade_time;

    if (effect_time > total_time) {
        return vec4(0.0);
    }

	vec2 centered = uv - impact_uv;
    centered.x *= screen_pixel_size.y / screen_pixel_size.x;
    float dist_from_impact = length(centered);

	float grow_t = clamp(effect_time / grow_time, 0.0, 1.0);
    float eased_grow = 1.0 - pow(1.0 - grow_t, 3.0);
    float front_radius = mix(outer_radius, hole_radius, eased_grow);

	float radius_mask = smoothstep(front_radius - edge_softness, front_radius + edge_softness, dist_from_impact);

	float edge = crack_edge(uv);
    float core = 1.0 - smoothstep(0.0, line_thickness, edge);
    float halo = 1.0 - smoothstep(0.0, line_thickness * halo_width, edge);
    float glow = core * core_intensity + max(halo - core, 0.0) * halo_intensity;

    float fade_t = clamp((effect_time - grow_time) / fade_time, 0.0, 1.0);
    float fade = 1.0 - fade_t;

    float alpha = glow * radius_mask * fade * crack_color.a;
	return vec4(crack_color.rgb, alpha);
}

void fragment() {
    COLOR = process(UV, SCREEN_PIXEL_SIZE);
}