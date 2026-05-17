extends HSlider

@export var bus_name: String
@export var override_min_max_value: bool = true


var linear_volume: float
var bus_setting: AudioBusSetting

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	bus_setting = AudioBusSettingsManager.GetBus(bus_name)
	bus_setting.Changed.connect(_on_setting_value_changed)
	value = bus_setting.Value
	update_ui()
	
	value_changed.connect(_on_value_changed)
	
	if override_min_max_value:
		min_value = 0
		max_value = 100

func _on_setting_value_changed(sender, new_value):
	if (sender == self):
		return
	linear_volume = new_value
	update_ui()
	
func update_ui():
	value = linear_volume * max_value

func _on_value_changed(linear_value: float) -> void:
	linear_volume = linear_value
	bus_setting.GdTryUpdateValue(self, linear_value/max_value)
