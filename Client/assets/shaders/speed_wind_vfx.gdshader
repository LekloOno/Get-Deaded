shader_type canvas_item;

instance uniform float intensity : hint_range(0.0, 1.0) = 1.0;
uniform vec4 vignette_color : source_color = vec4(0.6, 0.0, 0.0, 1.0);
uniform float tightness: hint_range(0.1, 10.0) = 2.0;
uniform float trails: hint_range(4., 1000.) = 20.; 
uniform float speed: hint_range(0., 50.) = 3.; 
uniform float frequency: hint_range(0.01,10.) = 1.;
uniform float start_radius: hint_range(0.0, 1.0) = 10.;
uniform float softness : hint_range(0.01, 1.0) = 1.0;
uniform float trail_length: hint_range(0.0, 1.0) = 0.2;

void fragment() {
    vec2 uv = SCREEN_UV - 0.5;
    uv.x *= SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	float dist = length(uv);
	float angle = atan(uv.y, uv.x);
	
	float angle_split = angle / (2.*PI) * trails;
	float id = floor(angle_split);
	float angle_frac = fract(angle_split);
	float hash = fract(sin(id * 12.456212) * 43758.5453);
	
	float angle_spiked = pow(sin(angle_frac * 2. * PI), tightness);
	angle_spiked = max(0., angle_spiked);
	float trails_mask = angle_spiked;
	
	float dist_min =  smoothstep(start_radius, start_radius + softness, dist);
	float dist_mask = fract(dist * frequency - TIME * speed - hash);
	dist_mask = smoothstep(1. - trail_length, 1.0, dist_mask);
	COLOR.a = trails_mask * dist_mask * dist_min * intensity;
}
