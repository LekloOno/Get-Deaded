shader_type spatial;

render_mode unshaded, fog_disabled;

uniform sampler2D depth_texture : source_color, hint_depth_texture, filter_nearest;
uniform sampler2D screen_texture : source_color, hint_screen_texture, filter_nearest;
uniform sampler2D normal_texture : source_color, hint_normal_roughness_texture, filter_nearest;

uniform float depth_threshold = .1;
uniform float normal_threshold = 1.2;
uniform float max_thickness = 10.;
uniform float min_thickness = .51;
uniform float min_ratio = .2;
uniform float min_depth = .3;
uniform float fresnel_power = 5.;
uniform float angle_mask_power = 1.;
uniform float angle_modulation = 20.;
uniform bool relative_depth = true;

void vertex() {
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

vec3 get_original(vec2 screen_uv) {
	return texture(screen_texture, screen_uv).rgb;
}

vec3 get_normal(vec2 screen_uv) {
	return texture(normal_texture, screen_uv).rgb * 2.0 - 1.0;
}

vec2[4] get_kernel(vec2 uv, vec2 width) {
	vec2 kernel[4];
	kernel[0] = vec2(uv.x, uv.y + width.y);
	kernel[1] = vec2(uv.x, uv.y - width.y);
	kernel[2] = vec2(uv.x + width.x, uv.y);
	kernel[3] = vec2(uv.x - width.x, uv.y);
	return kernel;
}

float get_depth(vec2 screen_uv, mat4 inv_projection_matrix/*, mat4 inv_view_matrix, out vec3 world_position*/) {
	float depth = texture(depth_texture, screen_uv).x;
	vec3 ndc = vec3(screen_uv * 2.0 - 1.0, depth);
	vec4 view = inv_projection_matrix * vec4(ndc, 1.0);
	view.xyz /= view.w;
	float linear_depth = -view.z;/*
	vec4 world = inv_view_matrix * inv_projection_matrix * vec4(ndc, 1.0);
  	world_position = world.xyz / world.w;*/
	return linear_depth;
}

float normal_variation(vec2 screen_uv, vec2 kernel[4]) {
	vec3 normal = get_normal(screen_uv);
	float variation = 0.;
	
	for(int i = 0; i < 4; i++) {
		vec3 n = get_normal(kernel[i]);
		float d = distance(normal, n);
		variation += d;
	}
	return variation;
}

float depth_variation(vec2 screen_uv, vec2 kernel[4], mat4 inversion_matrix) {
	float depth = get_depth(screen_uv, inversion_matrix);
	float variation = -4.*depth;
	
	for(int i = 0; i < 4; i++) {
		float d = get_depth(kernel[i], inversion_matrix);
		variation += d;
	}
	variation = abs(variation);
	if (relative_depth)
		variation /= depth;
	return variation;
}
float fresnel(float amount, vec3 normal, vec3 view) {
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0)), amount);
}

float depth_grazing(float power, float mask_power, float modulation_factor, vec3 normal, vec3 view)
{
	float f = fresnel(power, normal, view) - 1.;
	f /= mask_power;
	f += 1.;
	f = clamp(f, 0., 1.);
	f *= modulation_factor;
	f += 1.;
	return f;
}

float closest_in_kernel(vec2 uv, vec2 kernel[4], mat4 inv_proj_matrix){
	float d = get_depth(uv, inv_proj_matrix);
	for(int i = 0; i < 4; i++) {
		d = min(d, get_depth(kernel[i], inv_proj_matrix));
	}
	return d;
}

float thickness_modulation(float depth, float max_t, float min_d) {
	float clamped_depth = max(depth, min_d)/min_d;
	return max_t/clamped_depth;
}

float gray(vec3 text) {
	return dot(text, vec3(0.299, 0.587, 0.114));
}

float soften(vec2 uv, vec2 screen_size, float edge) {
	return (edge
	+ gray(texture(screen_texture, uv + vec2(1.,0.)/screen_size).rgb)
	+ gray(texture(screen_texture, uv + vec2(-1.,0.)/screen_size).rgb)
	+ gray(texture(screen_texture, uv + vec2(0.,1.)/screen_size).rbg)
	+ gray(texture(screen_texture, uv + vec2(0.,-1.)/screen_size).rbg)) / 5.;
}


void fragment() {
	// Called for every pixel the material is visible on.
	vec3 original = get_original(SCREEN_UV);
	vec2[4] surroundings = get_kernel(SCREEN_UV, vec2(max_thickness));
	float modulated_thickness = thickness_modulation(
		closest_in_kernel(SCREEN_UV, surroundings, INV_PROJECTION_MATRIX),
		//get_depth(SCREEN_UV, INV_PROJECTION_MATRIX),
		max_thickness, min_depth);
		
	float thickness_diff = min(modulated_thickness - min_thickness, 0.);
	modulated_thickness = modulated_thickness - thickness_diff;
	float thickness_ratio = 1. + (thickness_diff / min_thickness);
	thickness_ratio = max(min_ratio, thickness_ratio);
	
	vec2 texel_size = modulated_thickness / VIEWPORT_SIZE.xy;
		
	vec2 kernel[4] = get_kernel(SCREEN_UV, texel_size);
	
	float outline = 0.;
	float normal_variation = normal_variation(SCREEN_UV, kernel);
	
	/*
	if (normal_variation > normal_threshold)
		outline = 1.;
	else {
		float depth_variation = depth_variation(SCREEN_UV, kernel, INV_PROJECTION_MATRIX);
		float grazing = depth_grazing(
			fresnel_power, angle_mask_power,
			angle_modulation, NORMAL, VIEW);
			
		float threshold = depth_threshold * grazing;
		if (depth_variation > threshold)
			outline = 1.;
	}*/
	float depth_variation = depth_variation(SCREEN_UV, kernel, INV_PROJECTION_MATRIX);
	float grazing = depth_grazing(
		fresnel_power, angle_mask_power,
		angle_modulation, NORMAL, VIEW);
		
	float threshold = depth_threshold * grazing;
	//outline = smoothstep(.0, threshold, depth_variation);
	//outline += smoothstep(.0, normal_threshold, normal_variation);
	outline = step(threshold, depth_variation);
	outline += step(normal_threshold, normal_variation);
	
	outline = max(outline * thickness_ratio, 0.);
	ALBEDO = mix(original, vec3(0.0), outline);
	//outline = soften(SCREEN_UV, VIEWPORT_SIZE, outline);
	//ALBEDO = mix(original, vec3(0.0), outline);
}
