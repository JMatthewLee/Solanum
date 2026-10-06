extends Node
class_name HitFlash

const FLASH_SHADER := preload("res://shaders/hit_flash.gdshader")

static func flash(host: Node, sprites: Array) -> void:
	for item in sprites:
		if item == null or not (item is CanvasItem):
			continue
		var sprite := item as CanvasItem
		var material := sprite.material as ShaderMaterial
		if material == null or material.shader != FLASH_SHADER:
			material = ShaderMaterial.new()
			material.shader = FLASH_SHADER
			sprite.material = material
		material.set_shader_parameter("flash_amount", 1.0)
		var tween := host.create_tween()
		tween.tween_property(material, "shader_parameter/flash_amount", 0.0, 0.12)
