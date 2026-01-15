extends LineEdit

@export var allow_float: bool = true
@export var allow_negative: bool = true

var slider: Slider

@export var clamp_value: bool
@export var min_value: float
@export var max_value: float

signal value_applied(value: float)

var last_value = 0.
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for sibling in get_parent().get_children():
		if sibling is Slider:
			slider = sibling
			clamp_value = true
			min_value = slider.min_value
			max_value = slider.max_value
			value_applied.connect(slider.set_value_no_signal)
			slider.value_changed.connect(_on_slider_value_changed)
			break
			
	text_submitted.connect(_on_text_changed)

func _on_text_changed(new_text: String) -> void:
	var new_value
	match allow_float:
		true: match new_text.is_valid_float():
			true: new_value = float(new_text)
			false: new_value = last_value
		false: match new_text.is_valid_int():
			true: new_value = int(new_text)
			false: new_value = last_value
			
	if !allow_negative:
		new_value = max(0, new_value)
			
	if clamp_value:
		if slider is Slider:
			new_value = clamp(new_value, slider.min_value, slider.max_value)
		else:
			new_value = clamp(new_value, min_value, max_value)
		
	text = str(new_value)
	apply_value(new_value)
	
func _on_slider_value_changed(value: float):
	last_value = value
	text = str(value)
			
func apply_value(value: float):
	last_value = value
	value_applied.emit(value)
