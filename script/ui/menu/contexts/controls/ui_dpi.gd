extends HSplitContainer

var dpis = {
	"400": 400,
	"800": 800,
	"1000": 1000,
	"1600": 1600,
	"2400": 2400,
	"3200": 3200,
	"6400": 6400,
}

var dpi_option: OptionButton
var dpi_edit: LineEdit

var camera_control: PC_Control

func _ready() -> void:
	init_children()
	for dpi in dpis:
		dpi_option.add_item(dpi)
		
	visibility_changed.connect(_on_visibility_changed)
	camera_control = get_camera_control()

func get_camera_control() -> PC_Control:
	var parent = get_viewport().get_camera_3d()
	while parent != null:
		if parent is PC_Control :
			return parent
		parent = parent.get_parent()
	return null
	
func init_children():
	for child in get_children():
		if child is OptionButton:
			dpi_option = child
		elif child is LineEdit:
			dpi_edit = child
		else: 
			continue
		
		if dpi_option && dpi_edit:
			return

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	dpi_option.selected = dpis.keys().find(str(camera_control.Dpi))
	dpi_edit.text = str(camera_control.Dpi)

func _on_line_edit_value_applied(value: float) -> void:
	camera_control.Dpi = int(value)

func _on_option_button_item_selected(index: int) -> void:
	var dpi_str = dpi_option.get_item_text(index)
	if dpi_str.is_valid_float():
		camera_control.Dpi = int(dpi_str)
