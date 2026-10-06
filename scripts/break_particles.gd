extends CPUParticles2D


func _ready() -> void:
	one_shot = true
	emitting = true
	explosiveness = 0.9
	amount = 8
	lifetime = 0.35
	spread = 180.0
	gravity = Vector2(0, 30)
	initial_velocity_min = 16.0
	initial_velocity_max = 48.0
	texture = preload("res://assets/sprites/toothpick_particle.png")
	await get_tree().create_timer(lifetime + 0.05).timeout
	queue_free()
