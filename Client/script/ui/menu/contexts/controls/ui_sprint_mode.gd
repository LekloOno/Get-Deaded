extends Node

@onready var toggle_button : Button = $ToggleButton
@onready var hold_button : Button = $HoldButton
@onready var auto_button : Button = $AutoButton

var button_group := ButtonGroup.new()

func _ready() -> void:
	toggle_button.button_group = button_group
	hold_button.button_group = button_group
	auto_button.button_group = button_group
	button_group.pressed.connect(_on_group_pressed)
	
	var mode = SprintModeSetting.Value
	get_button(mode).button_pressed = true
	
	SprintModeSetting.Changed.connect(_on_setting_changed)
	
	

func _on_setting_changed(sender, new_value):
	if sender == self :
		return
	
	get_button(new_value).button_pressed = true

func _on_group_pressed(button):
	SprintModeSetting.GdTryUpdateValue(self, get_mode(button))

func get_button(mode: int) -> Button :
	match mode:
		0:
			return auto_button
		1:
			return hold_button
		2:
			return toggle_button
	return auto_button
	
func get_mode(button: Button) -> int :
	match button:
		auto_button:
			return 0
		hold_button:
			return 1
		toggle_button:
			return 2
	return 0
