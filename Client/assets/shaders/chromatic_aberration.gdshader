shader_type canvas_item;

instance uniform float intensity : hint_range(0.0, 5.0) = 0.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
    vec2 uv = SCREEN_UV;

    float offset = intensity * 0.01;
	/*
    vec2 dir = uv - vec2(0.5);
    float r = texture(SCREEN_TEXTURE, uv + dir * offset).r;
    float g = texture(SCREEN_TEXTURE, uv).g;
    float b = texture(SCREEN_TEXTURE, uv - dir * offset).b;

    COLOR = vec4(r, g, b, 1.0);*/
	
	vec4 center = texture(SCREEN_TEXTURE, uv);
	vec4 shifted = textureLod(SCREEN_TEXTURE, uv + offset, 3.0);

	COLOR.rgb = mix(center.rgb,
	                vec3(shifted.r, center.g, shifted.b),
	                intensity);
}