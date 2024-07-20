using Godot;
using System;
using System.Threading.Tasks;

public partial class SummonObject : HealthObject
{
	public GameManager gm;

	[Export] public Sprite2D selectCircle; //플레이어에게 공격 대상이 되었을 때 활성화되는 원
	[Export] public AnimationPlayer circlePlayer;
	public int id = 0; //임시: 0이면 이동 1이면 정지

	public Player player;

	[Export] public int killScore;
	float moveSpd;
	Vector2 moveDir;

	public override void _Ready()
	{
		base._Ready();
	}

	public async void Move()
	{
		RandomNumberGenerator random = new RandomNumberGenerator();

		//행동	
		if(id == 0)
		{
			while(true)
			{
				if(curHP <= 0) break;
				if(player.dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			
				gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 10, 1500, false, this, null);

				await Task.Delay(3000);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		//if(curHP <= 0) return;
		//GlobalPosition += moveDir * moveSpd * (float)delta;
	}

	public void CircleAnim()
	{
		circlePlayer.Stop();
		circlePlayer.Play("circle");
	}
	public void DeCircleAnim()
	{
		circlePlayer.Stop();
		circlePlayer.Play("dis");
	}
}
