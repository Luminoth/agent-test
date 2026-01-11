shader_type spatial;
render_mode unshaded, cull_disabled, blend_add;

uniform vec4 ring_color : source_color = vec4(1.0, 0.1, 0.05, 0.8);
uniform float speed = 1.0;
uniform float frequency = 10.0;
uniform sampler2D depth_texture : hint_depth_texture, filter_linear_mipmap;
uniform float intersection_width = 2.0;
uniform vec4 intersection_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);


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

    // Intersection Highlight
    float depth_tex = texture(depth_texture, SCREEN_UV).r;

    // NOTE: In Godot 4, the depth texture is raw. Reconstruction depends on Projection type.
    // For Perspective (which we are using now), standard reconstruction works.

    vec4 world_pos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV * 2.0 - 1.0, depth_tex, 1.0);
    world_pos.xyz /= world_pos.w;
    float linear_depth = -world_pos.z;
    float vertex_depth = -VERTEX.z;

    float diff = linear_depth - vertex_depth;

    // Strict Clamping: Only glow if scene is BEHIND the ring (diff > 0)
    // If diff < 0, it means the scene is in front (or self-z-fighting), so NO glow.
    float intersection = 0.0;
    if (diff > 0.0) {
        intersection = 1.0 - smoothstep(0.0, intersection_width, diff);
    }

    // Add UV Ground Glow - REMOVED to rely on pure intersection consistency
    // float ground_glow = smoothstep(0.1, 0.0, UV.y) * 3.0;

    // Mix intersection color on top (Additive or Mix)
    // Using simple addition for glow boost
    final_color += (intersection_color.rgb * intersection * 4.0);
    final_alpha += intersection;

    ALBEDO = final_color;
    ALPHA = clamp(final_alpha, 0.0, 1.0);
}
