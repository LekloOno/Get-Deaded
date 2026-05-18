extends Control

@onready var label = $Label
@onready var confirm_button = $ConfirmButton
@onready var cancel_button = $CancelButton

var pending_action: Callable

func ready():
	confirm_button.pressed.connect(_on_confirm_button_pressed)
	cancel_button.pressed.connect(_on_cancel_button_pressed)
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed():
	if (!is_visible_in_tree()):
		_on_cancel_button_pressed()

func ask(message: String, action: Callable):
	label.text = message
	pending_action = action
	visible = true

func _on_confirm_button_pressed():
	visible = false

	if pending_action.is_valid():
		pending_action.call()

	pending_action = Callable()

func _on_cancel_button_pressed():
	visible = false
	pending_action = Callable()
