extends Button

@export var resolution_options: UI_ResolutionPicker
@export var x_value: LineEdit
@export var y_value: LineEdit

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pressed.connect(_on_pressed)

func _on_pressed() :
	var res = retrieve_resolution()
	resolution_options.add_resolution(res)

func retrieve_resolution() -> Vector2i:
	var x = x_value.text
	if x == "":
		x = 1280
	else:
		x = int(x)
		
	var y = y_value.text
	if y == "" :
		y = 720
	else:
		y = int(y)
	
	return Vector2i(x, y)
