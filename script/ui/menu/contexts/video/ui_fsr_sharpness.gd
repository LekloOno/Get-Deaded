extends Control

@export var label: Control

func _ready() -> void:
	update_ui()
	RenderScaleModeSetting.Changed.connect(_on_render_scale_mode_changed)
	
func update_ui():
	var do_show = show_scale()
	visible = do_show
	if label:
		label.visible = do_show

func show_scale() -> bool:
	var mode = RenderScaleModeSetting.Value
	return mode == Viewport.SCALING_3D_MODE_FSR || mode == Viewport.SCALING_3D_MODE_FSR2
	
func _on_render_scale_mode_changed(sender, mode):
	update_ui()
