extends Node

@export var locations: Dictionary[RemoteTransform3D, float]
@export var camera: Camera3D

var index: int = 0
var timer: Timer
var current_location: RemoteTransform3D

func _ready() -> void:
	timer = Timer.new()
	add_child(timer)
	current_location = locations.keys()[0]
	timer.timeout.connect(swap)
	swap()
	
func swap():
	current_location.remote_path = ^""
	current_location = locations.keys()[index]
	current_location.remote_path = camera.get_path()
	camera.reset_physics_interpolation()
	var time = locations.values()[index]
	index += 1
	index %= locations.size()
	timer.start(time)
	
