extends Control

@export var label: Control
@export var slider: Slider

var max_fps

func _ready() -> void:
	slider.value_changed.connect(_on_value_changed)
	visibility_changed.connect(_on_visibility_changed)
	
	max_fps = CONF_UserSettingsLoader.GetVideoSetting("max_fps")
	_initialize()

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	max_fps = CONF_UserSettingsLoader.GetVideoSetting("max_fps")
	
	if max_fps == 0:
		max_fps = 60
		
	slider.value = max_fps
	slider.value_changed.emit(max_fps)

func _initialize():
	var show_limiter = max_fps != 0 
	visible = show_limiter
	
	if label:
		label.visible = show_limiter

func _on_value_changed(new_value: float) -> void:
	max_fps = new_value
	Engine.max_fps = max_fps
	CONF_UserSettingsLoader.RegisterVideoSetting("max_fps", max_fps)
