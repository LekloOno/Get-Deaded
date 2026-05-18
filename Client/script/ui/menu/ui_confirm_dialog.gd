extends Control

class_name UI_ConfirmDialog

@export var menu: UI_EscapeMenu 
@export var label: Label
@export var confirm_button: Button
@export var cancel_button: Button

var pending_action: Callable

func _ready():
	confirm_button.pressed.connect(_on_confirm_button_pressed)
	cancel_button.pressed.connect(_on_cancel_button_pressed)
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed():
	if (!is_visible_in_tree()):
		free_action()

func ask(message: String, action: Callable):
	menu.Enter(self)
	label.text = message
	pending_action = action

func _on_confirm_button_pressed():
	if pending_action.is_valid():
		pending_action.call()
	
	menu.ExitCurrent()
	free_action()

func free_action():
	pending_action = Callable()

func _on_cancel_button_pressed():
	menu.ExitCurrent()
	free_action()
