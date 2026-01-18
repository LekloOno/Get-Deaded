extends Control

@export var label: Control
var slider: Slider

var render_scale

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
		
	if !render_scale:
		_initialize()
	
	slider.value = render_scale
	slider.value_changed.emit(render_scale)
	
func _initialize():
	render_scale = get_tree().root.scaling_3d_scale
		
	var do_show = show_scale()
	visible = do_show
	
	if label:
		label.visible = do_show

func show_scale() -> bool:
	return get_tree().root.scaling_3d_mode != Viewport.SCALING_3D_MODE_BILINEAR || get_tree().root.scaling_3d_scale != 1

func _on_value_changed(new_value: float) -> void:
	render_scale = new_value
	get_tree().root.scaling_3d_scale = new_value
	
func _on_render_scale_mode_changed(mode: int):
	var do_show = mode != -1
	visible = do_show
	label.visible = do_show
	
	if !do_show:
		get_tree().root.scaling_3d_scale = 1
		return
	
	if mode == Viewport.SCALING_3D_MODE_BILINEAR:
		slider.max_value = 2
	else : # implictly, if it is FSR
		slider.max_value = 1
	
