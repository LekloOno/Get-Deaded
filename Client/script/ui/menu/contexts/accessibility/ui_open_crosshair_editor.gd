extends Button

@export var menu: UI_EscapeMenu
@export var editor: Control

func _ready() -> void:
	pressed.connect(_on_pressed)

func _on_pressed():
	menu.Enter(editor)
