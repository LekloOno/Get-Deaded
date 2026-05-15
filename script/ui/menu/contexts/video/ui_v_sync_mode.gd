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
	VsyncModeSetting.Changed.connect(_on_setting_value_changed)
	
func _on_setting_value_changed(sender, value):
	if sender == self:
		return;
	update_ui(value)
	
func update_ui(mode):
	selected = mode_to_idx(mode)
	
func mode_to_idx(mode) -> int:
	return modes.values().find(mode)

func _on_item_selected(index: int) -> void:
	var mode = modes[get_item_text(index)]
	VsyncModeSetting.GdTryUpdateValue(self, mode)
	
func get_current_mode() :
	return VsyncModeSetting.Value
