extends Button

@export var custom_res: Control

func _ready() -> void:
	toggled.connect(_on_toggled)
	_on_toggled(button_pressed)

func _on_toggled(toggled_on: bool) -> void:
	custom_res.visible = toggled_on
	
	match toggled_on:
		true: text = "-"
		false: text = "+"
