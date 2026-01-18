extends Control

@export var label: Control
var slider: Slider

var sharpness

func _ready() -> void:
	init_children()
	slider.value_changed.connect(_on_value_changed)
	visibility_changed.connect(_on_visibility_changed)
	
func init_children():
	slider = get_slider()
	
func get_slider() -> Slider:
	for child in get_children():
		if child is Slider:
			return child
	return null
			
func _on_visibility_changed() -> void:
	if !visible:
		return
		
	if !sharpness:
		_initialize()
	
	slider.value = sharpness
	slider.value_changed.emit(sharpness)
	
func _initialize():
	sharpness = get_tree().root.fsr_sharpness
		
	var do_show = show_scale()
	visible = do_show
	
	if label:
		label.visible = do_show

func show_scale() -> bool:
	var mode = get_tree().root.scaling_3d_mode
	return mode == Viewport.SCALING_3D_MODE_FSR || mode == Viewport.SCALING_3D_MODE_FSR2

func _on_value_changed(new_value: float) -> void:
	sharpness = new_value
	get_tree().root.fsr_sharpness = sharpness
	
func _on_render_scale_mode_changed(mode: int):
	var do_show = mode != -1
	visible = do_show
	label.visible = do_show
	
	if !do_show:
		get_tree().root.fsr_sharpness = 1
