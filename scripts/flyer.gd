extends GardenEnemy

const PPU := 32.0
const REACH := 1.5 * PPU
const SHOOT_INTERVAL := 2.0
const MAX_ATTACKS := 2

enum Mode { ASLEEP, MOVE, ATTACK }

@export var diagonal := false
@export var shot_color := Color(0.85, 0.15, 0.2)

var _mode := Mode.ASLEEP
var _nodes: Array[Node2D] = []
var _node_index := 0
var _attacks := 0
var _shoot_timer := 0.0
var _path_timer := 0.0


func _ready() -> void:
	speed = 1.5 * 1.5 * 0.6 * PPU
	super._ready()
	sprite.play("idle")
	sprite.animation_finished.connect(_on_animation_finished)
	$Detector.body_entered.connect(_on_detector_body)
	add_to_group("flyers")
	for node in get_tree().get_nodes_in_group("patrol_nodes"):
		var marker := node as Node2D
		if marker:
			_nodes.append(marker)
	_nodes.sort_custom(func(a: Node2D, b: Node2D) -> bool: return a.name < b.name)


func wake() -> void:
	if _mode != Mode.ASLEEP or not alive:
		return
	_mode = Mode.MOVE
	_shoot_timer = 0.0
	_go_to_current_node()


func _physics_process(delta: float) -> void:
	if not alive:
		return
	if _mode == Mode.MOVE:
		_shoot_timer += delta
		if _shoot_timer >= SHOOT_INTERVAL:
			_shoot_timer = 0.0
			if randf() < 0.2:
				_start_attack()
				return
		var goal := _current_goal()
		if global_position.distance_to(goal) <= REACH:
			_start_attack()
			return
		if sprite.animation != "move":
			sprite.play("move")
		_path_timer -= delta
		var refresh := _path_timer <= 0.0
		if refresh:
			_path_timer = 0.3
		steer_toward(goal, refresh)


func _start_attack() -> void:
	if _mode == Mode.ATTACK:
		return
	_mode = Mode.ATTACK
	velocity = Vector2.ZERO
	sprite.play("attack")


func _on_animation_finished() -> void:
	if not alive or _mode != Mode.ATTACK:
		return
	if sprite.animation != "attack":
		return
	_fire()
	_attacks += 1
	if _attacks >= MAX_ATTACKS:
		_attacks = 0
	_pick_next_node()
	_mode = Mode.MOVE
	_shoot_timer = 0.0
	_go_to_current_node()


func _fire() -> void:
	var pool := get_tree().get_first_node_in_group("berry_pool") as ProjectilePool
	if pool == null:
		return
	for direction in _shot_directions():
		pool.spawn(global_position, direction)


func _shot_directions() -> Array[Vector2]:
	if diagonal:
		return [Vector2(1, -1), Vector2(-1, 1), Vector2(-1, -1), Vector2(1, 1)]
	return [Vector2.RIGHT, Vector2.LEFT, Vector2.UP, Vector2.DOWN]


func _pick_next_node() -> void:
	if _nodes.size() <= 1:
		return
	var next := _node_index
	while next == _node_index:
		next = randi_range(0, _nodes.size() - 1)
	_node_index = next


func _go_to_current_node() -> void:
	_path_timer = 0.0
	sprite.play("move")


func _current_goal() -> Vector2:
	if _nodes.is_empty():
		return global_position
	return _nodes[_node_index].global_position


func _on_detector_body(body: Node) -> void:
	if body.is_in_group("player"):
		wake()
