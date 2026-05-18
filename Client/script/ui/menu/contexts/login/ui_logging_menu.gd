extends Control

@export var menu: UI_EscapeMenu
@export var confirm_dialog: UI_ConfirmDialog
@export var confirm_button: Button
@export var login_field: LineEdit
@export var password_field: LineEdit

@export var login_text: String = "LOG_IN_BUTTON"
@export var register_text: String = "REGISTER_BUTTON"

enum LOGIN_MODE {
	NONE,
	LOGIN,
	REGISTER,
	OK,
	CONFIRM,
	ERROR,
}

var mode = LOGIN_MODE.NONE;
var tried_mode = LOGIN_MODE.NONE;

func _ready() -> void:
	password_field.secret = true
	confirm_button.pressed.connect(_on_confirm)
	
	login_field.text_submitted.connect(_on_submit)
	password_field.text_submitted.connect(_on_submit)
	
	visibility_changed.connect(_on_visibility_changed)
	
	ApiGodotGlue.LoginFinished.connect(_on_login_finished)
	ApiGodotGlue.RegisterFinished.connect(_on_register_finished)
	
func _on_visibility_changed():
	if (is_visible_in_tree()):
		return
	if (mode == LOGIN_MODE.CONFIRM):
		return
		
	mode = LOGIN_MODE.NONE
	tried_mode = LOGIN_MODE.NONE
		
	
func start_login():
	menu.Enter(self)
	mode = LOGIN_MODE.LOGIN
	tried_mode = mode
	update_ui()

func start_register():
	menu.Enter(self)
	mode = LOGIN_MODE.REGISTER
	tried_mode = mode
	update_ui()
	
func update_ui():
	confirm_button.text = button_text()
	confirm_button.disabled = mode == LOGIN_MODE.NONE

func button_text() -> String:
	match mode:
		LOGIN_MODE.LOGIN:
			return "LOG_IN_BUTTON"
		LOGIN_MODE.REGISTER, LOGIN_MODE.CONFIRM:
			return "REGISTER_BUTTON"
		LOGIN_MODE.OK:
			return "OK_BUTTON"
		LOGIN_MODE.ERROR:
			return "ERROR_BUTTON"
		_:
			return "..."

func _on_submit(_text: String):
	_on_confirm()

func _on_confirm():
	match mode:
		LOGIN_MODE.NONE:
			pass
		LOGIN_MODE.OK:
			menu.ExitCurrent()
		LOGIN_MODE.LOGIN:
			login()
		LOGIN_MODE.REGISTER, LOGIN_MODE.CONFIRM:
			mode = LOGIN_MODE.CONFIRM
			confirm_dialog.ask("CONFIRM_REGISTER_MESSAGE", register)
		LOGIN_MODE.ERROR:
			mode = tried_mode
			update_ui()

func login():
	mode = LOGIN_MODE.NONE
	update_ui()
			
	var username = login_field.text.strip_edges()
	var password = password_field.text

	if username.is_empty() or password.is_empty():
		_show_error("Username or password missing")
		return

	ApiGodotGlue.Login(username, password)


func register():
	mode = LOGIN_MODE.NONE
	update_ui()
	
	var username = login_field.text.strip_edges()
	var password = password_field.text

	if username.is_empty() or password.is_empty():
		_show_error("Username or password missing")
		return

	ApiGodotGlue.Register(username, password)
	
func _on_login_finished(result: Dictionary) -> void:
	_handle_auth_result(result, true)


func _on_register_finished(result: Dictionary) -> void:
	_handle_auth_result(result, false)


func _handle_auth_result(result: Dictionary, is_login: bool) -> void:
	var success: bool = result.get("success", false)
	var error: String = result.get("error", "Unknown")
	var message: String = result.get("message", "")

	if success:
		var token: String = result.get("token", "")
		print("Auth success. Token:", token)
		mode = LOGIN_MODE.OK
		update_ui()
	else:
		_show_error("%s: %s" % [error, message])

func _show_error(msg: String) -> void:
	print("AUTH ERROR:", msg)
	mode = LOGIN_MODE.ERROR
	update_ui()
