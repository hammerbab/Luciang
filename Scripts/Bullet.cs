using Godot;
using System;

public partial class Bullet : Area2D
{
	public GameManager gm;

	//타게팅은 플레이어만 사용함
	public bool isTargeting;
	public bool isEnemy; //적이 발사했는가?
	public int dmg;
	public float spd;
	public SummonObject target;
	public HealthObject owner;
	public HealthObject player;

	public Vector2 dir;

	public Vector2 originPos;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(isTargeting)
		{
			if( !IsInstanceValid(target) || target == null) this.QueueFree();

			LookAt(target.GlobalPosition);
			dir = (target.GlobalPosition - GlobalPosition).Normalized();
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
		if(!isTargeting && other.IsInGroup("Player") && isEnemy) //적이 아군을 맞춤
		{
			player.GetDamage(dmg);

			this.QueueFree();
		}
		if(other == target && isTargeting && !isEnemy) //플레이어가 적을 타겟으로 쏜 총알이 도달함
		{
			target.GetDamage(dmg);

			gm.ChangeScore(20);

			if(target.curHP <= 0)
			{
				gm.ChangeScore(target.killScore);
			}

			this.QueueFree();
		}
	}
}
