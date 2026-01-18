extends OptionButton

class_name UI_ResolutionPicker

var resolutions = {
	"3840x2160": Vector2i(3840,2160),
	"2560x1440": Vector2i(2560,1440),
	"1920x1080": Vector2i(1920,1080),
	"1600x900": Vector2i(1600,900),
	"1440x900": Vector2i(1440,900),
	"1366x768": Vector2i(1366,768),
	"1280x720": Vector2i(1280,720),
	"1024x600": Vector2i(1024,600),
	"800x600": Vector2i(800,600)
}

var curr_res: Vector2i

func _ready() -> void:
	for resolution in resolutions:
		add_item(resolution)
		
	if DisplayServer.window_get_mode() == DisplayServer.WindowMode.WINDOW_MODE_WINDOWED:
		curr_res = get_window().size
	else:
		curr_res = get_window().content_scale_size
	

func _on_visibility_changed() -> void:
	if !visible:
		return
	
	var window_size_str = str(
		curr_res.x,
		"x",
		curr_res.y
	)
	
	var resolution_id = resolutions.keys().find(window_size_str)
	selected = resolution_id
		
func set_window_size() :
	if DisplayServer.window_get_mode() == DisplayServer.WindowMode.WINDOW_MODE_WINDOWED:
		get_window().set_size(curr_res)
	get_window().content_scale_size = curr_res

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	var resolution = resolutions[key]
	curr_res = resolution
	set_window_size()
	
func add_resolution(resolution: Vector2i):
	var res_str = str(resolution.x, "x", resolution.y)
	if resolutions.has(res_str):
		return
	
	resolutions[res_str] = resolution
	add_item(res_str)
