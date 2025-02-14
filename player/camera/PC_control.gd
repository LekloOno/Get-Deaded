extends Camera3D

@export var sensitivity: float = 2.8

@export var body: Node3D = null

var real_sens: float = sensitivity/6500;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		print(event.relative)
		body.rotate_y(-event.relative.x * real_sens)
		self.rotate_x(-event.relative.y * real_sens)
		self.rotation.x = clamp(self.rotation.x, deg_to_rad(-90), deg_to_rad(90))

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
