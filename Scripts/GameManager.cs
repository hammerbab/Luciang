using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*게임매니저에서 다루는 것
1. 카메라 이동
2. 적 소환
3. 스테이지 생성
4. 총알 관리
5. UI 관리
*/
public partial class GameManager : Node
{
	Node GameScene; //오브젝트 생성하고 붙일 패런트
	Node2D cam;
	Player player;

	[Export] PackedScene[] smnObjScenes;

	List<SummonObject> summonedObjs = new List<SummonObject>();

	[Export] PackedScene[] bulletPrefabs;

	[Export] Label scoreText;
	public float score;
	public float ScoreMult = 1f; //오르는 스코어의 계수(시간이 지날 수록 오름)



	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		GameScene = GetTree().Root.GetNode("GameScene");
		cam = GetTree().Root.GetNode<Node2D>("GameScene/Camera2D");
		player = GetTree().Root.GetNode<Player>("GameScene/Player");

		await Task.Delay(1);
		Summon(0, Vector2.Up * 300f);

		RandomSummon(); //임시
	}

	async void RandomSummon()
	{
		RandomNumberGenerator random = new RandomNumberGenerator();
		while(true)
		{
			if(player.dead) return;
			await Task.Delay(random.RandiRange(3000, 6000));

			Summon(0, cam.GlobalPosition + new Vector2(random.RandfRange(-800, 800), random.RandfRange(-700, -1100)));
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if((player.dead)) //죽었다면 특수 글자로 변경
		{
			StringBuilder sb = new StringBuilder("Life: 0/100").AppendLine().Append("Score: ").Append(score.ToString("0.00"));
			scoreText.Text = sb.ToString();
			return;
		}
		
		cam.GlobalPosition +=Vector2.Up * (float)delta * 150f;
		ScoreMult += (float)delta * 0.001f;

		ChangeScore((float)delta * 3f);
	}

	public void ChangeScore(float amount)
	{
		score += amount * ScoreMult;

		StringBuilder sb = new StringBuilder("Life: ").Append(player.curHP).Append("/").AppendLine(player.maxHP.ToString()).Append(" Score: ").Append(score.ToString("0.00"));
		scoreText.Text = sb.ToString();
	}

	protected void Summon(int id, Vector2 pos)
	{
		SummonObject smndObj = smnObjScenes[id].Instantiate<SummonObject>();
		smndObj.GlobalPosition = pos;
		GameScene.AddChild(smndObj);

		smndObj.player = player;
		smndObj.gm = this;

		smndObj.Move();
	}

	/// <summary>
	/// dir의 경우 Normalize를 하지 않고 넣어야 됨
	/// target의 경우 타겟팅일 경우 지정해야 되고 아니라면 null로
	/// </summary>
	public void SummonBullet(int id, Vector2 globPos, Vector2 dir, int dmg, float spd, bool isT, HealthObject owner, HealthObject target, bool isEnemy = true)
	{
		Bullet blt = bulletPrefabs[id].Instantiate<Bullet>();
		blt.GlobalPosition = blt.originPos = globPos;
		GameScene.AddChild(blt);

		blt.dir = dir.Normalized();
		blt.dmg = dmg;
		blt.spd = spd;
		blt.isTargeting = isT;
		blt.target = target as SummonObject;
		blt.owner = owner;
		blt.isEnemy = isEnemy;
		blt.player = this.player;

		blt.gm = this;
	}
}
