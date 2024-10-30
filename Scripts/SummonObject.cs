using Godot;
using System;
using System.Data;
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
	float moveSpd = 0;
	Vector2 moveDir;
	private Bullet followBullet;
	SummonObject so1, so2; //보스전 중 따까리
	
	public bool conv = false;

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
			while(!gm.pause)
			{
				if(!conv) break;
				player.GetDamage(-999);
				await Task.Delay(1);
			}
			while(gm.pause) //대화 기다리기
			{
				if(!conv) break;
				await Task.Delay(1);
			}

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(1000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
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
				await gm.SummonShine(GlobalPosition);
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
			gm.appFadeOut(); //페이드인

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(3000);

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 40; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(100);
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
			while(!gm.pause)
			{
				if(!conv) break;
				player.GetDamage(-999);
				await Task.Delay(1);
			}
			while(gm.pause) //대화 기다리기
			{
				if(!conv) break;
				await Task.Delay(1);
			}

			while(true)
			{
				await Task.Delay(2000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 3; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(0, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 20, 800, false, this);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(400);
				}
			}
		}
		if(id == 21)
		{
			await Task.Delay(1500);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(2000);

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

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
			await gm.Conversation(new string[]{"루시앙", "카우-BO2", "루시앙", "카우-BO2"},
			new string[][]
			{
				new string[]{"거기 아저씨", "우리 엄마 어딨는지 아세요?"},
				new string[]{"칩입자다!!!!!!!!"},
				new string[]{"꺼져!!!!!!!!!!!!!!!!!!"},
				new string[]{"가까이 오지 마라!!!!!!!!!!!!!"}
			});
			player.attackSeal = false;
			gm.appFadeOut(); //페이드인

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);
				moveSpd = GlobalPosition.DistanceTo(player.GlobalPosition) * 0.4f;
				if(random.RandiRange(0, 1) == 0) gm.Summon(1, GlobalPosition); 

				await Task.Delay(1000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 5; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

					Vector2 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
					Bullet genBullet = gm.SummonBullet(0, GlobalPosition, dir.Rotated(Mathf.DegToRad((i%2==0?i:-i)*6f)), 20, 700, false, this, null, i == 0);
				}

				await Task.Delay(random.RandiRange(500,1000));
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 20, 600, false, this);
				}

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

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
			while(!gm.pause)
			{
				if(!conv) break;
				player.GetDamage(-999);
				await Task.Delay(1);
			}
			while(gm.pause) //대화 기다리기
			{
				if(!conv) break;
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
				if(player.GlobalPosition.DistanceTo(GlobalPosition) <= range)
				{
					await gm.SummonShine(GlobalPosition);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

					followBullet = gm.SummonBullet(5, GlobalPosition, -moveDir, 30, 0, false, this, null, true, true, false, 5000);
					followBullet.Scale = Vector2.One * 1.2f;

					moveSpd = 8f;
					moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);

					break;
				}
			}
		}
		if(id == 31)
		{
			await Task.Delay(1000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(2000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				//일자로 찍

				var d = GlobalPosition.DirectionTo(player.GlobalPosition);

				for(int i = 0; i < 8; i++)
				{
					gm.SummonBullet(2, GlobalPosition, d, 25, 700, false, this);
					await Task.Delay(100);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}
			}
		}
		if(id == 32)
		{
			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(2000);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				//12방향음파

				var d = GlobalPosition.DirectionTo(player.GlobalPosition);

				for(int i = 0; i < 12; i++)
				{
					gm.SummonBullet(2, GlobalPosition, d.Rotated(Mathf.DegToRad(i*30)), 25, 500, false, this);
					await Task.Delay(66);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}
			}
		}
		if(id == 39)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-100) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "로라니", "루시앙", "로라니", "루시앙", "로라니", "루시앙"},
			new string[][]
			{
				new string[]{"아악!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"},
				new string[]{"뭐야 시끄러워 꺼져"},
				new string[]{"????", "말을 하네"},
				new string[]{"말하는 고라니 처음 보냐??"},
				new string[]{"오 그럼 우리 엄마 어딨는지 알려줄 수 있어?"},
				new string[]{"그거 알아?", "고라니 식구들 이름은 ㄱㄴㄷㄹ 순서로 첫 글자만 바뀐다는 사실"},
				new string[]{"헛소리 말고", "우리 엄마 어딨는지나 알려달라고!!!!!!!!!!!!!!"}
			});
			player.attackSeal = false;
			gm.appFadeOut(); //페이드인

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await Task.Delay(3000);

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 3; i++)
				{
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(-30)), 25, 500, false, this);
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(000)), 25, 500, false, this, null, false);
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(030)), 25, 500, false, this, null, false);
					await Task.Delay(1000);

					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}

				if(so1 != null) so1.QueueFree();
				so1 = gm.Summon(30 + random.RandiRange(0, 2), new Vector2(random.RandfRange(-600f, -350f), cam.GlobalPosition.Y-100));
				so1.killScore = 0;
				if(so2 != null) so2.QueueFree();
				so2 = gm.Summon(30 + random.RandiRange(0, 2), new Vector2(random.RandfRange(350f, 600f), cam.GlobalPosition.Y-100));
				so2.killScore = 0;

				await Task.Delay(2000);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			} 
		}
		if(id == 92)
		{
			gm.scroll = true; //보스 사망시 스크롤 재개
		}
		if(id == 40)
		{
			while(!gm.pause)
			{
				if(!conv) break;
				player.GetDamage(-999);
				await Task.Delay(1);
			}
			while(gm.pause) //대화 기다리기
			{
				if(!conv) break;
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}

			await Task.Delay(1000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				gm.SummonBullet(6, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, 0, false, this, null, true, true, false, 2000);

				await Task.Delay(3000);
			}
		}
		if(id == 41)
		{
			await Task.Delay(1000);
			
			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				gm.SummonBullet(7, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, 200, false, this, null, true, true, true, 5000);

				await Task.Delay(5000);
			}
		}
		if(id == 42)
		{
			await gm.SummonShine(GlobalPosition);

			moveSpd = 3f;
			moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);
		}
		if(id == 43)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-100) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '킹'", "루시앙"},
			new string[][]
			{
				new string[]{"어디서 본 슬라임인데?"},
				new string[]{"$%**^*!(...드...무,,,,,아)"},
				new string[]{"어디가 잘못됐나?"}
			});
			player.attackSeal = false;
			gm.appFadeOut(); //페이드인

			await Task.Delay(1000);

			if(so1 != null) so1.QueueFree();
			so1 = gm.Summon(40, cam.GlobalPosition + new Vector2(-600, -200));
			(so1.col).QueueFree();
			so1.hpBar.ProcessMode = ProcessModeEnum.Disabled;
			so1.killScore = 0;
			await Task.Delay(1000);
			if(so2 != null) so2.QueueFree();
			so2 = gm.Summon(40, cam.GlobalPosition + new Vector2(600, -200));
			(so2.col).QueueFree();
			so2.hpBar.ProcessMode = ProcessModeEnum.Disabled;
			so2.killScore = 0;
			await Task.Delay(1000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 40; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					await Task.Delay(100);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(2, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, 700, false, this, null,true,true,true);
				}

				await Task.Delay(2500);
			}
		}
		if(id == 44)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-150) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '보이'", "루시앙", "가디언-레플리카 '보이'"},
			new string[][]
			{
				new string[]{"아까부터 자꾸 익숙한 얼굴들이 보이는데"},
				new string[]{"!!!!!!!!!!!!!!!!!!!!!!!!!!"},
				new string[]{"다들 뭔가 이상해!!!!!!!"},
				new string[]{"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"}
			});
			player.attackSeal = false;
			player.col.SetProcess(true);
			gm.appFadeOut(); //페이드인

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);
				moveSpd = GlobalPosition.DistanceTo(player.GlobalPosition) * 0.4f;

				if(so1 != null) so1.QueueFree();
				so1 = gm.Summon(42, GlobalPosition); //자폭병기소환
				so1.killScore = 0;

				await Task.Delay(1500);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 10; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

					Vector2 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
					Bullet genBullet = gm.SummonBullet(0, GlobalPosition, dir.Rotated(Mathf.DegToRad((i%2==0?i:-i)*7f)), 30, 700, false, this, null, i == 0);
				}

				await Task.Delay(300);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 30, 600, false, this);
				}
				
				await Task.Delay(1500);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				moveDir = GlobalPosition.DirectionTo(player.GlobalPosition);
				moveSpd = GlobalPosition.DistanceTo(player.GlobalPosition) * 0.4f;

				if(so2 != null) so2.QueueFree();
				so2 = gm.Summon(42, GlobalPosition); //자폭병기소환
				so2.killScore = 0;

				await Task.Delay(1500);

				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 10; i++)
				{
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

					Vector2 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
					Bullet genBullet = gm.SummonBullet(0, GlobalPosition, dir.Rotated(Mathf.DegToRad((i%2==0?i:-i)*7f)), 30, 700, false, this, null, i == 0);
				}

				await Task.Delay(300);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range)
				{
					gm.SummonBullet(1, player.GlobalPosition, Vector2.Zero, 30, 600, false, this);
				}
				
				await Task.Delay(1500);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			}
		}
		if(id == 45)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-100) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '라니'", "루시앙", "가디언-레플리카 '라니'", "루시앙", "레플리카 라니", "루시앙"},
			new string[][]
			{
				new string[]{"아악...!!"},
				new string[]{",,,,,,,,,"},
				new string[]{"아아악!!!!!!!!!!!!!!!!!!!"},
				new string[]{"메...탈....."},
				new string[]{"???"},
				new string[]{"헤 비... 메탈......"},
				new string[]{"뭐라고?"}
			});
			player.attackSeal = false;
			player.col.SetProcess(true);
			gm.appFadeOut(); //페이드인

			if(so1 != null) so1.QueueFree();
			so1 = gm.Summon(41, cam.GlobalPosition + new Vector2(-600, -200));
			(so1.col).QueueFree();
			so1.hpBar.ProcessMode = ProcessModeEnum.Disabled;
			so1.killScore = 0;
			if(so2 != null) so2.QueueFree();
			so2 = gm.Summon(41, cam.GlobalPosition + new Vector2(600, -200));
			(so2.col).QueueFree();
			so2.hpBar.ProcessMode = ProcessModeEnum.Disabled;
			so2.killScore = 0;

			await Task.Delay(2000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				for(int i = 0; i < 5; i++)
				{
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(033)), 30, 250, false, this,null,true,true);
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(000)), 30, 250, false, this, null, false,true);
					gm.SummonBullet(4, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition).Rotated(Mathf.DegToRad(-33)), 30, 250, false, this, null, false,true);
					await Task.Delay(1200);

					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}

				await Task.Delay(3000);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			}
		}
		if(id == 49)
		{
			while(GlobalPosition.Y < cam.GlobalPosition.Y-50) //화면에 내려올 때까지 대기
			{
				await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}
			gm.scroll = false; //보스는 카ㅔ라 이동 X
			await gm.Conversation(new string[]{"루시앙", "가디언-터미네이터", "루시앙", "가디언-터미네이터", "루시앙", "가디언-터미네이터"},
			new string[][]
			{
				new string[]{"네가 최종보스구나??"},
				new string[]{"위험물질. 처단한다."},
				new string[]{"헛소리 마!!!!!!!!!!!!!!!", "우리 엄마 당장 풀어줘!!!!!!!!!!!!!!!!!!"},
				new string[]{"........", "위험물질. 무슨 말을 하는 건지 모르겠다."},
				new string[]{"더이상 못 들어주겠다!!!!!", "결투다!!!!!!!!!!!!!!!!!!!"},
				new string[]{"뼈도 못 추릴 것이다."}
			});
			player.attackSeal = false;
			player.col.SetProcess(true);
			gm.appFadeOut(); //페이드인


			// var o = gm.Summon(41, cam.GlobalPosition + new Vector2(-600, -200));
			// (o.col).QueueFree();
			// o.hpBar.ProcessMode = ProcessModeEnum.Disabled;

			await Task.Delay(2000);

			while(true)
			{
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				if(curHP <= 75) //반피일 경우 패턴 추가 (8방향 포위 창)
				{
					await gm.SummonShine(GlobalPosition);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					for(int i= 0; i < 16; i++)
					{
						gm.SummonBullet(8, player.GlobalPosition, Vector2.Zero, 30, 0, false, this, null, true, true, false, i%2==0?2000:2001);
						await Task.Delay(1000-i*12);
						if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					}
				}
				else
				{
					//아니라면 그냥 레이저 찍찍
					await gm.SummonShine(GlobalPosition);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					Vector2 gp = GlobalPosition + Vector2.Up*340;
					for(int i = 0; i<6; i++)
					{
						gm.SummonBullet(6, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, 0, false, this, null, true, true, false, 2000);
						await Task.Delay(100);
						if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					}
				}

				//유도탄 발사
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(7, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, curHP > 75 ? 160 : 240, false, this, null, true, true, true, 5000);
				if(curHP <= 75)
				{
					await Task.Delay(500);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(7, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, curHP > 75 ? 160 : 240, false, this, null, true, true, true, 5000);

				}//반피일 경우 2
				
				//11111 레이저
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, -700), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, -450), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, -200), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, 50), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, 300), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, 550), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(1500, 800), Vector2.Left, 30, 0, false, this, null, true, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1000 : 400);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				//자폭병 소환
				if(so1 != null) so1.QueueFree();
				so1 = gm.Summon(42, GlobalPosition);
				so1.killScore = 0;
				await Task.Delay(1000);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				if(curHP <= 75)
				{
					if(so2 != null) so2.QueueFree();
					so2 = gm.Summon(42, GlobalPosition);
					so2.killScore = 0;
					}//반피일 경우 2마리
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				await Task.Delay(1000);

				//유도탄 발사
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(7, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, curHP > 75 ? 160 : 240, false, this, null, true, true, true, 5000);
				if(curHP <= 75)
				{
					await Task.Delay(500);
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
					gm.SummonBullet(7, GlobalPosition, GlobalPosition.DirectionTo(player.GlobalPosition), 30, curHP > 75 ? 160 : 240, false, this, null, true, true, true, 5000);

				}//반피일 경우 2

				//랜덤 위치 레이저
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				for(int i = 0; i<(curHP > 75 ? 9 : 11); i++)
				{
					var ranVector = new Vector2(random.RandfRange(-800, 800),random.RandfRange(-800, 800));
					gm.SummonBullet(6, GlobalPosition+ranVector, (GlobalPosition+ranVector).DirectionTo(player.GlobalPosition), 30, 0, false, this, null, true, true, false, 2000);
					gm.SummonBullet(6, GlobalPosition+ranVector, (GlobalPosition+ranVector).DirectionTo(player.GlobalPosition), 30, 0, false, this, null, false, true, false, 2000);
					
					await Task.Delay(800-i*(curHP > 75 ? 20 : 33));
					if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				}

				await Task.Delay(2000);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				//1 1 1 1, 1 1 1, 1 1 1 1 레이저
				await gm.SummonShine(GlobalPosition);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤

				gm.SummonBullet(6, GlobalPosition + new Vector2(-1024, -1200), Vector2.Down, 30, 0, false, this, null, true, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(-512, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(0, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(512, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(1024, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1200 : 600);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(-1280, -1200), Vector2.Down, 30, 0, false, this, null, true, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(-768, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(-256, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(256, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(768, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(1280, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				await Task.Delay(curHP > 75 ? 1200 : 600);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
				gm.SummonBullet(6, GlobalPosition + new Vector2(-1024, -1200), Vector2.Down, 30, 0, false, this, null, true, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(-512, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(0, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(512, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				gm.SummonBullet(6, GlobalPosition + new Vector2(1024, -1200), Vector2.Down, 30, 0, false, this, null, false, true, false, 2000);
				
				await Task.Delay(2500);
				if(player.dead || dead) break; //자신 혹은 플레이어 사망시 움직임 멈춤
			} 
		}
	}

	public async override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if(GlobalPosition.Y > cam.GlobalPosition.Y + 888 && ( id != 19 && id != 29 && id != 44 && id != 49))  QueueFree();//화면 지나가면 사라짐

		if(dead) //사망시기믹
		{
			//공통 기믹
			hpBar.Visible = false;
			if(id != 0 && id != 1 && id != 40 && id != 41 && curHP > -300000)
			{
				curHP = -999999; //중복 방지를 위한 트리거
				if(so1 != null) so1.QueueFree();
				if(so2 != null) so2.QueueFree();
				DieSound.Play();
			}

			//개별 기믹
			if((id == 1 || id == 40 || id == 41) && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				Node b = gm.SummonBullet(3, GlobalPosition, Vector2.Up, 20, 0, false, this, null, true, true, false, 900);
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

				await gm.Conversation(new string[]{"루시앙", "카우-BO2", "루시앙", "카우-BO2", "루시앙"},
				new string[][]{new string[]{"우리 엄마 어딨냐고?"},
				new string[]{"정..."},
				new string[]{"..."},
				new string[]{"글............", "차.......이...."},
				new string[]{"??", "...", "정글로 가보자", "(체력 회복력이 강화됐다.)"}});

				gm.Summon(91, cam.GlobalPosition + Vector2.Up*1000); //스테이지 클리어 글자
				gm.SummonStage3();
			}
			if(id == 30 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				GD.Print("Remove Because Dead");
				gm.RemoveBullet(	followBullet);
			}
			if(id == 39 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "로라니", "루시앙", "로라니", "루시앙"},
				new string[][]{new string[]{"알려달라고!!!!!!!"},
				new string[]{"사실 모름 ㅋ"},
				new string[]{"???    "},
				new string[]{"ㅋ    "},
				new string[]{"수상한 연구소가 보인다", "가보자"}});

				gm.Summon(92, cam.GlobalPosition + Vector2.Up*1000); //스테이지 클리어 글자
				gm.SummonStage4();
			}
			if(id == 43 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '킹'", "루시앙", "가디언-레플리카 '킹'", "루시앙", "가디언-레플리카 '킹'"},
				new string[][]{new string[]{"말 좀 똑바로 해봐 제발"},
				new string[]{"$$$$$%%%^(ㅌ;ㅌ;ㅌ;ㅌ;ㅌ;ㅌ;777777777???)"},
				new string[]{"......"},
				new string[]{"ㅅ;;;;;'''''>>ㄱㄷㅂㄷㅂ"},
				new string[]{"??", "드디어 뭔가 말하는 건가?"},
				new string[]{".    .        .                . ...........", "............................", ".................................."}});
				
				gm.scroll = true; //보스 사망시 스크롤 재개
				player.col.SetProcess(false);

				gm.Summon(0, cam.GlobalPosition + new Vector2(0, -800));
				gm.Summon(0, cam.GlobalPosition + new Vector2(200, -800));

				await Task.Delay(12000);
				gm.Summon(44, cam.GlobalPosition + new Vector2(0, -800));
			}
			if(id == 44 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '보이'", "루시앙", "가디언-레플리카 '보이'", "루시앙", "가디언-레플리카 '보이'"},
				new string[][]{new string[]{"아저씨 정신 좀 차려봐"},
				new string[]{"!...!...!..."},
				new string[]{"......"},
				new string[]{"!!!!!!!!!!!!!"},
				new string[]{"에휴"},
				new string[]{".......!..!...............!...."}});
				
				gm.scroll = true; //보스 사망시 스크롤 재개
				player.col.SetProcess(false);

				gm.Summon(0, cam.GlobalPosition + new Vector2(0, -800));
				gm.Summon(0, cam.GlobalPosition + new Vector2(200, -800));

				await Task.Delay(12000);
				gm.Summon(45, cam.GlobalPosition + new Vector2(0, -800));
			}
			if(id == 45 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "가디언-레플리카 '라니'", "루시앙", "가디언-레플리카 '라니'", "루시앙"},
				new string[][]{new string[]{"다들 제정신이 아니구만"},
				new string[]{"헤......비............"},
				new string[]{"헤비메탈이 뭐 어쨌다는 거야!!!!!!!"},
				new string[]{"......................ㅋ"},
				new string[]{"......   ", "저 앞에 뭐야??"}});
				
				gm.scroll = true; //보스 사망시 스크롤 재개
				player.col.SetProcess(false);

				gm.Summon(0, cam.GlobalPosition + new Vector2(0, -800));
				gm.Summon(0, cam.GlobalPosition + new Vector2(200, -800));

				await Task.Delay(12000);
				gm.Summon(49, cam.GlobalPosition + new Vector2(0, -800));
			}
			if(id == 49 && curHP > -3000000)
			{
				curHP = -999999999; //중복 방지를 위한 트리거

				await gm.Conversation(new string[]{"루시앙", "가디언-터미네이터", "루시앙", "가디언-터미네이터", "루시앙", "가디언-터미네이터", "가디언-터미네이터", "루시앙"},
				new string[][]{new string[]{"우리 엄마 어딨어!!!!!!!!!!!!"},
				new string[]{"5초 후 자폭.... 4... 3..."},
				new string[]{"????????!!!!!!!!!!!!!!"},
				new string[]{"2... 1.........."},
				new string[]{"으악!!!!!!!!!!!!!!!!!!!!!!!!!"},
				new string[]{". . . . . ."},
				new string[]{"농담이다.", "......................"},
				new string[]{".................................", "에휴 계속 뒤져봐야겠다"}});

				player.col.SetProcess(false);
				
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(gm.stageRsBlind, "modulate", new Color(0,0,0,1f), 1.5f);

				await Task.Delay(1000);
				ScoreContainer scoCon = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer; //점수 저장
				scoCon.lastScore = gm.score;
				
				await Task.Delay(1000);
				GetTree().ChangeSceneToFile("res://Scenes/epilogue.tscn"); //게임 클리어
			}	
			
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
			if(moveSpd == 0) return;

			moveSpd *= 1.1f;
			GlobalPosition += moveDir * moveSpd * (float)delta;
			if(followBullet != null ) followBullet.GlobalPosition += moveDir * moveSpd * (float)delta;
		}
		if(id == 92)
		{
			GlobalPosition += Vector2.Down * 400 * (float)delta;
		}
		if(id == 42)
		{
			if(moveSpd == 0) return;

			moveSpd *= 1.028f;
			moveDir += -0.06f * (moveDir - GlobalPosition.DirectionTo(player.GlobalPosition));

			GlobalPosition += moveDir * moveSpd * (float)delta;
			
			if(GlobalPosition.DistanceTo(player.GlobalPosition) <= range && !dead)
			{
				GetDamage(999999999);
				await gm.SummonShine(GlobalPosition);
				gm.SummonBullet(3, GlobalPosition, Vector2.Zero, 40, 0, false, this, null, true, true, false, 900);
			}
		}
		if(id == 44)
		{
			GlobalPosition += moveDir * moveSpd * (float)delta;
		}
		if(id == 49)
		{
			if(curHP <= 50)
			{
				Modulate = new Color(1, 0.8f, 0.8f, 1);
			}
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

		if(followBullet != null&& followBullet.ProcessMode == ProcessModeEnum.Disabled) followBullet = null;

		if(hpBar != null)
		{
			hpBar.MaxValue = maxHP;
			hpBar.Value = curHP;
		}
    }
}
