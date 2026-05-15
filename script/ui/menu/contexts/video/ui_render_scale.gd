extends Control

@export var label: Control
var slider: Range

var render_scale

func _ready() -> void:
	init_children()
		
	slider.value_changed.connect(_on_value_changed)
	render_scale = RenderScaleScaleSetting.Value
	if (render_scale):
		update_ui()
	RenderScaleScaleSetting.Changed.connect(_on_setting_value_changed)
	
func _on_setting_value_changed(sender, value):
	if sender == self:
		return
		
	render_scale = value
	update_ui()
		
func update_ui():
	if slider == null:
		return
		
	slider.value = render_scale
	slider.value_changed.emit(render_scale)
	return
	
func init_children():
	slider = get_slider()
	
func get_slider() -> Range:
	for child in get_children():
		if child is Range:
			return child
	return null

func _on_value_changed(new_value: float) -> void:
	if new_value == render_scale:
		return
		
	render_scale = new_value
	RenderScaleScaleSetting.GdTryUpdateValue(self, render_scale)
	
func _on_render_scale_mode_changed(mode: int):
	var do_show = mode != -1
	get_parent().visible = do_show
	label.visible = do_show
	
	if !do_show:
		RenderScaleScaleSetting.GdTryUpdateValue(self, render_scale)
		return
	
	if !slider:
		return
		 
	if mode == Viewport.SCALING_3D_MODE_BILINEAR:
		slider.max_value = 2
	else : # implictly, if it is FSR
		slider.max_value = 1
	
