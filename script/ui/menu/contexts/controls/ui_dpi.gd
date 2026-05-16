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

var dpi: int

func _ready() -> void:
	init_children()
	for dpi_opts in dpis:
		dpi_option.add_item(dpi_opts)
	
	dpi_option.selected = -1
	
	DpiSetting.Changed.connect(_on_setting_value_changed)
	dpi = DpiSetting.Value
	update_ui()
	
func _on_setting_value_changed(sender, new_value):
	if (sender == self):
		return
		
	dpi = new_value
	update_ui()
	
func update_ui():
	dpi_edit.text = str(dpi)

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

func set_dpi():
	DpiSetting.GdTryUpdateValue(self, dpi)


func _on_line_edit_value_applied(value: float) -> void:
	dpi = int(value)
	set_dpi()

func _on_option_button_item_selected(index: int) -> void:
	var dpi_str = dpi_option.get_item_text(index)
	if dpi_str.is_valid_float():
		dpi = int(dpi_str)
		set_dpi()
