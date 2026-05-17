extends Node
	
func _on_hidden() -> void:
	UserSettingsServer.Abort()
 
func _on_apply_pressed() -> void:
	UserSettingsServer.Save()
