extends OptionButton

var modes = {
	"FULLSCREEN_DISPLAY_MODE": DisplayServer.WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
	"BORDERLESS_DISPLAY_MODE": DisplayServer.WindowMode.WINDOW_MODE_FULLSCREEN,
	"WINDOWED_DISPLAY_MODE": DisplayServer.WindowMode.WINDOW_MODE_WINDOWED,
}

func display_mode_to_key(mode: DisplayServer.WindowMode) -> String:
	match mode:
		DisplayServer.WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN: return "FULLSCREEN_DISPLAY_MODE"
		DisplayServer.WindowMode.WINDOW_MODE_FULLSCREEN: return "BORDERLESS_DISPLAY_MODE"
		DisplayServer.WindowMode.WINDOW_MODE_WINDOWED: return "WINDOWED_DISPLAY_MODE"
	return "BORDERLESS_DISPLAY_MODE"

func _ready() -> void:
	for mode in modes:
		add_item(mode)
		
	DisplayModeSetting.Changed.connect(_on_setting_value_changed)
	update_ui(DisplayModeSetting.Value)

func set_mode(mode: DisplayServer.WindowMode):
	DisplayModeSetting.GdTryUpdateValue(self, mode)

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	set_mode(modes[key])

func _on_setting_value_changed(sender, value):
	if sender == self:
		return;
	update_ui(value)
	
func update_ui(mode):
	var mode_str = display_mode_to_key(mode)
	selected = modes.keys().find(mode_str)
