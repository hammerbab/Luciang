using Godot;
using System;

public partial class HighScore : Label
{
	ScoreContainer sc;

    public override void _Ready()
	{
		sc = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer;
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

		Text = "하이스코어" + (sc.highScore != 0 ? $": {sc.highScore}" : " 없음");
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(Input.IsActionJustPressed("Reset"))
		{
			using var file = FileAccess.Open("user://hs.dat", FileAccess.ModeFlags.Write);
			if(file != null)
			{
				file.StoreString(0.ToString());
			}
    		
			sc.highScore = 0;

			Text = "하이스코어 없음";
		}
    }
}
