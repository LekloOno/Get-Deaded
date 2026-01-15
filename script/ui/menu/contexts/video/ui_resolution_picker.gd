extends OptionButton

var resolutions = {
	"3840x2160": Vector2i(3840,2160),
	"2560x1440": Vector2i(2560,1440),
	"1920x1080": Vector2i(1920,1080),
	"1366x768": Vector2i(1366,768),
	"1280x720": Vector2i(1280,720),
	"1440x900": Vector2i(1440,900),
	"1600x900": Vector2i(1600,900),
	"1024x600": Vector2i(1024,600),
	"800x600": Vector2i(800,600)
}

func _ready() -> void:
	for resolution in resolutions:
		add_item(resolution)

func _on_visibility_changed() -> void:
	if !visible:
		return
	
	var window_size = get_window_size()
	var window_size_str = str(
		window_size.x,
		"x",
		window_size.y
	)
	
	var resolution_id = resolutions.keys().find(window_size_str)
	selected = resolution_id
	
func get_window_size() -> Vector2i :
	if DisplayServer.window_get_mode() == DisplayServer.WindowMode.WINDOW_MODE_WINDOWED:
		return get_window().size
	else:
		return get_window().content_scale_size

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	var resolution = resolutions[key]
	
	if DisplayServer.window_get_mode() == DisplayServer.WindowMode.WINDOW_MODE_WINDOWED:
		get_window().set_size(resolution)
	else:
		get_window().content_scale_size = resolution
