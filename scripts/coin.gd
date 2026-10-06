extends Area2D

@export var value := 1


func _ready() -> void:
	body_entered.connect(_on_body_entered)


func _on_body_entered(body: Node) -> void:
	if body.is_in_group("player") and body.has_method("gain_coins"):
		body.gain_coins(value)
		queue_free()
