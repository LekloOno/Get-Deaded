extends Label

@export var character_body: CharacterBody3D

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
    var speed = (character_body.velocity * Vector3(1, 0, 1)).length()
    speed *= 3.6
    set_text("%d " % speed)