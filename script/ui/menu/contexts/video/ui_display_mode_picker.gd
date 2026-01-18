extends OptionButton

signal switched_mode()

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

func set_mode(mode: DisplayServer.WindowMode):
	DisplayServer.window_set_mode(mode)
	switched_mode.emit()

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	set_mode(modes[key])


func _on_visibility_changed() -> void:
	if !visible:
		return
		
	var mode_str = display_mode_to_key(DisplayServer.window_get_mode())
	
	selected = modes.keys().find(mode_str)
