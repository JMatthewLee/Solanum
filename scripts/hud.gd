extends CanvasLayer

@onready var _hp: Label = $Margin/Panel/Stats/Hp
@onready var _coins: Label = $Margin/Panel/Stats/Coins
@onready var _enemies: Label = $Margin/Panel/Stats/Enemies


func set_hp(current: int, maximum: int) -> void:
	_hp.text = "HP %d/%d" % [current, maximum]


func set_coins(amount: int) -> void:
	_coins.text = "%dg" % amount


func set_enemies(remaining: int) -> void:
	_enemies.text = "Enemies %d" % remaining
