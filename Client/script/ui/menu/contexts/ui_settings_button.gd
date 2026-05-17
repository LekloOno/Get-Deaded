extends Button

@export var settings_menu: Control;

signal enter_settings(menu: Control);

func _ready() -> void:
	pressed.connect(_on_button_pressed)
	
func _on_button_pressed() -> void:
	emit_signal("enter_settings", settings_menu);
