extends OptionButton

var locales = {
	"FR_LANGUAGE": "fr",
	"EN_LANGUAGE": "en",
}

var locale_str: String

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for locale in locales:
		add_item(locale)
	LanguageSetting.Changed.connect(_on_setting_value_changed)
	locale_str = LanguageSetting.Value
	update_ui()
	item_selected.connect(_on_item_selected)

func _on_setting_value_changed(sender, value) -> void:
	if sender == self:
		return
	locale_str = value
	update_ui()
	
func update_ui():
	selected = get_language_key(locale_str)

func get_language_key(locale: String) -> int:
	var idx = locales.values().find(locale)
	if idx != -1:
		return idx
	
	var locale_values = locales.values()
	for i in range(locale_values.size()):
		if locale.begins_with(locale_values[i]):
			return i
		if locale_values[i].begins_with(locale):
			return i
	
	return -1
		
			

func _on_item_selected(index: int) -> void:
	var locale = locales[get_item_text(index)]
	LanguageSetting.GdTryUpdateValue(self, locale)
