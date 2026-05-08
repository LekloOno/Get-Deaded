extends HSlider

@export var camera_settings: PC_Settings

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	value_changed.connect(_on_value_changed)
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	value = camera_settings.HorizontalFov
	value_changed.emit(value)

func _on_value_changed(new_value: float) -> void:
	camera_settings.HorizontalFov = new_value
