shader_type spatial;
render_mode unshadeds, depth_test_disabled, depth_draw_never;
stencil_mode write, compare_always, 1;

void fragment() {
    ALBEDO = vec3(0.0);
    ALPHA = 0.;
}