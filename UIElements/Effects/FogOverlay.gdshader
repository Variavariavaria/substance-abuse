shader_type canvas_item;
render_mode unshaded;

uniform vec4 fog_color : source_color = vec4(0.035, 0.045, 0.035, 0.62);
uniform vec4 haze_color : source_color = vec4(0.16, 0.17, 0.14, 1.0);
uniform float fog_density : hint_range(0.0, 1.0) = 0.55;
uniform float fog_speed : hint_range(0.0, 0.2) = 0.02;
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.65;

float hash(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

float noise(vec2 p) {
	vec2 i = floor(p);
	vec2 f = fract(p);
	vec2 u = f * f * (3.0 - 2.0 * f);

	float a = hash(i);
	float b = hash(i + vec2(1.0, 0.0));
	float c = hash(i + vec2(0.0, 1.0));
	float d = hash(i + vec2(1.0, 1.0));

	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
	float value = 0.0;
	float amplitude = 0.5;

	for (int i = 0; i < 5; i++) {
		value += amplitude * noise(p);
		p *= 2.03;
		amplitude *= 0.5;
	}

	return value;
}

void fragment() {
	vec2 uv = UV;
	vec2 drift = vec2(TIME * fog_speed, TIME * fog_speed * 0.35);

	float wide_layer = fbm(uv * vec2(3.0, 2.2) + drift);
	float thin_layer = fbm(uv * vec2(7.0, 5.5) - drift * 0.7 + 13.0);
	float fog = smoothstep(0.24, 0.86, wide_layer * 0.7 + thin_layer * 0.3);

	float distance_from_center = distance(uv, vec2(0.5));
	float vignette = smoothstep(0.32, 0.86, distance_from_center);

	vec3 tint = mix(fog_color.rgb, haze_color.rgb, fog);
	float alpha = fog_color.a + fog * fog_density * 0.25;
	alpha = clamp(alpha + vignette * vignette_strength * 0.32, 0.0, 0.94);

	COLOR = vec4(tint, alpha);
}
