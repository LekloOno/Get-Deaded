extends HSplitContainer

@export var camera_settings: PC_Settings

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

func _ready() -> void:
	init_children()
	for dpi in dpis:
		dpi_option.add_item(dpi)
	
	dpi_option.selected = -1
		
	visibility_changed.connect(_on_visibility_changed)
	
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
		
	dpi_edit.text = str(camera_settings.Dpi)

func set_dpi(dpi: int):
	camera_settings.Dpi = dpi
	CONF_UserSettingsLoader.RegisterControlSetting("dpi", dpi)


func _on_line_edit_value_applied(value: float) -> void:
	set_dpi(int(value))

func _on_option_button_item_selected(index: int) -> void:
	var dpi_str = dpi_option.get_item_text(index)
	if dpi_str.is_valid_float():
		set_dpi(int(dpi_str))
