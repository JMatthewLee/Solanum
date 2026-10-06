extends Node2D
class_name PixelBurst

const LIFETIME := 0.28
const SPEED_MIN := 28.0
const SPEED_MAX := 56.0

var _bits: Array[Dictionary] = []
var _time := 0.0


static func spawn(sprite: Sprite2D) -> void:
	if sprite == null or sprite.texture == null or not sprite.is_inside_tree():
		return
	var image := sprite.texture.get_image()
	if image == null:
		return
	if image.is_compressed():
		image.decompress()
	var width := image.get_width()
	var height := image.get_height()
	var origin := Vector2(width, height) * 0.5
	var bits: Array[Dictionary] = []
	for y in height:
		for x in width:
			var color := image.get_pixel(x, y)
			if color.a < 0.5:
				continue
			var local := Vector2(float(x) + 0.5, float(y) + 0.5) + sprite.offset
			if sprite.centered:
				local -= origin
			var world := sprite.to_global(local)
			var away := world - sprite.global_position
			if away.length_squared() < 0.01:
				away = Vector2.from_angle(randf() * TAU)
			else:
				away = away.normalized()
			away = away.rotated(randf_range(-0.45, 0.45))
			bits.append({
				"pos": world,
				"vel": away * randf_range(SPEED_MIN, SPEED_MAX),
				"color": color,
			})
	if bits.is_empty():
		return
	var scene := sprite.get_tree().current_scene
	if scene == null:
		return
	var burst := PixelBurst.new()
	burst._bits = bits
	burst.z_index = 8
	scene.add_child(burst)
	burst.top_level = true
	burst.global_position = Vector2.ZERO


func _process(delta: float) -> void:
	_time += delta
	if _time >= LIFETIME:
		queue_free()
		return
	var drag := exp(-7.0 * delta)
	for bit in _bits:
		var vel: Vector2 = bit["vel"]
		vel *= drag
		bit["vel"] = vel
		bit["pos"] = (bit["pos"] as Vector2) + vel * delta
	queue_redraw()


func _draw() -> void:
	var alpha := 1.0
	var fade_at := LIFETIME * 0.55
	if _time > fade_at:
		alpha = 1.0 - (_time - fade_at) / (LIFETIME - fade_at)
	for bit in _bits:
		var color: Color = bit["color"]
		color.a *= alpha
		var pos: Vector2 = (bit["pos"] as Vector2).round()
		draw_rect(Rect2(pos, Vector2.ONE), color)
