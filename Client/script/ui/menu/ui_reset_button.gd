extends Button

@export var section: String
@export var key: String

var button: Button

func _ready() -> void:
	hide()
	button = get_child(0)
	button.pressed.connect(reset)
	DisplayModeSetting.Changed.connect(_on_setting_value_changed)

func reset():
	DisplayModeSetting.Reset()
	
func _on_setting_value_changed(sender, value):
	visible = DisplayModeSetting.GdDefault() != DisplayModeSetting.Value
