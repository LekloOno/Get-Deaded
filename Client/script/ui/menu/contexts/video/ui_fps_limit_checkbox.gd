extends CheckBox

func _ready() -> void:
	toggled.connect(_on_toggled)
	LimitFpsSetting.Changed.connect(_on_setting_value_changed)
	update_ui(LimitFpsSetting.Value)

func update_ui(limit):
	button_pressed = limit

func _on_setting_value_changed(sender, value):
	if sender == self:
		return;
	update_ui(value)

func _on_toggled(val: bool):
	LimitFpsSetting.GdTryUpdateValue(self, val)
