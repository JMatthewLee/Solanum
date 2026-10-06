extends GardenEnemy

const PPU := 32.0
const CHASE_RANGE := 16.0 * PPU
const ATTACK_RANGE := 0.5 * PPU
const EXPLODE_REACH := 24.0
const SPLOTCH_SCENE := preload("res://scenes/vfx/splotch.tscn")

enum Mode { ASLEEP, RIPEN, CHASE, EXPLODE }

@export var explosion_damage := 5

var _mode := Mode.ASLEEP
var _player
var _path_timer := 0.0


func _ready() -> void:
	speed = 2.0 * PPU
	super._ready()
	sprite.play("unripe")
	sprite.animation_finished.connect(_on_animation_finished)
	$Detector.body_entered.connect(_on_detector_body)


func take_damage(amount: int) -> void:
	wake()
	super.take_damage(amount)


func wake() -> void:
	if _mode != Mode.ASLEEP or not alive:
		return
	_mode = Mode.RIPEN
	sprite.play("ripen")


func _physics_process(delta: float) -> void:
	if not alive or _mode != Mode.CHASE:
		return
	_player = get_tree().get_first_node_in_group("player")
	if _player == null:
		return
	var distance := global_position.distance_to(_player.global_position)
	if distance <= ATTACK_RANGE:
		_explode()
		return
	if distance > CHASE_RANGE:
		velocity = Vector2.ZERO
		sprite.play("idle")
		move_and_slide()
		return
	if sprite.animation != "move":
		sprite.play("move")
	_path_timer -= delta
	var refresh := _path_timer <= 0.0
	if refresh:
		_path_timer = 0.25
	steer_toward(_player.global_position, refresh)


func _explode() -> void:
	_mode = Mode.EXPLODE
	velocity = Vector2.ZERO
	set_physics_process(false)
	sprite.play("explode")
	var player = get_tree().get_first_node_in_group("player")
	if player and global_position.distance_to(player.global_position) <= EXPLODE_REACH:
		player.receive_damage(explosion_damage)
	var splotch := SPLOTCH_SCENE.instantiate() as Sprite2D
	splotch.texture = preload("res://assets/sprites/blueberry_splotch.png")
	splotch.global_position = global_position
	get_parent().add_child(splotch)


func _on_animation_finished() -> void:
	if not alive:
		return
	if sprite.animation == "ripen" and _mode == Mode.RIPEN:
		_mode = Mode.CHASE
		sprite.play("move")
	elif sprite.animation == "explode" and _mode == Mode.EXPLODE:
		die()


func _on_detector_body(body: Node) -> void:
	if body.is_in_group("player"):
		wake()


func die() -> void:
	if not alive and _mode == Mode.EXPLODE:
		return
	if _mode == Mode.EXPLODE:
		alive = false
		set_collision_layer_value(3, false)
		died.emit()
		queue_free()
		return
	_mode = Mode.EXPLODE
	super.die()
