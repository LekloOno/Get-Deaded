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
	StretchModeSetting.Changed.connect(_on_setting_value_changed)
	update_ui(StretchModeSetting.Value)

func set_mode(mode: Window.ContentScaleAspect):
	StretchModeSetting.GdTryUpdateValue(self, mode)


func _on_setting_value_changed(sender, value):
	if sender == self:
		return;
	update_ui(value)
	
func update_ui(mode):
	selected = aspect_ratio_to_idx(mode)
	
	
	

func aspect_ratio_to_idx(aspect_ratio) -> int:
	return aspect_ratios.values().find(aspect_ratio)

func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	var mode = aspect_ratios[key]
	StretchModeSetting.GdTryUpdateValue(self, mode)
