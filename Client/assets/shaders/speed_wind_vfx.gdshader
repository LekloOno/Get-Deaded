shader_type canvas_item;

uniform bool auto = false;
instance uniform float intensity : hint_range(0.0, 1.0) = 1.0;
instance uniform float phase;
instance uniform float manual_frequency;

uniform vec4 vignette_color : source_color = vec4(0.6, 0.0, 0.0, 1.0);
uniform float count: hint_range(4., 1000.) = 20.;
uniform float speed: hint_range(0., 50.) = 3.; 
uniform float frequency: hint_range(0.01,10.) = 1.;
uniform float start_radius: hint_range(0.0, 1.0) = 10.;
uniform float softness : hint_range(0.01, 1.0) = 1.0;
uniform float trail_length: hint_range(0.0, 2.0) = 0.2;

float auto_dist_mask(float dist, float hash) {
	float effective_speed = mix(0.2, 1.0, intensity) * speed;
	float effective_frequency = mix(0.1, 1.0, intensity) * frequency;
	float mask = fract(dist * effective_frequency - TIME * effective_speed * effective_frequency - hash);
	return smoothstep(1. - trail_length * effective_frequency, 1.0, mask);
}

float manual_dist_mask(float dist, float hash) {
	float mask = fract(dist * manual_frequency - phase - hash);
	return smoothstep(1. - trail_length * manual_frequency, 1.0, mask);
}

void fragment() {
    vec2 uv = SCREEN_UV - 0.5;
    uv.x *= SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	float dist = length(uv);
	float angle = atan(uv.y, uv.x);
	
	float angle_split = angle / (2.*PI) * count;
	float id = floor(angle_split);
	float angle_frac = fract(angle_split);
	float hash = fract(sin(id * 12.456212) * 43758.5453);
	
	float angle_spiked = sin(angle_frac * 2. * PI);
	angle_spiked = max(0., angle_spiked);
	float trails_mask = angle_spiked;
	
	float dist_mask;
	if (auto) {
		dist_mask = auto_dist_mask(dist, hash);
	} else {
		dist_mask = manual_dist_mask(dist, hash);
	}
	
	float dist_min =  smoothstep(start_radius, start_radius + softness, dist);
	COLOR.a = trails_mask * dist_mask * dist_min * intensity;
}