using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class Bullet : Area2D
{
	public GameManager gm;
	public Camera2D cam;

	public int id;

	//타게팅은 플레이어만 사용함
	public bool isTargeting;
	public bool isEnemy; //적이 발사했는가?
	public int dmg;
	public float spd;
	public SummonObject target;
	public HealthObject owner;
	public HealthObject player;
	public bool removeImidiately = true;
	public int remainTime = int.MaxValue;

	public Vector2 dir;

	public Vector2 originPos;

	public async override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public async void Spawned()
	{
		if(id == 2)
		{
			Visible = false;
			await Task.Delay(30);
			Visible = true;
		}
		if(remainTime == int.MaxValue) return;
		await Task.Delay(remainTime-30);

		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(target != null && !IsInstanceValid(target)) QueueFree();
		if(GlobalPosition.DistanceTo(cam.GlobalPosition) > 3333) QueueFree();

		if(isTargeting)
		{
			if( target.dead || !IsInstanceValid(target) || target == null) this.QueueFree();

			dir = (target.GlobalPosition - GlobalPosition).Normalized();

			LookAt(GlobalPosition - dir);
			
			GlobalPosition += dir * spd * (float)delta;
		}
		else
		{
			LookAt(originPos + dir);
			GlobalPosition += dir * spd * (float)delta;
		}
	}

	private void OnBodyEntered(Node other)
	{
		if(!isTargeting && (other.IsInGroup("Player") || other.IsInGroup("Neutral")) && isEnemy) //적이 아군을 맞춤
		{
			var a = other as HealthObject;
			a.GetDamage(other.IsInGroup("Neutral") ? 1 : dmg); //중립은 데미지 1만 받음

			if(removeImidiately) this.QueueFree();
		}
		if(other == target && isTargeting && !isEnemy) //플레이어가 적을 타겟으로 쏜 총알이 도달함
		{
			target.GetDamage(dmg);
			player.GetDamage(-target.hitHeal); //피흡

			//gm.ChangeScore(20);

			if(target.curHP <= 0)
			{
				gm.ChangeScore(target.killScore);
			}

			this.QueueFree();
		}
	}
}
