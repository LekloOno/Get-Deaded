extends Button

@export var confirm_dialog: UI_ConfirmDialog

signal left()

func _ready() -> void:
	pressed.connect(_on_button_pressed)
	
func _on_button_pressed() -> void:
	confirm_dialog.ask("CONFIRM_LEAVE_MESSAGE", leave)

func leave():
	left.emit()
