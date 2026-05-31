extends Control

@export var render_scale: Control
@export var render_scale_slider: Range
@export var fsr_sharpness: Control

func _ready() -> void:
	_on_setting_value_changed(null, RenderScaleModeSetting.Value)
	RenderScaleModeSetting.Changed.connect(_on_setting_value_changed)
	
func _on_setting_value_changed(sender, value):
	if sender == self:
		return
	
	if value == -1 :
		render_scale.visible = false
		fsr_sharpness.visible = false
		return
		
	render_scale.visible = true
	
	if value == Viewport.SCALING_3D_MODE_BILINEAR:
		render_scale_slider.max_value = 2
		fsr_sharpness.visible = false
	else : # implictly, if it is FSR
		render_scale_slider.max_value = 1
		fsr_sharpness.visible = true
