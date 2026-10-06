extends Node
class_name ProjectilePool

@export var projectile_scene: PackedScene
@export var pool_size := 32

var _free: Array[Projectile] = []


func _ready() -> void:
	for _i in pool_size:
		var projectile := projectile_scene.instantiate() as Projectile
		add_child(projectile)
		projectile.pool = self
		projectile.deactivate()
		_free.append(projectile)


func spawn(origin: Vector2, direction: Vector2) -> void:
	if _free.is_empty():
		return
	var projectile := _free.pop_back()
	projectile.activate(origin, direction)


func recycle(projectile: Projectile) -> void:
	if projectile in _free:
		return
	projectile.deactivate()
	_free.append(projectile)
