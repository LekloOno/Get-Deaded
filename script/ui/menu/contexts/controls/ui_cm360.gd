extends HSplitContainer

@export var camera_settings: PC_Settings

var edit: LineEdit
var slider: Slider

signal cm_360_changed(val: float)

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	init_children()
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed() -> void:
	if !visible:
		return
	
	slider.value = camera_settings.CmPer360
	cm_360_changed.emit(camera_settings.CmPer360)

func init_children():
	for child in get_children():
		if child is LineEdit:
			edit = child
		elif child is Slider:
			slider = child
		else: 
			continue
		
		if edit && slider:
			return

func _on_master_slider_value_changed(value: float) -> void:
	camera_settings.CmPer360 = value

func _on_line_edit_value_applied(value: float) -> void:
	camera_settings.CmPer360 = value
