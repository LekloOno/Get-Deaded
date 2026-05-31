extends Control

func _ready():
	# a quirky fix, to fix for real later
	hidden.connect(_on_hidden)
	UserSettingsServer.Abort()

func _on_hidden() -> void:
	UserSettingsServer.Abort()
 
func _on_apply_pressed() -> void:
	UserSettingsServer.Save()
