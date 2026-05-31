extends Control

func _ready() -> void:
	update(SprintModeSetting.Value)
	SprintModeSetting.Changed.connect(_on_setting_changed)

func _on_setting_changed(_sender, new_value):	
	update(new_value)

func update(mode: int):
	visible = mode == 0
