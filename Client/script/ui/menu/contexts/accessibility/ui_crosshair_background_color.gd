extends ColorPickerButton

@export var crosshair_background: StyleBoxFlat

func _ready() -> void:
	if not crosshair_background:
		return

	color = crosshair_background.bg_color
	color_changed.connect(_on_color_changed)

func _on_color_changed(new_color: Color) -> void:
	if crosshair_background:
		crosshair_background.bg_color = new_color
