extends PanelContainer

@export var picker: GL_Picker
@export var style: StyleBoxFlat
@export var low_bpm: float = 35
@export var high_bpm: float = 110
@export var max_size: int = 45
@export var bpm_trans: Tween.TransitionType = Tween.TransitionType.TRANS_EXPO

var time_left: float
var curr_bpm: float
var pulse_time: float

var bpm_tween: Tween

var buff_timer: SceneTreeTimer

func _ready() -> void:
	picker.DamageBuffPicked.connect(_on_damage_buff_picked)
	picker.EffectsCleansed.connect(end_buff)
	add_theme_stylebox_override("panel", style)
	visible = false
	set_process(false)

func _on_damage_buff_picked(buff: GL_DamageBuffData):
	time_left = buff.Duration
	pulse_time = 0
	curr_bpm = low_bpm
	
	if (bpm_tween):
		bpm_tween.kill()
		
	bpm_tween = create_tween()
	bpm_tween.tween_property(self, "curr_bpm", high_bpm, buff.Duration).set_trans(bpm_trans).set_ease(Tween.EASE_IN)
	visible = true
	set_process(true)
	buff_timer = get_tree().create_timer(buff.Duration, true, true, false)
	buff_timer.timeout.connect(end_buff)

func set_radius(radius: int):
	style.border_width_top = radius
	style.border_width_bottom = radius
	style.border_width_left = radius
	style.border_width_right = radius

func _process(delta: float) -> void:
	var bpm_ratio = curr_bpm/low_bpm
	pulse_time += delta * bpm_ratio
	var radius = roundi(heart_pulse(pulse_time, low_bpm) * max_size)
	set_radius(radius)
	time_left -= delta
	
func heart_pulse(time: float, bpm: float) -> float:
	var freq = bpm / 60
	var s = sin(2 * PI * freq * time)
	return pow(max(0, s), 10)
	
func end_buff():
	if (bpm_tween):
		bpm_tween.kill()
		
	if (buff_timer && buff_timer.timeout.is_connected(end_buff)):
		buff_timer.timeout.disconnect(end_buff)
		
	visible = false
	set_process(false)
