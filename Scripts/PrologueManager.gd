extends Node

@export var picture : TextureButton
@export var texture1 : CompressedTexture2D
@export var texture2 : CompressedTexture2D
@export var texture3 : CompressedTexture2D

var count : int

func _ready():
	picture.texture_normal = texture1



func _on_pic_pressed():
	if(count == 2):
		Loading.load_scene("res://Scenes/game_scene.tscn")
	elif(count == 1):
		count += 1
		picture.texture_normal = texture3
	elif(count == 0):
		count += 1
		picture.texture_normal = texture2
