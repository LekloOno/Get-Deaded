extends LineEdit

@export var allow_float: bool = true
@export var allow_negative: bool = true
# -1 means no rounding
@export var decimals: int = 2

var slider: Slider

@export var link_to_slider: bool = true
@export var clamp_to_slider: bool = true

@export var clamp_value: bool
@export var min_value: float
@export var max_value: float

signal value_applied(value: float)

var last_value = 0.

func _ready() -> void:
	if link_to_slider:
		slider = get_slider()
		if slider:
			init_to_slider()
			
	text_submitted.connect(_on_text_changed)

func get_slider() -> Slider:
	for sibling in get_parent().get_children():
		if sibling is Slider:
			return sibling
	return null
	
func init_to_slider() -> void:
	if clamp_to_slider:
		clamp_value = true
		min_value = slider.min_value
		max_value = slider.max_value
		
	value_applied.connect(slider.set_value_no_signal)
	slider.value_changed.connect(_on_slider_value_changed)

func _on_text_changed(new_text: String) -> void:
	var new_value = formated_value(new_text)
	text = str(new_value)
	apply_value(new_value)
	
func apply_value_no_signal(value: float):
	text = str(formated_value(str(value)))
	last_value = value
	
func formated_value(new_text: String) :
	var new_value
	if new_text.is_valid_float():
		var float_val = float(new_text)
		match allow_float:
			true: new_value = round_to_decimals(float_val)
			false: new_value = roundi(float_val)
			
	if !allow_negative:
		new_value = max(0, new_value)
			
	if clamp_value:
		new_value = clamp(new_value, min_value, max_value)
	return new_value
	

func round_to_decimals(value: float) -> float:
	if decimals == -1:
		return value
	
	var offset = pow(10, decimals)
	return round(value*offset)/offset

func _on_slider_value_changed(value: float):
	last_value = value
	match allow_float:
		true: text = str(value)
		false: text = str(int(value))
			
func apply_value(value: float):
	last_value = value
	value_applied.emit(value)
