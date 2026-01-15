extends HSlider


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	value_changed.connect(_on_value_changed)
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	var camera = get_viewport().get_camera_3d()
	if camera is PC_DirectCamera:
		value = camera.BaseFov
	else:
		value = camera.fov
	value_changed.emit(value)

func _on_value_changed(new_value: float) -> void:
	var camera = get_viewport().get_camera_3d()
	
	if camera is PC_DirectCamera:
		camera.BaseFov = new_value
	else:
		camera.fov = new_value
