extends RefCounted
class_name HitFlash

const SHADER := preload("res://shaders/hit_flash.gdshader")
const BLINK := 0.06

static func prepare(sprite: CanvasItem) -> void:
	var mat := ShaderMaterial.new()
	mat.shader = SHADER
	mat.set_shader_parameter("flash", 0.0)
	sprite.material = mat


static func play(sprite: CanvasItem) -> void:
	if sprite == null:
		return
	var mat := sprite.material as ShaderMaterial
	if mat == null or mat.shader != SHADER:
		prepare(sprite)
		mat = sprite.material as ShaderMaterial
	_stop(sprite)
	_set_flash(mat, 1.0)
	var tween := sprite.create_tween()
	sprite.set_meta(&"hit_flash_tween", tween)
	tween.tween_interval(BLINK)
	tween.tween_callback(_set_flash.bind(mat, 0.0))
	tween.tween_interval(BLINK)
	tween.tween_callback(_set_flash.bind(mat, 1.0))
	tween.tween_interval(BLINK)
	tween.tween_callback(_set_flash.bind(mat, 0.0))


static func _stop(sprite: CanvasItem) -> void:
	if not sprite.has_meta(&"hit_flash_tween"):
		return
	var tween: Tween = sprite.get_meta(&"hit_flash_tween")
	if tween != null and tween.is_valid():
		tween.kill()


static func _set_flash(mat: ShaderMaterial, amount: float) -> void:
	if is_instance_valid(mat):
		mat.set_shader_parameter("flash", amount)
