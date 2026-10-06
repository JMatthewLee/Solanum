extends Control


func _ready() -> void:
	var start := $Center/Panel/VBox/Start as Button
	start.pressed.connect(_start)


func _start() -> void:
	get_tree().change_scene_to_file("res://scenes/garden.tscn")
