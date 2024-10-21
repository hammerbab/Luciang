using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*게임매니저에서 다루는 것
1. 카메라 이동
2. 적 소환
3. 스테이지 적 생성
4. 총알 관리
5. UI 관리
6. 대화 관리
*/
public partial class GameManager : Node
{
	Node GameScene; //오브젝트 생성하고 붙일 패런트
	Camera2D cam;

	[Export] public AudioStreamPlayer[] ShootSound;
	[Export] public AudioStreamPlayer HitSound;
	[Export] public AudioStreamPlayer DieSound;

	public bool scroll = true;
	[Export] Texture2D[] bg_imgs;
	[Export] Sprite2D bg1;
	[Export] Sprite2D bg2;
	Player player;

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

	[Export] TextureRect stageRsBlind; //스테이지 넘어갈 때 화면 전환, 페이드인 페이드아웃

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		GameScene = GetTree().Root.GetNode("GameScene");
		cam = GetTree().Root.GetNode<Camera2D>("GameScene/Camera2D");
		player = GetTree().Root.GetNode<Player>("GameScene/Player");

		ChangeScore(0);

		await Task.Delay(1);
		SummonStage1(); //1스테이지부터 소환 시작 (2스테이지부터는 실행 책임이 보스에게 넘어감)
	}

	async void SummonStage1()
	{
		player.attackSeal = true;


		await Task.Delay(2000);

		await Conversation(new string[]{"루시앙", "루시앙"},
		new string[][]
		{
			new string[]{"안녕", "나는 루시앙이다", "(클릭으로 넘기기)"},
			new string[]{"우리 엄마를 찾아줘", "(우클릭으로 이동, 좌클릭으로 대쉬)"}
		});

		await Task.Delay(4000);
		Summon_Short(10, 0);
		await Task.Delay(3000);

		await Conversation(new string[]{"루시앙", "샌드백", "루시앙", "샌드백"},
		new string[][]
		{
			new string[]{"거기", "우리 엄마 어딨는지 알아??"},
			new string[]{"....(난 샌드백이야 무식한 놈아)"},
			new string[]{"뭐래는 거야", "죽어라!!!"},
			new string[]{"...(...)", "......(나를 우클릭하면 공격이다)"}
		});
		player.attackSeal = false;

		await Task.Delay(2000);
		Summon_Short(10, 0);
		await Task.Delay(5000);
		Summon_Short(10, 400);
		await Task.Delay(5000);
		Summon_Short(10, -200);
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
		await Task.Delay(1000);
		Summon_Short(10, -300);
		await Task.Delay(1000);
		Summon_Short(10, -200);
		await Task.Delay(3000);
		Summon_Short(10, 700);
		Summon_Short(10, 900);
		await Task.Delay(3000);
		Summon_Short(11, -300);
		Summon_Short(0, 100);
		await Task.Delay(3000);
		Summon_Short(10, 200);
		Summon_Short(11, 0);
		
		await Task.Delay(12000);
		player.attackSeal = true;
		Summon_Short(19, 0);
	}

	public async void SummonStage2()
	{
		player.attackSeal = true;
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
		player.GlobalPosition = cam.GlobalPosition + Vector2.Down * 600;
		await Task.Delay(2000);

		player.autoMove = false;
		player.col.SetProcess(true);
		//화면전환 종료
		//

		await Task.Delay(5000);
		Summon_Short(20, 0);
		await Task.Delay(3000);

		await Conversation(new string[]{"루시앙", "황야의 무법자", "루시앙", "황야의 무법자"},
		new string[][]
		{
			new string[]{"거기 해적씨 우리 엄마 어딨는지 알아??"},
			new string[]{"내가 어딜 봐서 해적이야"},
			new string[]{"몰라", "죽어라!!!"},
			new string[]{"???"}
		});
		player.attackSeal = false;

		await Task.Delay(3000);
		
		await Task.Delay(12000);
		player.attackSeal = true;
		Summon_Short(29, 0);
	}

	public async void SummonStage3()
	{
		player.attackSeal = true;

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
		player.GlobalPosition = cam.GlobalPosition + Vector2.Down * 600;
		await Task.Delay(2000);

		player.autoMove = false;
		player.col.SetProcess(true);
		//화면전환 종료
		//

		await Task.Delay(5000);
		Summon_Short_RandomX(30);
		await Task.Delay(3000);

		await Conversation(new string[]{"고라니", "루시앙", "고라니", "루시앙"},
		new string[][]
		{
			new string[]{"아아악!!!!!!"},
			new string[]{"아아악!!!!!!!!!!!"},
			new string[]{"아아악!!!!!!!!!!!!!!!!!!"},
			new string[]{"아아악!!!!!!!!!!!!!!!!!!!!!!!!!", "!!!!!!!!!!!!!!!!!!!!!!!", "!!!!!!!!!!!!!!!!!!!!!"},
		});
		player.attackSeal = false;

		await Task.Delay(3000);
		Summon_Short_RandomX(31);
		await Task.Delay(5000);
		Summon_Short_RandomX(32);
		await Task.Delay(12000);
		player.attackSeal = true;
		Summon(39, cam.GlobalPosition + new Vector2(0, -800));
	}

	void Summon_Short(int id, float X)
	{
		Summon(id, cam.GlobalPosition + new Vector2(X, -800));
	}

	void Summon_Short_RandomX(int id)
	{
		RandomNumberGenerator random = new RandomNumberGenerator();
		Summon(id, cam.GlobalPosition + new Vector2(random.RandfRange(-800, 800), -800));
	}

	public override void _Process(double delta)
	{
		if(!player.dead) //체력
		{
			StringBuilder sb = new StringBuilder("Life: ").Append(player.curHP).Append("/").AppendLine(player.maxHP.ToString()).Append(" Score: ").Append(score.ToString("0"));
			scoreText.Text = sb.ToString();
		}
		else
		{
			StringBuilder sb = new StringBuilder("Life: 0/100").AppendLine().Append("Score: ").Append(score.ToString("0"));
			
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

	public SummonObject Summon(int id, Vector2 pos)
	{
		SummonObject smndObj = smnObjScenes[id].Instantiate<SummonObject>();
		smndObj.GlobalPosition = pos;
		GameScene.AddChild(smndObj);

		smndObj.player = player;
		smndObj.gm = this;
		smndObj.HitSound = HitSound;
		smndObj.DieSound = DieSound;

		smndObj.Move();

		return smndObj;
	}

	/// <summary>
	/// target의 경우 타겟팅일 경우 지정해야 되고 아니라면 null로
	/// </summary>
	public Bullet SummonBullet(int id, Vector2 globPos, Vector2 dir, int dmg, float spd, bool isT, HealthObject owner, HealthObject target = null, bool soundEff = true, bool isEnemy = true, bool removeImidiately = true, int remainTime = int.MaxValue)
	{
		Bullet blt = bulletPrefabs[id].Instantiate<Bullet>();
		blt.GlobalPosition = blt.originPos = globPos;
		GameScene.AddChild(blt);

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

		blt.Spawned();

		if(soundEff)
		{
			ShootSound[id].Play();
		}

		return blt;
	}

	public async Task Conversation(string[] name, string[][] text, bool pause = true, bool autoskip = false, int charDelay = 50)
	{
		this.pause = pause;
		await convBox.Conv(name, text, pause, autoskip, charDelay);
		this.pause = false;
	}
}
