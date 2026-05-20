extends ProgressBar

@export var picker: GL_Picker

@export var fade_out_time: float = 0.5

var fill_tween: Tween
var opacity_tween: Tween

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	visible = false
	picker.SlowMoStarted.connect(_on_slow_mo_started)
	picker.SlowMoFadeInStarted.connect(_on_slow_mo_fade_in_started)
	picker.EffectsCleansed.connect(_on_effects_cleansed)

func _on_effects_cleansed():
	if opacity_tween:
		opacity_tween.kill()
		
	if fill_tween:
		fill_tween.kill()
		
	visible = false

func _on_slow_mo_fade_in_started(duration: float):
	value = 1
	visible = true
	modulate.a = 0
	
	if opacity_tween:
		opacity_tween.kill()
		
	opacity_tween = create_tween()
	opacity_tween.tween_property(self, "modulate:a", 1, duration)


func _on_slow_mo_started(duration: float):
	if fill_tween:
		fill_tween.kill()
		
	fill_tween = create_tween()
	fill_tween.tween_property(self, "value", 0, duration)
	
	await fill_tween.finished
	
	if opacity_tween:
		opacity_tween.kill()
		
	opacity_tween = create_tween()
	opacity_tween.tween_property(self, "modulate:a", 0, fade_out_time)
	
	await opacity_tween.finished
	
	visible = false
