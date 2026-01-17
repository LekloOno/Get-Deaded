extends HSplitContainer

var edit: LineEdit
var slider: Slider

var camera_control: PC_Control

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	init_children()
	visibility_changed.connect(_on_visibility_changed)
	camera_control = get_camera_control()

func get_camera_control() -> PC_Control:
	var parent = get_viewport().get_camera_3d()
	while parent != null:
		if parent is PC_Control :
			return parent
		parent = parent.get_parent()
	return null

func _on_visibility_changed() -> void:
	if !visible:
		return
	
	edit.text = str(camera_control.CmPer360)
	slider.value = camera_control.CmPer360

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
	camera_control.CmPer360 = value

func _on_line_edit_value_applied(value: float) -> void:
	camera_control.CmPer360 = value
