extends OptionButton

var modes = {
	"VSYNC_DISABLED": DisplayServer.VSYNC_DISABLED,
	"VSYNC_ENABLED": DisplayServer.VSYNC_ENABLED,
	"VSYNC_ADAPTATIVE": DisplayServer.VSYNC_ADAPTIVE,
	"VSYNC_MAILBOX": DisplayServer.VSYNC_MAILBOX,
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
	print(mode)
	DisplayServer.window_set_vsync_mode(mode)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	var curr_mode = get_current_mode()
	selected = mode_to_idx(curr_mode)
	
func get_current_mode() :
	return DisplayServer.window_get_vsync_mode()
