extends Button

@export var label: Control
@export var input: Control

func _ready() -> void:
	toggled.connect(_on_toggled)
	_on_toggled(button_pressed)

func _on_toggled(toggled_on: bool) -> void:
	label.visible = toggled_on
	input.visible = toggled_on
	
	match toggled_on:
		true: text = "-"
		false: text = "+"
