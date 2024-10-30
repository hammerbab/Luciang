using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Bullet : Area2D
{
	public GameManager gm;
	public Camera2D cam;

	[Export] public int id;

	//타게팅은 플레이어만 사용함
	public bool isTargeting;
	public bool isEnemy; //적이 발사했는가?
	[Export] public int dmg;
	public float spd;
	public SummonObject target;
	public HealthObject owner;
	public HealthObject player;
	[Export] public bool removeImidiately = true;
	public int remainTime = int.MaxValue;

	public float time = 0;
	public bool canBeRmvd = true;

	public Vector2 dir;

	public Vector2 originPos;

	public async override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public async void Spawned()
	{
		time = 0f;
		canBeRmvd = true;

		if(id == 8)
		{
			for(int i = 0; i<8; i++)
			{
				var b = GetChild(0).GetChild(i) as Bullet;
				b.id = 10;
				b.dir = dir.Normalized();
				b.dmg = dmg;
				b.spd = spd;
				b.isTargeting = isTargeting;
				b.target = target as SummonObject;
				b.owner = owner;
				b.isEnemy = isEnemy;
				b.player = this.player;
				b.removeImidiately = removeImidiately;
				b.remainTime = remainTime;

				b.gm = this.gm;
				b.cam = this.cam;

				b.time = 0f;
				b.canBeRmvd = false;
				b.ProcessMode = ProcessModeEnum.Pausable;
				b.Visible = true;
				b.GetChild(1).ProcessMode = ProcessModeEnum.Inherit;
				b.Spawned();
			}
		}

		if(id != 8 && id != 9 && id != 10) LookAt(originPos + dir);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		time+=(float)delta;
		if(canBeRmvd && remainTime != int.MaxValue && time >= remainTime/1000f && ProcessMode == ProcessModeEnum.Pausable)
		{
			if(id == 7)
			{
				var b = gm.SummonBullet(3, GlobalPosition, Vector2.Zero, dmg, 0, false, owner, null, true, true, false, 900);
				b.Scale = Vector2.One * 0.3f;
			}
			gm.RemoveBullet(this);
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		if(target != null && !IsInstanceValid(target)) gm.RemoveBullet(this);
		if(canBeRmvd && time != 0 && id!=6 && id!=8 && id!=10 && GlobalPosition.DistanceTo(cam.GlobalPosition) > 1333)gm.RemoveBullet(this);

		if(isTargeting)
		{
			if( target.dead || !IsInstanceValid(target) || target == null) gm.RemoveBullet(this);

			dir = (target.GlobalPosition - GlobalPosition).Normalized();

			LookAt(GlobalPosition - dir);
			
			GlobalPosition += dir * spd * (float)delta;
		}
		else
		{
			if(id == 7) //유도탄
			{
				dir += -0.03f * (dir - GlobalPosition.DirectionTo(player.GlobalPosition));
				spd *= 1.007f;
				LookAt(GlobalPosition - dir);
			}
			else if(id != 8 && id != 9 && id != 10)
			{
				LookAt(originPos + dir);
			}
			
			GlobalPosition += dir * spd * (float)delta;
		}
	}

	private void OnBodyEntered(Node other)
	{
		if(!isTargeting && isEnemy && (other.IsInGroup("Player") || (other.IsInGroup("Neutral") && id == 1))) //적이 아군을 맞춤
		{
			if(id == 4)
			{
				Node b = gm.SummonBullet(3, GlobalPosition, Vector2.Zero, dmg, 0, false, owner, null, true, true, false, 900);
			}
			else
			{
				var a = other as HealthObject;
				a.GetDamage(other.IsInGroup("Neutral") ? 1 : dmg); //중립은 데미지 1만 받음
			}

			if(removeImidiately) gm.RemoveBullet(this);
		}
		if((other == target || other.IsInGroup("Neutral")) && isTargeting && !isEnemy) //플레이어가 적을 타겟으로 쏜 총알이 도달함
		{
			var a = other as HealthObject;
			a.GetDamage(other.IsInGroup("Neutral") ? 1 : dmg); //중립은 데미지 1만 받음

			player.GetDamage(-target.hitHeal); //피흡

			if(target.curHP <= 0)
			{
				gm.ChangeScore(target.killScore);
			}

			if(removeImidiately) gm.RemoveBullet(this);
		}
	}
}
