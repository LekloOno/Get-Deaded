extends ColorPickerButton

@export var crosshair_background: PanelContainer

var _panel_stylebox: StyleBoxFlat


func _ready() -> void:
	if not crosshair_background:
		return

	var stylebox := crosshair_background.get_theme_stylebox("panel")

	if stylebox is StyleBoxFlat:
		_panel_stylebox = stylebox.duplicate()
	else:
		_panel_stylebox = StyleBoxFlat.new()

	crosshair_background.add_theme_stylebox_override("panel", _panel_stylebox)

	color = _panel_stylebox.bg_color
	color_changed.connect(_on_color_changed)


func _on_color_changed(new_color: Color) -> void:
	if _panel_stylebox:
		_panel_stylebox.bg_color = new_color
