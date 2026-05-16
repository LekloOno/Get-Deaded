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
		visibility_changed.connect(_on_visibility_change)

func _on_bind_value_changed(sender, event: InputEvent):
	input_event = event
	update_ui()
	if sender == self:
		cancel()
		return
	
func update_ui():
	if !input_event:
		text = "n/a"
	if input_event is InputEventMouseButton:
		text = "m" + str(input_event.button_index)
	elif input_event is InputEventKey:
		var label = DisplayServer.keyboard_get_label_from_physical(input_event.physical_keycode)
		text = OS.get_keycode_string(label)

func _on_pressed():
	text = "listening ..."
	waiting_release = true
	set_process_input(true)
	
func _on_visibility_change():
	if visible:
		input_event = bind.InputEvent
		update_ui()
	else:
		cancel()
		
func cancel():
	set_process_input(false)
	set_pressed_no_signal(false)

func _input(event: InputEvent) -> void:
	if event.is_action("ui_cancel"):
		get_viewport().set_input_as_handled()
		cancel()
		update_ui()
		return
	
	if event is InputEventMouseButton:
		if waiting_release && event.is_released() :
			waiting_release = false
			return
		send_bind(event)
	
	if event is InputEventKey :
		send_bind(event)
		
func send_bind(event: InputEvent):
	get_viewport().set_input_as_handled()
	if input_event == event:
		bind.TryUpdateValue(self, null)
	elif input_event && input_event.is_match(event):
		bind.TryUpdateValue(self, null)
	else:
		bind.TryUpdateValue(self, event)
