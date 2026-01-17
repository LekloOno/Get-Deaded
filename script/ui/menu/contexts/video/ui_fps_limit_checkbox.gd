extends CheckBox

func _ready() -> void:
	visibility_changed.connect(_on_visibility_changed)
	toggled.connect(_on_toggled)

func _on_visibility_changed() -> void:
	if !visible:
		return
	
	button_pressed = Engine.max_fps != 0

func _on_toggled(val: bool):
	if !val:
		Engine.max_fps = 0
