extends CharacterBody2D

const PPU := 32.0
const MOVE_SPEED := 4.0 * PPU
const FIRE_COOLDOWN := 0.4
const MAX_HP := 5
const SPLOTCH_SCENE := preload("res://scenes/vfx/splotch.tscn")

signal hp_changed(current: int, maximum: int)
signal coins_changed(amount: int)
signal defeated

@export var toothpick_pool_path: NodePath

var hp := MAX_HP
var coins := 0
var _cooldown := 0.0
var _alive := true
var _facing := Vector2.RIGHT

@onready var _body: AnimatedSprite2D = $Body
@onready var _hat: AnimatedSprite2D = $Hat
@onready var _muzzle: Node2D = $Muzzle
@onready var _spawn: Marker2D = $Muzzle/Spawn


func _ready() -> void:
	motion_mode = MOTION_MODE_FLOATING
	_body.play("idle")
	_hat.play("idle")
	hp_changed.emit(hp, MAX_HP)
	coins_changed.emit(coins)


func _physics_process(delta: float) -> void:
	if not _alive:
		return
	var input := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	velocity = input * MOVE_SPEED
	move_and_slide()

	var aim := get_global_mouse_position() - global_position
	if aim.length_squared() > 1.0:
		_facing = aim.normalized()
		_muzzle.rotation = _facing.angle()
	_face_cursor()

	var moving := input.length_squared() > 0.01
	_set_anim("move" if moving else "idle")

	_cooldown = max(_cooldown - delta, 0.0)
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT) and _cooldown <= 0.0:
		_fire()


func gain_coins(amount: int) -> void:
	coins += amount
	coins_changed.emit(coins)


func receive_damage(amount: int) -> void:
	if not _alive:
		return
	hp = max(hp - amount, 0)
	HitFlash.flash(self, [_body, _hat])
	_spawn_splotch()
	hp_changed.emit(hp, MAX_HP)
	if hp <= 0:
		_die()


func _fire() -> void:
	var pool := get_node_or_null(toothpick_pool_path) as ProjectilePool
	if pool == null:
		return
	_cooldown = FIRE_COOLDOWN
	pool.spawn(_spawn.global_position, _facing)


func _face_cursor() -> void:
	var flip := _facing.x < 0.0
	_body.flip_h = flip
	_hat.flip_h = flip


func _set_anim(anim_name: String) -> void:
	if _body.animation != anim_name:
		_body.play(anim_name)
	if _hat.sprite_frames and _hat.sprite_frames.has_animation(anim_name) and _hat.animation != anim_name:
		_hat.play(anim_name)


func _spawn_splotch() -> void:
	var splotch := SPLOTCH_SCENE.instantiate() as Node2D
	splotch.global_position = global_position + Vector2(0, 12)
	get_parent().add_child(splotch)


func _die() -> void:
	_alive = false
	velocity = Vector2.ZERO
	set_collision_layer_value(2, false)
	defeated.emit()
	await get_tree().create_timer(0.7).timeout
	if is_inside_tree():
		get_tree().reload_current_scene()
