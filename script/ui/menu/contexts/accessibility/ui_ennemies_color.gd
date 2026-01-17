extends ColorPickerButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	color_changed.connect(_on_color_changed)
	visibility_changed.connect(_on_visibility_changed)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	color = ConfHitColors.HitColors.Critical


func _on_color_changed(new_color: Color) -> void:
	ConfHitColors.HitColors.Critical = new_color
