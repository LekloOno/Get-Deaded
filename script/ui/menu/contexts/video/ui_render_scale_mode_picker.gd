extends OptionButton

signal mode_changed(mode: int)

var modes = {
	"DISABLED_RENDER_SCALE_MODE": -1,
	"BILINEAR_RENDER_SCALE_MODE": Viewport.SCALING_3D_MODE_BILINEAR,
	"FSR_RENDER_SCALE_MODE": Viewport.SCALING_3D_MODE_FSR,
	"FSR_2_RENDER_SCALE_MODE": Viewport.SCALING_3D_MODE_FSR2,
}

func _ready() -> void:
	for mode in modes:
		add_item(mode)
	item_selected.connect(_on_item_selected)
	update_ui()
	RenderScaleModeSetting.Changed.connect(_on_setting_value_changed)
	
func _on_setting_value_changed(sender, value):
	if sender != self:
		update_ui()

func mode_to_idx(mode) -> int:
	return modes.values().find(mode)

func _on_item_selected(index: int) -> void:
	var mode = modes[get_item_text(index)]
	
	if mode == -1 :
		RenderScaleModeSetting.GdTryUpdateValue(self, Viewport.SCALING_3D_MODE_BILINEAR)
	else :
		RenderScaleModeSetting.GdTryUpdateValue(self, mode)
		
	mode_changed.emit(mode)
	
func update_ui():	
	var curr_mode = get_current_mode()
	selected = mode_to_idx(curr_mode)
	mode_changed.emit(curr_mode)
	
func get_current_mode() :
	var curr_mode = RenderScaleModeSetting.Value
	if curr_mode == Viewport.SCALING_3D_MODE_BILINEAR:
		if RenderScaleScaleSetting.Value == 1.0:
			return -1
	
	return curr_mode
