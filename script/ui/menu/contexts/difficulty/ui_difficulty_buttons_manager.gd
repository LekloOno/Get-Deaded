extends Node

signal difficulty_selected()

func _ready() -> void:
	for node in get_children() :
		if node is Button:
			node.pressed.connect(_on_pressed)

func _on_pressed():
	difficulty_selected.emit()
