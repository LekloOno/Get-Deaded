extends Control

@export var confirm_dialog : UI_ConfirmDialog

func _ready():
	# a quirky fix, to fix for real later
	#hidden.connect(_on_hidden)
	UserSettingsServer.Abort()
	
func close_menu() -> bool:
	if (UserSettingsServer.HasBeenModified):
		confirm_dialog.ask("CONFIRM_ABORT_SETTINGS_MESSAGE", abort)
		return false
	return true
 
func _on_apply_pressed() -> void:
	if (UserSettingsServer.HasBeenModified):
		confirm_dialog.ask("CONFIRM_SAVE_SETTINGS_MESSAGE", save)
	
func abort():
	UserSettingsServer.Abort()
	
func save():
	UserSettingsServer.Save()
