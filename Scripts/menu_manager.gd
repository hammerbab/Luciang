extends Node

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	
func to_menu():
	Loading.load_scene("res://Scenes/menu_scene.tscn")

func game_start():
	Loading.load_scene("res://Scenes/game_scene.tscn")
