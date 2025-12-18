shader_type spatial;

render_mode unshaded, fog_disabled;

uniform sampler2D depth_texture : source_color, hint_depth_texture, filter_nearest, repeat_disable;
uniform sampler2D screen_texture : source_color, hint_screen_texture, filter_nearest, repeat_disable;
uniform sampler2D normal_texture : source_color, hint_normal_roughness_texture, filter_nearest, repeat_disable;

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
uniform bool half_thickness = true;	// Adds an outline pass to smoothen the discrete texel nature
uniform bool focal_scaled = true;	// Outline sizes distance scale uses camera focal length
uniform bool kernel_depth = true;	// Depth is determined as the smallest depth in the kernel rather than the depth of uv
uniform vec4 color : source_color = vec4(0.0, 0.0, 0.0, 1.0);

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

float get_normal_outline(vec2 uv, vec2[4] kernel, float threshold) {
	float variation = normal_variation(uv, kernel);
	//return clamp(pow(mix(.0, 1., n_variation/n_threshold),8.), 0., 1.);
	//return += smoothstep(.0, n_threshold, n_variation);
	return step(threshold, variation);
}

float get_depth_outline(
	vec2 uv, mat4 inv_projection_matrix,
	vec2[4] kernel, float threshold, float grazing
) {
	float variation = depth_variation(uv, kernel, inv_projection_matrix);
	float grazed_threshold = threshold * grazing;
	//return smoothstep(.0, grazed_threshold, d_variation);
	//return clamp(mix(.0, 1., d_variation/grazed_threshold), 0., 1.);
	return step(grazed_threshold, variation);
}

float get_outline(
	vec2 uv, vec3 normal, vec3 view, mat4 inv_projection_matrix,
	vec2 texel_size, float d_threshold, float n_threshold,
	out float d_grazing
) {
	vec2 kernel[4] = get_kernel(uv, texel_size);
	
	float outline = get_normal_outline(uv, kernel, n_threshold);
		
	if (outline < 1.) {
		d_grazing = depth_grazing(
			fresnel_power, angle_mask_power,
			angle_modulation, normal, view);
		
		outline += get_depth_outline(
			uv, inv_projection_matrix, kernel,
			d_threshold, d_grazing);
	}
	return outline;
}

float get_outline_with_grazed(
	vec2 uv, vec3 normal, vec3 view, mat4 inv_projection_matrix,
	vec2 texel_size, float d_threshold, float n_threshold,
	float d_grazing
) {
	vec2 kernel[4] = get_kernel(uv, texel_size);
	
	float outline = get_normal_outline(uv, kernel, n_threshold);
	
	if (outline < 1.) {
		outline += get_depth_outline(
			uv, inv_projection_matrix, kernel,
			d_threshold, d_grazing);
	}
	return outline;
}

float screen_size_thickness(
	float focal_length, float min_d,
	float thickness, float depth
) {
	float clamped_depth = max(depth, min_d)/min_d;
	return abs((focal_length * thickness)) / clamped_depth;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec3 original = get_original(SCREEN_UV);
	vec2[4] surroundings = get_kernel(SCREEN_UV, vec2(max_thickness)/VIEWPORT_SIZE.xy);
	
	float depth;
	if (kernel_depth) {
		depth = closest_in_kernel(SCREEN_UV, surroundings, INV_PROJECTION_MATRIX);
	} else {
		depth = get_depth(SCREEN_UV, INV_PROJECTION_MATRIX);
	}
	
	
	float modulated_thickness;
	if (focal_scaled) {
		modulated_thickness = screen_size_thickness(
			PROJECTION_MATRIX[1][1],
			min_depth,
			max_thickness,
			depth);
	} else {
		modulated_thickness = thickness_modulation(
			depth,
			max_thickness, min_depth);
	}
	
	float thickness = floor(modulated_thickness);
	float semi_thickness = max(ceil(modulated_thickness), min_thickness);
	float semi_thickness_ratio = mod(modulated_thickness, min_thickness)/min_thickness;
	
	vec2 texel_size = thickness / VIEWPORT_SIZE.xy;
	vec2 semi_texel_size = semi_thickness / VIEWPORT_SIZE.xy;

	float outline = .0;
	
	float d_grazing;
	if (thickness > 0.5) {
		outline += get_outline(
			SCREEN_UV, NORMAL, VIEW, INV_PROJECTION_MATRIX,
			texel_size, depth_threshold, normal_threshold, d_grazing);
	} else {
		d_grazing = depth_grazing(
			fresnel_power, angle_mask_power,
			angle_modulation, NORMAL, VIEW);
	}
	
	if (half_thickness) {
		float ratio = semi_thickness_ratio;
		if (thickness == 0.) {
			ratio = max(ratio, min_ratio);
		}
		outline += get_outline_with_grazed(
			SCREEN_UV, NORMAL, VIEW, INV_PROJECTION_MATRIX,
			semi_texel_size, depth_threshold, normal_threshold, d_grazing) * ratio;
	}
	
	ALBEDO = mix(original, color.rgb, outline * color.a);
}
