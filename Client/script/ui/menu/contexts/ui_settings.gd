extends Node

func _ready():
	# a quirky fix, to fix for real later
	UserSettingsServer.Abort()

func _on_hidden() -> void:
	UserSettingsServer.Abort()
 
func _on_apply_pressed() -> void:
	UserSettingsServer.Save()
