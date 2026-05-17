extends OptionButton

var line_edit

func _ready() -> void:
	line_edit = get_line_edit()
	item_selected.connect(_on_item_selected)
	
func get_line_edit() -> LineEdit :
	for sibling in get_parent().get_children():
		if sibling is LineEdit:
			return sibling
	return null
	
func _on_item_selected(index: int):
	line_edit.text = get_item_text(index)
	selected = -1
