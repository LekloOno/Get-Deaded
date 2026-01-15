extends HSlider

@export var bus_name: String
@export var override_min_max_value: bool = true

var bus_index: int

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	bus_index = AudioServer.get_bus_index(bus_name)
	value_changed.connect(_on_value_changed)
	visibility_changed.connect(_on_visibility_changed)
	
	if override_min_max_value:
		min_value = 0
		max_value = 100

func _on_visibility_changed() -> void:
	if visible:
		value = db_to_linear(
			AudioServer.get_bus_volume_db(bus_index)
		) * max_value

func _on_value_changed(linear_value: float) -> void:
	AudioServer.set_bus_volume_db(
		bus_index,
		linear_to_db(linear_value/max_value)
	)
