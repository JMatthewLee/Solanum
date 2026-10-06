extends Node2D

signal enemies_changed(remaining: int)

const TITLE_SCENE := "res://scenes/title.tscn"
const CLEAR_FONT := preload("res://assets/fonts/dogicapixelbold.ttf")

@export var camera_limits := Rect2(0, 0, 480, 270)
@export_file("*.tscn") var next_scene := ""
@export var clear_message := "Level clear"

var _remaining := 0
var _clearing := false

@onready var _hud = $HUD
@onready var _player = $Actors/Tom
@onready var _camera: Camera2D = $Actors/Tom/Camera


func _ready() -> void:
	_apply_camera_limits()
	_player.hp_changed.connect(_hud.set_hp)
	_player.coins_changed.connect(_hud.set_coins)
	_hud.set_hp(_player.hp, 5)
	_hud.set_coins(_player.coins)
	for node in get_tree().get_nodes_in_group("enemies"):
		var enemy := node as GardenEnemy
		if enemy == null or enemy.died.is_connected(_on_enemy_died):
			continue
		enemy.died.connect(_on_enemy_died)
		_remaining += 1
	_hud.set_enemies(_remaining)
	$WakeTrigger.body_entered.connect(_on_wake_body)


func _apply_camera_limits() -> void:
	var bounds: Rect2 = camera_limits
	_camera.limit_left = int(bounds.position.x)
	_camera.limit_top = int(bounds.position.y)
	_camera.limit_right = int(bounds.end.x)
	_camera.limit_bottom = int(bounds.end.y)
	_camera.reset_smoothing()


func _on_enemy_died() -> void:
	_remaining = max(_remaining - 1, 0)
	enemies_changed.emit(_remaining)
	_hud.set_enemies(_remaining)
	if _remaining == 0:
		_begin_clear()


func _begin_clear() -> void:
	if _clearing:
		return
	_clearing = true
	_show_clear_banner()
	await get_tree().create_timer(1.6).timeout
	if not is_inside_tree():
		return
	var dest := next_scene if next_scene != "" else TITLE_SCENE
	get_tree().change_scene_to_file(dest)


func _show_clear_banner() -> void:
	var layer := CanvasLayer.new()
	layer.layer = 30
	var label := Label.new()
	label.text = clear_message
	label.position = Vector2(0, 118)
	label.size = Vector2(480, 32)
	label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	label.mouse_filter = Control.MOUSE_FILTER_IGNORE
	label.add_theme_font_override("font", CLEAR_FONT)
	label.add_theme_font_size_override("font_size", 16)
	label.add_theme_color_override("font_color", Color(0.93, 0.95, 0.86))
	label.add_theme_color_override("font_shadow_color", Color(0.11, 0.16, 0.09))
	label.add_theme_constant_override("shadow_offset_x", 1)
	label.add_theme_constant_override("shadow_offset_y", 1)
	layer.add_child(label)
	add_child(layer)


func _on_wake_body(body: Node) -> void:
	if not body.is_in_group("player"):
		return
	for node in get_tree().get_nodes_in_group("flyers"):
		var flyer := node as GardenEnemy
		if flyer:
			flyer.wake()
