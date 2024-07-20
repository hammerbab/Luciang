using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : HealthObject
{
	Node GameScene;
	Camera2D cam;
	GameManager gm;

	[Export] PackedScene g_o_Scene;

	RigidBody2D targetBody;
	SummonObject target;
	bool fireTrigger; //발사 트리거
	float maxCool;
	float curCool;

	[Export] PackedScene moveCirclePrefab;
	bool haveToMove = false; //이동트리거
	Vector2 clickPoint; //마우스를 클릭한 지점
	Vector2 destination; //이동 목적지
	Vector2 moveDir; //움직일 방향 벡터

	[Export] float moveSpeed;

	private bool sceneChange = true;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

		base._Ready();
		cam = GetTree().Root.GetNode<Camera2D>("GameScene/Camera2D");
		GameScene = GetTree().Root.GetNode("GameScene");
		gm = GetTree().Root.GetNode<GameManager>("GameScene/GameManager");

		maxHP = curHP = 100;
	}

    public async override void _Process(double delta)
	{
		base._Process(delta);

		if(dead && sceneChange) //죽었다면 씬 변경. sceneChange: 씬 변경이 이미 실행되었을 경우 다시 실행 안되게 막는 요옫
		{
			sceneChange = false;
			await Task.Delay(2000);
			GetTree().ChangeSceneToPacked(g_o_Scene);
			return;
		}

		if(GlobalPosition.Y > cam.GlobalPosition.Y + 666) //카메라 아래로 내려가면 데미지
		{
			haveToMove = false;
			ApplyImpulse(Vector2.Up * 30000);
			GetDamage(30);
		}

		curCool += (float)delta;//쿨타임

		if(Input.IsActionJustPressed("click"))
		{
			clickPoint = GetGlobalMousePosition();

			targetBody = FindWithPos(clickPoint); //"타겟" 변경 전에 비교를 위해 먼저 "타겟바디"를 바꾼다

			if (targetBody == null) //맨땅을 클릭했다
			{
				destination = clickPoint;
				haveToMove = true;

				Sprite2D mCirc = moveCirclePrefab.Instantiate<Sprite2D>(); //이동 표시
				mCirc.GlobalPosition = destination;
				GameScene.AddChild(mCirc);
				mCirc.GetNode<AnimationPlayer>("AnimationPlayer").Play("move");
				await RemoveMCirc(mCirc);

				fireTrigger = false;
			}
			else //적을 클릭했다
			{
				if(target != null && IsInstanceValid(target) && !target.dead && targetBody as SummonObject != target) //이전에 타겟이 있었고 현재 타겟과 다르면 기존 타겟의 표시를 지움
				{
					target.DeCircleAnim();
				}
				
				target = targetBody as SummonObject; //"타겟"까지 변경
				target.CircleAnim();
				fireTrigger = true; //발사 트리거
			}
		}
	}

	async Task RemoveMCirc(Sprite2D mCirc)
	{
		await Task.Delay(200);
		mCirc.QueueFree();
	}

	protected RigidBody2D FindWithPos(Vector2 pos)
	{
		//클릭 지점에 레이캐스트
		var spaceState = GetWorld2D().DirectSpaceState;
    	var query = PhysicsRayQueryParameters2D.Create(pos, pos+Vector2.One*0.01f, CollisionMask=1);
		query.Exclude = new Godot.Collections.Array<Rid> { GetRid() };
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		query.HitFromInside = true;
    	var result = spaceState.IntersectRay(query);
		//여기까지 레이캐스트

        //없으면 null
        if(!result.ContainsKey("collider")) return null;

        RigidBody2D obj = (RigidBody2D) result["collider"];
        return obj;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(fireTrigger && targetBody != null && IsInstanceValid(targetBody))
		{
			if(GlobalPosition.DistanceTo(target.GlobalPosition) <= range) //사거리 안이다
			{
				LookAt(target.GlobalPosition);

				if(curCool >= maxCool)
				{
					curCool = 0;
					maxCool = 0.4f;
					gm.SummonBullet(0, GlobalPosition, Vector2.Zero, 30, 3000f, true, this, target, false);
				}
				
				haveToMove = false;
			}
			else //아직 더 걸어가야 한다
			{
				destination = target.GlobalPosition; //이동방향을 적에게로 덮어씌움
				haveToMove = true;
			}
		}
		if(haveToMove) //이동
		{
			moveDir = GlobalPosition.DirectionTo(destination).Normalized();
			MoveAndCollide(moveDir * moveSpeed * (float)delta);
			LookAt(destination);

			if(!fireTrigger && GlobalPosition.DistanceTo(destination) <= 5) //목적지 도달
			{
				haveToMove = false;
				//GlobalPosition = destination;
			}
		}
	}

	private void OnBodyEntered(Node other)
	{
		if(other.IsInGroup("EnemyBullet"))
		{
			var bullet = other as Bullet;
			GetDamage(bullet.dmg);
		}
	}
}
