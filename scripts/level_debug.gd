extends Node

const LEVELS := [
	"res://scenes/garden.tscn",
	"res://scenes/level_2.tscn",
	"res://scenes/level_3.tscn",
]


func _unhandled_input(event: InputEvent) -> void:
	if not event is InputEventKey or not event.pressed or event.echo:
		return
	var index := -1
	match event.keycode:
		KEY_1:
			index = 0
		KEY_2:
			index = 1
		KEY_3:
			index = 2
	if index < 0:
		return
	var path := LEVELS[index]
	var current := get_tree().current_scene
	if current != null and current.scene_file_path == path:
		return
	get_viewport().set_input_as_handled()
	get_tree().change_scene_to_file(path)
