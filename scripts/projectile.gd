extends Area2D
class_name Projectile

const BREAK_SCENE := preload("res://scenes/vfx/break_particles.tscn")

@export var hits_enemies := true
@export var hits_player := false
@export var speed := 160.0
@export var damage := 3
@export var lifetime := 12.0
@export var rotation_offset := -PI * 0.25
@export var break_color := Color(0.95, 0.85, 0.45)

var pool: ProjectilePool
var direction := Vector2.RIGHT
var _life := 0.0
var _active := false


func _ready() -> void:
	body_entered.connect(_on_body_entered)


func activate(origin: Vector2, shot_direction: Vector2) -> void:
	_active = true
	direction = shot_direction.normalized()
	if direction == Vector2.ZERO:
		direction = Vector2.RIGHT
	global_position = origin
	global_rotation = direction.angle() + rotation_offset
	_life = lifetime
	show()
	monitoring = true
	set_physics_process(true)
	$Collision.set_deferred("disabled", false)


func deactivate() -> void:
	_active = false
	set_physics_process(false)
	monitoring = false
	hide()
	$Collision.set_deferred("disabled", true)


func _physics_process(delta: float) -> void:
	global_position += direction * speed * delta
	_life -= delta
	if _life <= 0.0:
		_break(false)


func _on_body_entered(body: Node) -> void:
	if not _active:
		return
	if hits_enemies and body.is_in_group("enemies") and body.has_method("take_damage"):
		body.take_damage(damage)
		_break(true)
		return
	if hits_player and body.is_in_group("player") and body.has_method("receive_damage"):
		body.receive_damage(damage)
		_break(true)
		return
	if body is TileMapLayer:
		_break(true)


func _break(spawn_fx: bool) -> void:
	if not _active:
		return
	_active = false
	if spawn_fx:
		var fx := BREAK_SCENE.instantiate() as Node2D
		fx.global_position = global_position
		fx.modulate = break_color
		get_tree().current_scene.add_child(fx)
	if pool:
		pool.recycle(self)
	else:
		queue_free()
