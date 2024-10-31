using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*게임매니저에서 다루는 것
1. 카메라 이동
2. 적 소환
3. 풀링
4. 스테이지 적 생성
5. 총알 관리
6. UI 관리
7. 대화 관리
*/
public partial class GameManager : Node
{
	[Export] Node GameScene; //오브젝트 생성하고 붙일 패런트
	[Export] Camera2D cam;

	[Export] public AudioStreamPlayer[] ShootSound;
	[Export] public AudioStreamPlayer HitSound;
	[Export] public AudioStreamPlayer DieSound;

	public bool scroll = true;
	[Export] Texture2D[] bg_imgs;
	[Export] Sprite2D bg1;
	[Export] Sprite2D bg2;
	[Export] Player player;

	int bgCount=1;

	[Export] PackedScene[] smnObjScenes;

	List<SummonObject> summonedObjs = new List<SummonObject>();

	[Export] PackedScene[] bulletPrefabs;

	[Export] Label scoreText;
	public float score;

	int summonCount;
	int summonIndex;

	[Export] Conversation convBox;
	public bool pause = false;

	[Export] public TextureRect stageRsBlind; //스테이지 넘어갈 때 화면 전환, 페이드인 페이드아웃
	[Export] TextureRect approachBlind; //스테이지마다 처음 적과 조우할 때, 보스와 조우할 때 페이드인 페이드아웃

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{

		ChangeScore(0);

		stageRsBlind.Visible = true;

		await Task.Delay(1);
		MakePool();

		ScoreContainer sc = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer;
		if(sc.stageID == 1)
		{
			SummonStage1(); //1스테이지부터 소환 시작 (2스테이지부터는 실행 책임이 보스에게 넘어감)
		}
		else if(sc.stageID == 2)
		{
			SummonStage2(); //디버깅용
		}
		else if(sc.stageID == 3)
		{
			SummonStage3(); //디버깅용
		}
		else if(sc.stageID == 4)
		{
			SummonStage4(); //디버깅용
		}
	}

	//풀링
	Bullet[,] smndBlts;
	private void MakePool()
	{
		stageRsBlind.Modulate = new Color(0,0,0,1f);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,0f), 2f);

		smndBlts = new Bullet[bulletPrefabs.GetLength(0), 150];
		for(int i = 0; i < smndBlts.GetLength(0)-2; i++)
		{
			for(int ii = 0; ii < 150; ii++)
			{
				Bullet blt = bulletPrefabs[i].Instantiate<Bullet>();
				blt.ProcessMode = ProcessModeEnum.Disabled;
				blt.Visible = false;
				blt.cam = cam;
				smndBlts[i, ii] = blt;
				GameScene.AddChild(blt);
			}
		}
		for(int i = smndBlts.GetLength(0)-2; i < smndBlts.GetLength(0); i++)
		{
			for(int ii = 0; ii < 10; ii++)
			{
				Bullet blt = bulletPrefabs[i].Instantiate<Bullet>();
				blt.ProcessMode = ProcessModeEnum.Disabled;
				blt.Visible = false;
				blt.cam = cam;
				smndBlts[i, ii] = blt;
				GameScene.AddChild(blt);
			}
		}
	}

	public void appFadeIn()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(approachBlind, "modulate", new Color(0,0,0,0.4f), 0.5f);
	}

	public void appFadeOut()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(approachBlind, "modulate", new Color(0,0,0,0f), 0.4f);
	}

	async void SummonStage1()
	{
		player.attackSeal = true;


		await Task.Delay(2000);

		await Conversation(new string[]{"루시앙", "루시앙"},
		new string[][]
		{
			new string[]{"안녕", "나는 루시앙이다", "(우클릭으로 넘기기, 좌클릭으로 스킵)"},
			new string[]{"우리 엄마를 찾아줘", "(우클릭으로 이동, 좌클릭으로 대쉬)", "(화면 아래쪽 끝에 닿지 말자)"}
		});

		await Task.Delay(4000);
		Summon_Short(10, 0, true);
		await Task.Delay(1000);
		appFadeIn(); //페이드인
		await Task.Delay(2000);

		await Conversation(new string[]{"루시앙", "샌드백", "루시앙", "샌드백"},
		new string[][]
		{
			new string[]{"거기", "우리 엄마 어딨는지 알아??"},
			new string[]{"....(난 샌드백이야 무식한 놈아)"},
			new string[]{"뭐래는 거야", "죽어라!!!"},
			new string[]{"......(나를 우클릭하면 공격이다)", ".....(이동 또는 대시하면 공격을 빨리 할 수 있다)", "...(참고로 나도 공격한다)"}
		});
		player.attackSeal = false;
		appFadeOut(); //페이드아웃

		await Task.Delay(2000);
		Summon_Short(10, 0);
		await Task.Delay(5000);
		Summon_Short(10, 400);
		await Task.Delay(5000);
		Summon_Short(10, -200);
		Summon_Short_RandomX(0);
		await Task.Delay(5000);
		Summon_Short(10, 300);
		Summon_Short(10, -300);
		await Task.Delay(5000);
		Summon_Short(10, 500);
		Summon_Short(10, 100);
		await Task.Delay(300);
		Summon_Short(0, -100);
		await Task.Delay(5000);
		Summon_Short(10, -400);
		await Task.Delay(2000);
		Summon_Short(10, -100);
		await Task.Delay(5000);
		Summon_Short(10, 700);
		Summon_Short(10, 900);
		await Task.Delay(5000);
		Summon_Short(11, -300);
		Summon_Short(0, 100);
		await Task.Delay(5000);
		Summon_Short(10, 200);
		Summon_Short(11, 0);
		await Task.Delay(5000);
		Summon_Short(11, 700);
		await Task.Delay(5000);
		Summon_Short(11, 500);
		await Task.Delay(5000);
		Summon_Short(11, -1000);
		Summon_Short(11, 1000);
		Summon_Short(0, 0);
		await Task.Delay(5000);
		Summon_Short(10, 1000);
		Summon_Short(10, -1000);
		Summon_Short(11, 0);

		await Task.Delay(12000);
		player.attackSeal = true;
		Summon_Short(19, 0);
		appFadeIn(); //페이드인
	}

	public async void SummonStage2()
	{
		player.attackSeal = true;
		player.curHP = 100;

		//
		//화면전환
		player.autoMove = true;
		player.col.SetProcess(false);

		await Task.Delay(3981);

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,1f), 1.5f);
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,0f), 2f).SetDelay(2f);
		await Task.Delay(2000);

		bg1.Texture=bg_imgs[1];
		bg2.Texture=bg_imgs[1];
		player.autoMove = false;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		
		player.col.SetProcess(true);
		//화면전환 종료
		//

		await Task.Delay(3000);
		Summon_Short(1, 300);
		Summon_Short(1, -300);
		await Task.Delay(1500);
		Summon_Short(20, 0, true);
		appFadeIn();
		await Task.Delay(3000);

		await Conversation(new string[]{"루시앙", "황야의 무법자", "루시앙", "황야의 무법자"},
		new string[][]
		{
			new string[]{"해적이다!!!", "너네들이 우리 엄마 납치했구나!!!!!!!!!!!!"},
			new string[]{"내가 어딜 봐서 해적이야"},
			new string[]{"몰라", "죽어라!!!"},
			new string[]{"??? (화약통을 조심해라)"}
		});
		player.attackSeal = false;
		appFadeOut();

		await Task.Delay(3000);
		Summon_Short_RandomX(1);
		await Task.Delay(1000);
		Summon_Short_RandomX(20);
		await Task.Delay(4000);
		Summon_Short(20, 300);
		Summon_Short(1, 0);
		await Task.Delay(3500);
		Summon_Short(1, 800);
		Summon_Short(1, -800);
		await Task.Delay(2000);
		Summon_Short(21, 0);
		await Task.Delay(3500);
		Summon_Short_RandomX(1);
		Summon_Short_RandomX(0);
		await Task.Delay(2000);
		Summon_Short(20, -800);
		await Task.Delay(500);
		Summon_Short(21, 0);
		await Task.Delay(5000);
		Summon_Short(1, 1000);
		Summon_Short(1, 800);
		Summon_Short(1, 600);
		Summon_Short(1, 400);
		Summon_Short(1, 200);
		Summon_Short(1, 0);
		Summon_Short(1, -200);
		Summon_Short(1, -400);
		Summon_Short(1, -600);
		Summon_Short(1, -800);
		Summon_Short(1, -1000);
		await Task.Delay(2000);
		Summon_Short(21, 0);
		await Task.Delay(3000);
		Summon_Short(20, 0);
		Summon_Short_RandomX(0);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(3000);
		Summon_Short_RandomX(20);
		await Task.Delay(5000);
		Summon_Short_RandomX(0);
		Summon_Short(1, 0);
		await Task.Delay(2500);
		Summon_Short(21, 300);
		await Task.Delay(2500);
		Summon_Short(1, 0);
		await Task.Delay(2500);
		Summon_Short(21, -200);
		await Task.Delay(2500);
		Summon_Short(1, 0);
		await Task.Delay(2500);
		Summon_Short(21, 200);
		await Task.Delay(2500);
		Summon_Short(1, 0);
		await Task.Delay(5000);
		Summon_Short(1, 1000);
		Summon_Short(1, 800);
		Summon_Short(1, 600);
		Summon_Short(1, 400);
		Summon_Short(1, 200);
		Summon_Short(1, 0);
		Summon_Short(1, -200);
		Summon_Short(1, -400);
		Summon_Short(1, -600);
		Summon_Short(1, -800);
		Summon_Short(1, -1000);
		await Task.Delay(2000);
		Summon_Short_RandomX(0);
		Summon_Short(21,0);
		Summon_Short(20, -1000);
		Summon_Short(20, 1000);

		
		await Task.Delay(12000);
		player.attackSeal = true;
		Summon_Short(29, 0);
		appFadeIn(); //페이드인
	}

	public async void SummonStage3()
	{
		player.attackSeal = true;
		player.curHP = 100;

		//
		//화면전환
		player.autoMove = true;
		player.col.SetProcess(false);

		await Task.Delay(3981);

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,1f), 1.5f);
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,0f), 2f).SetDelay(2f);
		await Task.Delay(2000);

		bg1.Texture=bg_imgs[2];
		bg2.Texture=bg_imgs[2];
		player.autoMove = false;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;

		player.col.SetProcess(true);
		//화면전환 종료
		//

		await Task.Delay(4444);
		Summon_Short(30,0,true);
		await Task.Delay(1000);
		appFadeIn();
		await Task.Delay(2000);

		await Conversation(new string[]{"고라니", "루시앙", "고라니", "루시앙"},
		new string[][]
		{
			new string[]{"아아악!!!!!!"},
			new string[]{"???", "아아악!!!!!!!!!!!"},
			new string[]{"아아악!!!!!!!!!!!!!!!!!!"},
			new string[]{"아아악!!!!!!!!!!!!!!!!!!!!!!!!!", "!!!!!!!!!!!!!!!!!!!!!!!", "!!!!!!!!!!!!!!!!!!!!!"},
		});
		player.attackSeal = false;
		appFadeOut();

		await Task.Delay(3000);
		Summon_Short(30, -400);
		Summon_Short(30, 400);
		await Task.Delay(1000);
		Summon_Short(31, 0);
		await Task.Delay(6000);
		Summon_Short(31, 200 );
		Summon_Short(31, -700);
		await Task.Delay(6000);
		Summon_Short(31, 0 );
		Summon_Short(31, 600);
		await Task.Delay(6000);
		Summon_Short(30, 0 );
		Summon_Short(30, 300);
		Summon_Short(30, 600 );
		Summon_Short(30, 900);
		Summon_Short(30, -300 );
		Summon_Short(30, -600 );
		Summon_Short(30, -900);
		await Task.Delay(6000);
		Summon_Short(31, 900 );
		Summon_Short(30, 600);
		Summon_Short(30, 300 );
		Summon_Short(30, 0);
		Summon_Short(30, -300 );
		Summon_Short(30, -600 );
		Summon_Short(31, -900);
		await Task.Delay(6000);
		Summon_Short(0, 0);
		Summon_Short(30, 300);
		await Task.Delay(2000);
		Summon_Short_RandomX(30);
		await Task.Delay(1900);
		Summon_Short_RandomX(30);
		Summon_Short_RandomX(32);
		await Task.Delay(1800);
		Summon_Short_RandomX(30);
		await Task.Delay(1600);
		Summon_Short_RandomX(30);
		await Task.Delay(1300);
		Summon_Short_RandomX(30);
		await Task.Delay(800);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(300);
		Summon_Short_RandomX(30);
		await Task.Delay(6000);
		Summon_Short_RandomX(32);
		Summon_Short_RandomX(0);
		await Task.Delay(1200);
		Summon_Short_RandomX(30);
		Summon_Short_RandomX(30);
		await Task.Delay(1500);
		Summon_Short(32, 900);
		Summon_Short_RandomX(31);
		await Task.Delay(6000);
		Summon_Short(32, -900);
		Summon_Short(32, 900);
		Summon_Short(31, 0);
		await Task.Delay(1500);
		Summon_Short_RandomX(30);
		Summon_Short_RandomX(30);
		await Task.Delay(1100);
		Summon_Short_RandomX(0);
		Summon_Short_RandomX(31);
		await Task.Delay(12000);
		player.attackSeal = true;
		Summon_Short(39, 0);
		appFadeIn(); //페이드인
	}

	public async void SummonStage4()
	{
		player.attackSeal = true;
		player.curHP = 100;

		//
		//화면전환
		player.autoMove = true;
		player.col.SetProcess(false);

		await Task.Delay(3981);

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,1f), 1.5f);
		tween.TweenProperty(stageRsBlind, "modulate", new Color(0,0,0,0f), 2f).SetDelay(2f);
		await Task.Delay(2000);

		bg1.Texture=bg_imgs[3];
		bg2.Texture=bg_imgs[3];
		player.autoMove = false;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;
		await Task.Delay(1);
		player.GlobalPosition = cam.GlobalPosition;

		player.col.SetProcess(true);
		//화면전환 종료
		//

		await Task.Delay(4444);
		Summon_Short(40,0,true);
		await Task.Delay(1000);
		appFadeIn();
		await Task.Delay(2000);

		await Conversation(new string[]{"루시앙", "가디언-레이저", "루시앙", "가디언-레이저"},
		new string[][]
		{
			new string[]{"이 연구소가 우리 엄마를 납치한 범인이구만!"},
			new string[]{"침입자. 처단한다."},
			new string[]{"뭐? 한 주먹거리도 안 돼 보이는 게!!!!!!!!!!!!"},
			new string[]{"소란스럽다. 처단한다."},
		});
		player.attackSeal = false;
		appFadeOut();

		await Task.Delay(5000);
		Summon_Short(40, 400);
		Summon_Short(40, -400);
		await Task.Delay(5000);
		Summon_Short(40, 200);
		await Task.Delay(2000);
		Summon_Short(40, -500);
		await Task.Delay(5000);
		Summon_Short(42, 0);
		Summon_Short(41, -900);
		Summon_Short(41, 900);
		await Task.Delay(6000);
		Summon_Short(41, -600);
		Summon_Short(41, 600);
		await Task.Delay(6000);
		Summon_Short_RandomX(0);
		Summon_Short_RandomX(0);
		Summon_Short(40, -900);
		Summon_Short(41, 900);
		await Task.Delay(1600);
		Summon_Short(42, 800);
		await Task.Delay(1600);
		Summon_Short(42, -800);
		await Task.Delay(1600);
		Summon_Short(42, 800);
		await Task.Delay(1600);
		Summon_Short(42, -800);
		await Task.Delay(5000);
		Summon_Short(40, 0);
		await Task.Delay(1000);
		Summon_Short(41, 700);
		await Task.Delay(1000);
		Summon_Short(41, 1000);
		await Task.Delay(6000);
		Summon_Short(42, -1000);
		await Task.Delay(800);
		Summon_Short(42, -600);
		await Task.Delay(1600);
		Summon_Short(42, -200);
		await Task.Delay(1600);
		Summon_Short(41, 1000);
		Summon_Short(42, 200);
		await Task.Delay(1600);
		Summon_Short(42, 600);
		await Task.Delay(1600);
		Summon_Short(42, 1000);
		Summon_Short_RandomX(0);
		Summon_Short_RandomX(0);
		await Task.Delay(10000);
		player.attackSeal = true;
		Summon_Short(43, 0);
		appFadeIn(); //페이드인
	}

	void Summon_Short(int id, float X, bool conv = false)
	{
		Summon(id, cam.GlobalPosition + new Vector2(X, -800), conv);
	}

	void Summon_Short_RandomX(int id)
	{
		RandomNumberGenerator random = new RandomNumberGenerator();
		Summon(id, cam.GlobalPosition + new Vector2(random.RandfRange(-800, 800), -800));
	}

	public override void _Process(double delta)
	{
		if(!player.dead) //체력과 스코어 표시
		{
			StringBuilder sb = new StringBuilder("HP: ").Append(player.curHP).Append("/").AppendLine(player.maxHP.ToString()).Append("점수: ").Append(score.ToString("0"));
			scoreText.Text = sb.ToString();
		}
		else
		{
			StringBuilder sb = new StringBuilder("HP: DEAD").AppendLine().Append("점수: ").Append(score.ToString("0"));
			
			ScoreContainer scoCon = GetTree().Root.GetNode("ScoreContainer") as ScoreContainer; //점수 저장
			scoCon.lastScore = score;

			scoreText.Text = sb.ToString();
			return;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!scroll) return;
		cam.GlobalPosition +=Vector2.Up * (float)delta * 150f;

		if(cam.GlobalPosition.Y < bgCount*-1800+1800) //배경 스크롤
		{
			if(bgCount%2 == 0)
			{
				bg1.GlobalPosition = Vector2.Up*(bgCount*1800);
			}
			else
			{
				bg2.GlobalPosition = Vector2.Up*(bgCount*1800);
			}
			bgCount++;
		}
	}

	public void ChangeScore(float amount)
	{
		score += amount;
	}

	public SummonObject Summon(int id, Vector2 pos, bool conv = false)
	{
		SummonObject smndObj = smnObjScenes[id].Instantiate<SummonObject>();
		smndObj.GlobalPosition = pos;
		GameScene.AddChild(smndObj);

		smndObj.player = player;
		smndObj.gm = this;
		smndObj.HitSound = HitSound;
		smndObj.DieSound = DieSound;
		smndObj.conv = conv;

		smndObj.Move();

		return smndObj;
	}

	/// <summary>
	/// target의 경우 타겟팅일 경우 지정해야 되고 아니라면 null로
	/// </summary>
	public Bullet SummonBullet(int id, Vector2 globPos, Vector2 dir, int dmg, float spd, bool isT, HealthObject owner, HealthObject target = null, bool soundEff = true, bool isEnemy = true, bool removeImidiately = true, int remainTime = int.MaxValue)
	{
		Bullet blt = null;

		int len = 150;
		if(id == 8 || id == 9) len = 10;
		
		for(int i = 0; i < len; i++)
		{
			if(smndBlts[id, i].ProcessMode == ProcessModeEnum.Disabled)
			{
				blt = smndBlts[id, i];
				blt.GlobalPosition = blt.originPos = globPos;
				blt.ProcessMode = ProcessModeEnum.Pausable;
				blt.Visible = true;

				break;
			}
		}
		if(blt == null)
		{
			return null;
		}

		blt.id = id;
		blt.dir = dir.Normalized();
		blt.dmg = dmg;
		blt.spd = spd;
		blt.isTargeting = isT;
		blt.target = target as SummonObject;
		blt.owner = owner;
		blt.isEnemy = isEnemy;
		blt.player = this.player;
		blt.removeImidiately = removeImidiately;
		blt.remainTime = remainTime;

		blt.gm = this;
		blt.cam = this.cam;

		blt.time = 0f;
		blt.canBeRmvd = false;

		blt.Spawned();

		if(soundEff)
		{
			ShootSound[id].Play();
		}

		
		if(blt.GetChildCount() >= 3)
		{
			AnimationPlayer ap;
			if(blt.GetChild(2).Name == "AnimationPlayer")
			{
				ap = blt.GetChild(2) as AnimationPlayer;
				ap.Stop();
				if(blt.id != 8) ap.Play("Bullet");
				else ap.Play(remainTime == 2000 ? "Clock" : "RClock");
			}
		}

		return blt;
	}

	public void RemoveBullet(Bullet target)
	{
		CallDeferred("DeferRemBlt", target);
	}
	private void DeferRemBlt(Bullet target)
	{
		target.Scale = Vector2.One;
		if(target.id == 3) target.Scale*=2.3f;
		if(target.id == 4) target.Scale*=0.5f;
		if(target.id == 6) target.Scale*=-1f;
		if(target.id == 7) target.Scale*=1.2f;

		if(target.id == 8)
		{
			for(int i = 0; i<8; i++)
			{
				var a = target.GetChild(0);
				var b = a.GetChild<Bullet>(i);
				b.time = 0f;
				b.canBeRmvd = false;
				b.ProcessMode = ProcessModeEnum.Disabled;
				b.Visible = false;
			}
		}
		

		target.time = 0f;
		target.canBeRmvd = false;
		target.ProcessMode = ProcessModeEnum.Disabled;
		target.Visible = false;
	}

	public async Task SummonShine(Vector2 pos, float size = 1f)
	{
		Bullet s = SummonBullet(9, pos, Vector2.Zero, 0, 0, false, null, null, true, true, false, 1000);
		Sprite2D sp = s.GetChild(0) as Sprite2D;
		sp.Scale = Vector2.One * size;
		AnimationPlayer ap = s.GetChild(1) as AnimationPlayer;
		ap.Play("Shine");
		await Task.Delay(300);
	}

	public async Task Conversation(string[] name, string[][] text, bool pause = true, bool autoskip = false, int charDelay = 50)
	{
		this.pause = pause;
		await convBox.Conv(name, text, pause, autoskip, charDelay);
		this.pause = false;
	}
}
