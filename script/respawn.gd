class_name Respawn

extends Node3D
@export var player: PM_Controller

func _unhandled_input(event: InputEvent) -> void:
	if (event.is_action_pressed("revive")):
		player.global_position = global_position
		player.velocity = Vector3.ZERO
		player.RealVelocity = Vector3.ZERO
