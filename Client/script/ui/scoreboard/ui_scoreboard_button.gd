extends Button

@export var menu: UI_EscapeMenu
@export var score_board: UI_ScoreBoardManager

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pressed.connect(_on_pressed)

func _on_pressed():
	menu.Enter(score_board)
	score_board.Init()
