shader_type spatial;

#include "res://assets/shaders/include/perlin_noise.gdshaderinc"

render_mode blend_mix, cull_back, unshaded;

uniform vec4 ray_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float edge_fade : hint_range(0.0, 0.5) = 0.1;

uniform float dust_size = .2;
uniform float dust_strength = 0.7;
uniform float dust_sharpness = 2.0;
uniform float dust_speed = 0.05;

uniform float fluctuation_size = 3.0;
uniform float fluctuation_strength = 1.0;
uniform float fluctuation_sharpness = 1.0;
uniform float fluctuation_speed = 0.3;

varying vec3 w_vertex;

void vertex() {
	w_vertex = VERTEX;
}

void fragment() {
    float alpha = 1.0 - UV.y; // thinner side more opaque, wider less
	
	alpha = smoothstep(edge_fade, 1.0 - edge_fade, alpha);
	
	float fluctuation = noise3D(w_vertex + TIME*fluctuation_speed, 1.0/fluctuation_size, vec3(0.0));
	fluctuation = clamp(fluctuation, 0.0, 1.0);
	fluctuation = pow(fluctuation, fluctuation_sharpness);
	fluctuation = mix(1.0, fluctuation, fluctuation_strength);
	
	float dust = noise3D(w_vertex + TIME * dust_speed, 1.0/dust_size, vec3(0.0));
	float dust_layer2 = noise3D(w_vertex - TIME * dust_speed, 0.8/dust_size, vec3(0.0));
	dust *= dust_layer2;
	dust = clamp(dust, 0.0, 1.0);
	dust = pow(dust, dust_sharpness);
	dust = mix(1.0, dust, dust_strength);
	
	
    ALBEDO = ray_color.rgb;
    ALPHA = alpha * ray_color.a * dust * fluctuation;
}