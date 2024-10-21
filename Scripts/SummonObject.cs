using Godot;
using System;
using System.Threading.Tasks;

public partial class SummonObject : HealthObject
{
	Camera2D cam;

	[Export] public Sprite2D selectCircle; //플레이어에게 공격 대상이 되었을 때 활성화되는 원
	[Export] public AnimationPlayer circlePlayer;
	[Export] public int id;
	[Export] ProgressBar hpBar;

	public Player player;

	[Export] public int killScore;
	[Export] public int hitHeal = 1;
	float moveSpd;
	Vector2 moveDir;

	public override void _Ready()
	{
		base._Ready();
		cam = GetTree().Root.GetNode<Camera2D>("GameScene/Camera2D");
	}

	/*
	Move()의 무한루프에서 타이밍 재는 행동
	Process()에서 타이밍 안 재는 행동
	*/

	public async void Move()
	{
		RandomNumberGenerator random = new RandomNumberGenerator();

		//행동
		if(id == 10)
		{
			await Task.Delay(3000);
			while(gm.pause) //대화 기다리기
			{
				await Task.Delay(1);
			}

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(1000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 10, 800, false, this);
				await Task.Delay(2000);
			}
		}
		if(id == 11)
		{
			moveDir = Vector2.Right;
			moveSpd = 300;
			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(3000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 12, 700, false, this);
			}
		}
		if(id == 19)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-100) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "킹 샌드백", "루시앙"},
			new string[][]
			{
				new string[]{"와 겁나 큰 슬라임이다"},
				new string[]{"...(나도 샌드백이야 무식한 놈아)"},
				new string[]{"뭐래는거야 죽어라!!"}
			});
			player.attackSeal = false;

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(3000);

				for(int i = 0; i < 80; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(50);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 20, 700, false, this);
				}
			} 
		}
		if(id == 90)
		{
			gm.scroll = true; //보스 사망시 스크롤 재개
		}
		if(id == 20)
		{
			await Task.Delay(3000);
			while(gm.pause) //대화 기다리기
			{
				await Task.Delay(1);
			}

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 3; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(400);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 20, 800, false, this);
				}
				
				await Task.Delay(2000);
			}
		}
		if(id == 21)
		{
			await Task.Delay(1500);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(2000);

				for(int i = 0; i < 3; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(20);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 20, 1200, false, this);
				}
			}
		}
		if(id == 29)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-150) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "황야의 망나니", "루시앙"},
			new string[][]
			{
				new string[]{"우리 엄마 어딨는지 알아?"},
				new string[]{"칩입자다!!!!!!!!"},
				new string[]{"꺼져!!!!!!!!!!!!!!!!!!"}
			});
			player.attackSeal = false;

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 7; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

					Vector2 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
					Bullet genBullet = gm.SummonBullet(0, GlobalPosition, dir.Rotated(Mathf.DegToRad((i%2==0?i:-i)*10f)), 20, 700, false, this, null, i == 0);
				}

				await Task.Delay(random.RandiRange(500,1000));
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 20, 600, false, this);
				}

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);
				moveSpd = GlobalPosition.DistanceTo(player.GlobalPosition) * 0.4f;

				await Task.Delay(random.RandiRange(500,1000));
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 20, 600, false, this);
				}

				await Task.Delay(random.RandiRange(500,1000));
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 20, 600, false, this);
				}
				
				await Task.Delay(random.RandiRange(500,1000));
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			}
		}
		if(id == 91)
		{
			gm.scroll = true; //보스 사망시 스크롤 재개
		}
		if(id == 30)
		{
			await Task.Delay(3000);
			while(gm.pause) //대화 기다리기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(1500);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				//부채꼴

				gm.SummonBullet(2, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(-10)), 10, 600, false, this);
				gm.SummonBullet(2, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(000)), 10, 600, false, this, null, false);
				gm.SummonBullet(2, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(010)), 10, 600, false, this, null, false);
			}
		}
		if(id == 31)
		{
			await Task.Delay(2000);


		}
		if(id == 32)
		{
			await Task.Delay(2000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(2000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				//12방향음파

				for(int i = 0; i < 12; i++)
				{
					gm.SummonBullet(2, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(i*30)), 10, 600, false, this);
					await Task.Delay(66);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}
			}
		}
	}

	public async override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if(GlobalPosition.Y > cam.GlobalPosition.Y + 888 && ( id != 19 && id != 29)) QueueFree(); //화면 지나가면 사라짐

		if(dead) //사망시기믹
		{
			//공통 기믹
			hpBar.Visible = false;

			//개별 기믹
			if(id == 1 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				GD.Print("Exp");
				Node b = gm.SummonBullet(3, GlobalPosition, Vector2.Up, 24, 0, false, this, null, true, true, false, 2000);
				AnimationPlayer ap = b.GetChild(2) as AnimationPlayer;
				ap.Play("Explosion");
			}
			if(id == 19 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "킹 샌드백", "루시앙", "킹 샌드백", "루시앙"},
				new string[][]{new string[]{"우리 엄마 어딨냐고??"},
				new string[]{"...(...)"},
				new string[]{"??"},
				new string[]{"...................(...)"},
				new string[]{"딴데 가보자"}});

				gm.Summon(90, cam.GlobalPosition + Vector2.Up*1000); //스테이지 클리어 글자
				gm.SummonStage2();
			}
			if(id == 29 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "황야의 망나니", "루시앙", "황야의 망나니", "루시앙"},
				new string[][]{new string[]{"우리 엄마 어딨냐고?"},
				new string[]{"정..."},
				new string[]{"..."},
				new string[]{"글............", "차.......이...."},
				new string[]{"??", "...", "정글로 가보자"}});

				gm.Summon(91, cam.GlobalPosition + Vector2.Up*1000); //스테이지 클리어 글자
				gm.SummonStage3();
			}
			// if(id == 6 && curHP > -3000000)
			// {
			// 	curHP = -999999999; //중복 방지를 위한 트리거
			// 	ScoreContainer scoCon = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer; //점수 저장
			// 	scoCon.lastScore = gm.score;

			// 	await Task.Delay(2000);

			// 	GetTree().ChangeSceneToFile("res://Scenes/game_clear_scene.tscn"); //게임 클리어
			// }

			//리턴
			return;
		}

		//개별 기믹
		if(id == 11)
		{
			if(moveDir == Vector2.Right && GlobalPosition.X >= 1000) moveDir = Vector2.Left;
			else if(moveDir == Vector2.Left && GlobalPosition.X <= -1000) moveDir = Vector2.Right;

			GlobalPosition += moveDir * moveSpd * (float)delta;
		}
		if(id == 19)
		{

		}
		if(id == 90)
		{
			GlobalPosition += Vector2.Down * 400 * (float)delta;
		}
		if(id == 21)
		{
			moveDir = (GlobalPosition - player.GlobalPosition).Normalized();
			moveSpd = 20000000f / GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
			GlobalPosition += moveDir * moveSpd * (float)delta;
		}
		if(id == 29)
		{
			GlobalPosition += moveDir * moveSpd * (float)delta;
		}
		if(id == 91)
		{
			GlobalPosition += Vector2.Down * 400 * (float)delta;
		}
		if(id == 30)
		{
			GlobalPosition += moveDir * moveSpd * (float)delta;
		}
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

    public override void _Process(double delta)
    {
        base._Process(delta);

		hpBar.MaxValue = maxHP;
		hpBar.Value = curHP;
    }
}
