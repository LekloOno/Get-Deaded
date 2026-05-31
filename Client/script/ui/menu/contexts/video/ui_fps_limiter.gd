extends Control

func _ready() -> void:
	LimitFpsSetting.Changed.connect(_on_setting_value_changed)
	update_ui(LimitFpsSetting.Value)

func _on_setting_value_changed(sender, value):
	if sender == self:
		return;
	update_ui(value)
	
func update_ui(value):
	visible = value
