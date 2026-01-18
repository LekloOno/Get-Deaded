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
	visibility_changed.connect(_on_visibility_changed)

func mode_to_idx(mode) -> int:
	return modes.values().find(mode)

func _on_item_selected(index: int) -> void:
	var mode = modes[get_item_text(index)]
	
	if mode == -1 :
		get_tree().root.scaling_3d_mode = Viewport.SCALING_3D_MODE_BILINEAR
	else :
		get_tree().root.scaling_3d_mode = mode
		
	mode_changed.emit(mode)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	var curr_mode = get_current_mode()
	selected = mode_to_idx(curr_mode)
	
func get_current_mode() :
	var curr_mode = get_tree().root.scaling_3d_mode
	if curr_mode == Viewport.SCALING_3D_MODE_BILINEAR:
		if get_tree().root.scaling_3d_scale == 1:
			return -1
	
	return curr_mode
