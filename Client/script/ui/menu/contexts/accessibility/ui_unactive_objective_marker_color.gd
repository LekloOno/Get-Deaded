extends ColorPickerButton

var marker_color: Color

func _ready() -> void:
	color_changed.connect(_on_color_changed)
	marker_color = UnactiveObjectiveMarkerColorSetting.Value
	UnactiveObjectiveMarkerColorSetting.Changed.connect(_on_setting_value_changed)
	update_ui()

func _on_setting_value_changed(sender, value) -> void:
	if sender == self:
		return
	
	marker_color = value
	update_ui()
	
func update_ui():
	color = marker_color

func _on_color_changed(new_color: Color) -> void:
	UnactiveObjectiveMarkerColorSetting.GdTryUpdateValue(self, new_color)
