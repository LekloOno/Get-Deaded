extends OptionButton

var aspect_ratios = {
	"KEEP_ASPECT_RATIO": Window.CONTENT_SCALE_ASPECT_KEEP,
	"KEEP_WIDTH_ASPECT_RATIO": Window.CONTENT_SCALE_ASPECT_KEEP_WIDTH,
	"KEEP_HEIGHT_ASPECT_RATIO": Window.CONTENT_SCALE_ASPECT_KEEP_HEIGHT,
	"STRETCH_ASPECT_RATIO": Window.CONTENT_SCALE_ASPECT_IGNORE,
}

func _ready() -> void:
	for ratio in aspect_ratios:
		add_item(ratio)
	item_selected.connect(_on_item_selected)
	visibility_changed.connect(_on_visibility_changed)

func aspect_ratio_to_idx(aspect_ratio) -> int:
	return aspect_ratios.values().find(aspect_ratio)

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	get_tree().root.content_scale_aspect = aspect_ratios[key]
	


func _on_visibility_changed() -> void:
	if !visible:
		return
		
	selected = aspect_ratio_to_idx(get_tree().root.content_scale_aspect)
