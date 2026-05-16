extends Button

@export var action: String
@export var input_index: int

var input_event: InputEvent

var bind: EditableInputEvent

var waiting_release = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	toggle_mode = true
	focus_mode = Control.FOCUS_NONE
	set_process_input(false)
	var setting = KeyBindingSettingsManager.GetBinding(action)
	if setting:
		bind = setting.GdTryGetBind(input_index)
		bind.Changed.connect(_on_bind_value_changed)
		input_event = bind.InputEvent
		update_ui()
		pressed.connect(_on_pressed)

func _on_bind_value_changed(sender, event: InputEvent):
	input_event = event
	update_ui()
	if sender == self:
		set_process_input(false)
		set_pressed_no_signal(false)
		return
	
func update_ui():
	if input_event is InputEventMouseButton:
		text = "mouse_" + str(input_event.button_index)
	elif input_event is InputEventKey:
		text = OS.get_keycode_string(input_event.physical_keycode)

func _on_pressed():
	text = "listening ..."
	waiting_release = true
	set_process_input(true)

func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if waiting_release && event.is_released() :
			waiting_release = false
			return
		bind.TryUpdateValue(self, event)
	
	if event is InputEventKey :
		bind.TryUpdateValue(self, event)
