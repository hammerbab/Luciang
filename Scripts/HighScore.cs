using Godot;
using System;

public partial class HighScore : Label
{
	public override void _Ready()
	{
		ScoreContainer sc = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer;
		using var file = FileAccess.Open("user://hs.dat", FileAccess.ModeFlags.Read);
		string content;
		if(file != null)
		{
			content = file.GetAsText();
		}
		else
		{
			content = 0.ToString();
		}
    		
		sc.highScore = int.Parse(content) / 86;

		Text = $"하이스코어: {sc.highScore}";
	}
}
