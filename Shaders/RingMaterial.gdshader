shader_type spatial;
render_mode unshaded, cull_disabled, blend_add;

uniform vec4 ring_color : source_color = vec4(1.0, 0.4, 0.0, 0.5);
uniform float speed = 1.0;
uniform float frequency = 10.0;

void fragment() {
    float pattern = sin(UV.y * frequency + TIME * speed);
    pattern = smoothstep(0.2, 0.8, pattern);

    // Add some noise or secondary bands
    float pattern2 = sin(UV.y * (frequency * 2.0) - TIME * (speed * 1.5));
    pattern2 = smoothstep(0.4, 0.6, pattern2) * 0.5;

    float alpha_scan = pattern + pattern2;

    // Hexagon pattern or grid (Simulated)
    float grid = step(0.98, fract(UV.x * 30.0)) + step(0.98, fract(UV.y * 10.0));
    grid = clamp(grid, 0.0, 1.0);

    vec3 final_color = ring_color.rgb;
    float final_alpha = ring_color.a * (0.2 + alpha_scan * 0.4 + grid * 0.4);

    // Fresnel effect
    float fresnel = pow(1.0 - dot(NORMAL, VIEW), 2.0);
    final_alpha += fresnel * 0.5;

    // Vertical Fade (Top and Bottom)
    // Fade at the bottom (0.0 to 0.1) and top (0.8 to 1.0)
    float vertical_fade = smoothstep(0.0, 0.2, UV.y) * smoothstep(1.0, 0.6, UV.y);
    final_alpha *= vertical_fade;

    ALBEDO = final_color;
    ALPHA = clamp(final_alpha, 0.0, 1.0);
}
