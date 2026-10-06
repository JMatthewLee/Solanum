extends CharacterBody2D
class_name GardenEnemy

signal died

@export var max_hp := 10
@export var speed := 64.0

var hp := 0
var alive := true

@onready var sprite: AnimatedSprite2D = $Sprite
@onready var agent: NavigationAgent2D = $NavigationAgent2D


func _ready() -> void:
	hp = max_hp
	motion_mode = MOTION_MODE_FLOATING
	HitFlash.prepare(sprite)
	agent.avoidance_enabled = true
	agent.radius = 8.0
	agent.max_speed = speed
	agent.avoidance_priority = randf()
	agent.path_desired_distance = 4.0
	agent.target_desired_distance = 6.0
	agent.velocity_computed.connect(_on_safe_velocity)
	add_to_group("enemies")


func wake() -> void:
	pass


func take_damage(amount: int) -> void:
	if not alive:
		return
	hp -= amount
	HitFlash.play(sprite)
	if hp <= 0:
		die()


func die() -> void:
	if not alive:
		return
	alive = false
	velocity = Vector2.ZERO
	set_collision_layer_value(3, false)
	set_physics_process(false)
	if sprite.sprite_frames and sprite.sprite_frames.has_animation("death"):
		sprite.play("death")
		await sprite.animation_finished
	died.emit()
	queue_free()


func steer_toward(goal: Vector2, refresh_path: bool) -> void:
	if refresh_path:
		agent.target_position = goal
	if agent.is_navigation_finished():
		velocity = Vector2.ZERO
		move_and_slide()
		return
	var next := agent.get_next_path_position()
	var offset := next - global_position
	if offset.length_squared() < 4.0:
		velocity = Vector2.ZERO
		move_and_slide()
		return
	agent.set_velocity(offset.normalized() * speed)


func _on_safe_velocity(safe_velocity: Vector2) -> void:
	if not alive:
		return
	velocity = safe_velocity
	move_and_slide()
