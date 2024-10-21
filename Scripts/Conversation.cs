using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Conversation : TextureRect
{
	[Export] Label convNameLabel;
	[Export] Label convTextLabel;

	int charDelay = 50;

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(Input.IsActionJustPressed("clickL") || Input.IsActionJustPressed("clickR")) //대화빠르게
		{
			charDelay = 0;
		}
    }
    public async Task Conv(string[] name, string[][] text, bool pause = true, bool autoskip = false, int charDelay = 50)
	{
		GetTree().Paused = pause; //일시정지

		Visible = true;
		for(int i = 0; i < text.Length; i++)
		{
			convNameLabel.Text = $"[ {name[i]} ]";
			convTextLabel.Text = "";

			this.charDelay = charDelay;//딜레이초기화

			for(int ii = 0; ii < text[i].Length; ii++)
			{
				for(int iii = 0; iii < text[i][ii].Length; iii++)
				{
					convTextLabel.Text += text[i][ii][iii];
					await Task.Delay(this.charDelay);
				}
				convTextLabel.Text += "\n";
			}

			while(!autoskip && !(Input.IsActionJustPressed("clickL") || Input.IsActionJustPressed("clickR")))
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		Visible = false;

		GetTree().Paused = false; //일시정지
	}


}
