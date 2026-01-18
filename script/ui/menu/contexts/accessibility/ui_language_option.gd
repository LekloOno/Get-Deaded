extends OptionButton

var locales = {
	"FR_LANGUAGE": "fr",
	"EN_LANGUAGE": "en",
}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for locale in locales:
		add_item(locale)
	visibility_changed.connect(_on_visibility_changed)
	item_selected.connect(_on_item_selected)

func _on_visibility_changed() -> void:
	if !visible:
		return
		
	var locale_str = TranslationServer.get_locale()
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
	var key = get_item_text(index)
	TranslationServer.set_locale(locales[key])
