float fade(float t) {
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

int hash(int x, int y) {
    return ((x * 73856093) ^ (y * 19349663)) & 255;
}

vec2 grad2(int hash) {
    switch(hash & 7) {
        case 0: return vec2( 1, 0);
        case 1: return vec2(-1, 0);
        case 2: return vec2( 0, 1);
        case 3: return vec2( 0,-1);
        case 4: return vec2( 1, 1) * 0.7071;
        case 5: return vec2(-1, 1) * 0.7071;
        case 6: return vec2( 1,-1) * 0.7071;
        case 7: return vec2(-1,-1) * 0.7071;
    }
    return vec2(1,0);
}

float perlin_noise(vec2 p) {
    int xi = int(floor(p.x)) & 255;
    int yi = int(floor(p.y)) & 255;

    vec2 f = fract(p);

    vec2 u = vec2(fade(f.x), fade(f.y));

    float n00 = dot(grad2(hash(xi, yi)), f - vec2(0.0, 0.0));
    float n10 = dot(grad2(hash(xi + 1, yi)), f - vec2(1.0, 0.0));
    float n01 = dot(grad2(hash(xi, yi + 1)), f - vec2(0.0, 1.0));
    float n11 = dot(grad2(hash(xi + 1, yi + 1)), f - vec2(1.0, 1.0));

    float x1 = mix(n00, n10, u.x);
    float x2 = mix(n01, n11, u.x);

    return mix(x1, x2, u.y);
}

vec2 random(vec2 uv){
    uv = vec2( dot(uv, vec2(127.1,311.7) ),
               dot(uv, vec2(269.5,183.3) ) );
    return -1.0 + 2.0 * fract(sin(uv) * 43758.5453123);
}

float noise(vec2 uv) {
    vec2 uv_index = floor(uv);
    vec2 uv_fract = fract(uv);

    vec2 blur = smoothstep(0.0, 1.0, uv_fract);

    return mix( mix( dot( random(uv_index + vec2(0.0,0.0) ), uv_fract - vec2(0.0,0.0) ),
                     dot( random(uv_index + vec2(1.0,0.0) ), uv_fract - vec2(1.0,0.0) ), blur.x),
                mix( dot( random(uv_index + vec2(0.0,1.0) ), uv_fract - vec2(0.0,1.0) ),
                     dot( random(uv_index + vec2(1.0,1.0) ), uv_fract - vec2(1.0,1.0) ), blur.x), blur.y) + 0.5;
}

vec3 random3D(vec3 uvw){
	
    uvw = vec3( dot(uvw, vec3(127.1,311.7, 513.7) ),
               dot(uvw, vec3(269.5,183.3, 396.5) ),
			   dot(uvw, vec3(421.3,314.1, 119.7) ) );
			
    return -1.0 + 2.0 * fract(sin(uvw) * 43758.5453123);
}

float noise3D(vec3 uvw, float noise_scale, vec3 noise_transform){
	uvw *= noise_scale;
	uvw += noise_transform;
	
	vec3 gridIndex = floor(uvw); 
	vec3 gridFract = fract(uvw);
	
	vec3 blur = smoothstep(0.0, 1.0, gridFract);
	
	vec3 blb = gridIndex + vec3(0.0, 0.0, 0.0);
	vec3 brb = gridIndex + vec3(1.0, 0.0, 0.0);
	vec3 tlb = gridIndex + vec3(0.0, 1.0, 0.0);
	vec3 trb = gridIndex + vec3(1.0, 1.0, 0.0);
	vec3 blf = gridIndex + vec3(0.0, 0.0, 1.0);
	vec3 brf = gridIndex + vec3(1.0, 0.0, 1.0);
	vec3 tlf = gridIndex + vec3(0.0, 1.0, 1.0);
	vec3 trf = gridIndex + vec3(1.0, 1.0, 1.0);
	
	vec3 gradBLB = random3D(blb); 
	vec3 gradBRB = random3D(brb);
	vec3 gradTLB = random3D(tlb);
	vec3 gradTRB = random3D(trb);
	vec3 gradBLF = random3D(blf);
	vec3 gradBRF = random3D(brf);
	vec3 gradTLF = random3D(tlf);
	vec3 gradTRF = random3D(trf);
	
	
	vec3 distToPixelFromBLB = gridFract - vec3(0.0, 0.0, 0.0);
	vec3 distToPixelFromBRB = gridFract - vec3(1.0, 0.0, 0.0);
	vec3 distToPixelFromTLB = gridFract - vec3(0.0, 1.0, 0.0);
	vec3 distToPixelFromTRB = gridFract - vec3(1.0, 1.0, 0.0);
	vec3 distToPixelFromBLF = gridFract - vec3(0.0, 0.0, 1.0);
	vec3 distToPixelFromBRF = gridFract - vec3(1.0, 0.0, 1.0);
	vec3 distToPixelFromTLF = gridFract - vec3(0.0, 1.0, 1.0);
	vec3 distToPixelFromTRF = gridFract - vec3(1.0, 1.0, 1.0);
	
	float dotBLB = dot(gradBLB, distToPixelFromBLB);
	float dotBRB = dot(gradBRB, distToPixelFromBRB);
	float dotTLB = dot(gradTLB, distToPixelFromTLB);
	float dotTRB = dot(gradTRB, distToPixelFromTRB);
	float dotBLF = dot(gradBLF, distToPixelFromBLF);
	float dotBRF = dot(gradBRF, distToPixelFromBRF);
	float dotTLF = dot(gradTLF, distToPixelFromTLF);
	float dotTRF = dot(gradTRF, distToPixelFromTRF);
	
	
	return mix(
		mix(
			mix(dotBLB, dotBRB, blur.x),
			mix(dotTLB, dotTRB, blur.x), blur.y
		),
		mix(
			mix(dotBLF, dotBRF, blur.x),
			mix(dotTLF, dotTRF, blur.x), blur.y
		), blur.z
	) + 0.5;
}