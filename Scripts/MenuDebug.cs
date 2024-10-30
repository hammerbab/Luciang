using Godot;
using System;

public partial class MenuDebug : TextureRect
{
	[Export] Label label;

    public override void _Ready()
    {
        base._Ready();

		ScoreContainer sc = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer;
		sc.stageID = 1;
    }

    public override void _Process(double delta)
	{
		ScoreContainer sc = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer;
		if(Input.IsActionJustPressed("Debug1"))
		{
			sc.stageID = 1;
			label.Text = "Debug:1";
		}
		if(Input.IsActionJustPressed("Debug2"))
		{
			sc.stageID = 2;
			label.Text = "Debug:2";
		}
		if(Input.IsActionJustPressed("Debug3"))
		{
			sc.stageID = 3;
			label.Text = "Debug:3";
		}
		if(Input.IsActionJustPressed("Debug4"))
		{
			sc.stageID = 4;
			label.Text = "Debug:4";
		}
	}
}
