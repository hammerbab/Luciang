using Godot;
using System;
using System.Text.Json;

public partial class ScoreLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScoreContainer scoCon = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer; //점수 저장
		Text = "점수 : " + scoCon.lastScore.ToString("0");
		if(scoCon.stageID != 1) Text += "debug";
		else if(scoCon.lastScore > scoCon.highScore)
		{
			Text += "\n하이스코어 경신!";
			scoCon.highScore = scoCon.lastScore;

			using var file = FileAccess.Open("user://hs.dat", FileAccess.ModeFlags.Write);
    		file.StoreString((scoCon.highScore*86).ToString());
		}
	}
}
