extends Node
	
func _on_hidden() -> void:
	CONF_UserSettingsLoader.Abort()

func _on_apply_pressed() -> void:
	CONF_UserSettingsLoader.Apply()
