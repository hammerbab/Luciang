extends Node

@export var menu : Control
@export var prologue : Control

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	
func to_menu():
	Loading.load_scene("res://Scenes/menu_scene.tscn")

func game_start():
	menu.visible = false
	prologue.visible = true
