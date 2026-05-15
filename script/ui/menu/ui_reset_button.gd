extends Node

@export var section: String
@export var key: String

var button: Button
var target: Control

func _ready() -> void:
	button = get_child(0)
	target = get_parent().get_child(0)
	button.pressed.connect(reset)

func reset():
	CONF_UserSettingsLoader.Reset(section, key)
	target.visibility_changed.emit()
