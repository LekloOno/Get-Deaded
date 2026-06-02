extends Button

@export var confirm_dialog: UI_ConfirmDialog

signal surrendered()

func _ready() -> void:
	pressed.connect(_on_button_pressed)
	
func _on_button_pressed() -> void:
	confirm_dialog.ask("CONFIRM_SURRENDER_MESSAGE", surrender)

func surrender():
	surrendered.emit()
