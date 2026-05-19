extends PanelContainer

@export var normal_color := Color(0.024, 0.024, 0.024, 0.192)
@export var hover_color := Color(0.4, 0.4, 0.8, 0.157)

func _ready():
	mouse_entered.connect(_on_hover)
	mouse_exited.connect(_on_exit)
	_apply_color(normal_color)

func _on_hover():
	_apply_color(hover_color)

func _on_exit():
	_apply_color(normal_color)

func _apply_color(c: Color):
	var style = StyleBoxFlat.new()
	style.bg_color = c
	add_theme_stylebox_override("panel", style)
