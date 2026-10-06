extends Sprite2D


func _ready() -> void:
	if texture == null:
		texture = preload("res://assets/sprites/tom_splotch.png")
	var tween := create_tween()
	tween.tween_property(self, "modulate:a", 0.0, 0.8)
	tween.finished.connect(queue_free)
