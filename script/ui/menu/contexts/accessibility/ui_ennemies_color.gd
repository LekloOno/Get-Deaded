extends ColorPickerButton

var enemies_color: Color

func _ready() -> void:
	color_changed.connect(_on_color_changed)
	enemies_color = EnemiesColorSetting.Value
	EnemiesColorSetting.Changed.connect(_on_setting_value_changed)
	update_ui()

func _on_setting_value_changed(sender, value) -> void:
	if sender == self:
		return
	
	enemies_color = value
	update_ui()
	
func update_ui():
	color = enemies_color

func _on_color_changed(new_color: Color) -> void:
	EnemiesColorSetting.GdTryUpdateValue(self, new_color)
