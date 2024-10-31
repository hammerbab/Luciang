using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : HealthObject
{
	Node GameScene;
	Camera2D cam;

	[Export] TextureRect hurtEffect;
	[Export] TextureRect ddalpiEffect;

	[Export] PackedScene g_o_Scene;

	RigidBody2D targetBody;
	SummonObject target;
	bool fireTrigger; //발사 트리거
	float maxNormalCool;
	float curNormalCool;
	public bool attackSeal = false; //공격봉인

	[Export] PackedScene moveCirclePrefab;
	[Export] Sprite2D DashEff;
	bool isDashing = false; //대쉬 중엔 이동 불가능
	bool haveToMove = false; //이동트리거
	Vector2 clickPoint; //마우스를 클릭한 지점
	Vector2 destination; //이동 목적지
	Vector2 moveDir; //움직일 방향 벡터
	float maxDashCool;
	float curDashCool;
	public bool autoMove = false; //스테이지 넘어가는 효과 동안 자동 이동

	[Export] float moveSpeed;

	[Export] TextureRect normalCoolBlind;
	[Export] TextureRect dashCoolBlind;

	private bool sceneChange = true;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

		base._Ready();
		cam = GetTree().Root.GetNode<Camera2D>("GameScene/Camera2D");
		GameScene = GetTree().Root.GetNode("GameScene");

		maxHP = curHP = 100;
	}

    public async override void _Process(double delta)
	{
		invin_automove = autoMove; //스테이지 넘어가는 연출 동안 무적


		if(attackSeal)
		{
			fireTrigger = false;
		}

		if(DieSound == null || HitSound == null)
		{
			DieSound = gm.DieSound;
			HitSound = gm.HitSound;
		}
		base._Process(delta);

		normalCoolBlind.Scale = attackSeal ? Vector2.One : new Vector2(1, curNormalCool >= maxNormalCool ? 0 : 1 - (curNormalCool / maxNormalCool));
		dashCoolBlind.Scale = new Vector2(1, curDashCool >= maxDashCool ? 0 : 1 - (curDashCool / maxDashCool));

		if(curHP <= 20 && ddalpiEffect.Visible == false)
		{
			ddalpiEffect.Visible = true;
			AnimationPlayer danim = ddalpiEffect.GetChild(0) as AnimationPlayer;
			danim.Play("Ddalpi");
		}
		else if(curHP > 20 && ddalpiEffect.Visible == true)
		{
			ddalpiEffect.Visible = false;
			AnimationPlayer danim = ddalpiEffect.GetChild(0) as AnimationPlayer;
			danim.Stop();
		} //딸피일 경우 긴장감을 주는 이펙트 활성화

		
		if(dead && sceneChange) //죽었다면 씬 변경. sceneChange: 씬 변경이 이미 실행되었을 경우 다시 실행 안되게 막는 요옫
		{
			sceneChange = false;
			Tween tween = GetTree().CreateTween();
			hurtEffect.Modulate = new Color(1,0,0,0f);
			tween.TweenProperty(hurtEffect, "modulate", new Color(1,0,0,0.5f), 0.4f); //이펙트
			await Task.Delay(2000);
			GetTree().ChangeSceneToPacked(g_o_Scene);
			return;
		}

		if(player_effect_activate) //데미지 입음 이펙트
		{
			Tween tween = GetTree().CreateTween();
			hurtEffect.Modulate = new Color(1,0,0,0);
			tween.TweenProperty(hurtEffect, "modulate",new Color(1,0,0,0.2f), 0.06f);
			tween.TweenProperty(hurtEffect, "modulate", new Color(1,0,0,0), 0.06f);
			player_effect_activate = false;
		}

		if(GlobalPosition.Y > cam.GlobalPosition.Y + 666 && !autoMove) //카메라 아래로 내려가면 데미지
		{
			haveToMove = false;
			ApplyImpulse(Vector2.Up * 33333);
			GetDamage(35);
		}

		curNormalCool += (float)delta;//쿨타임
		curDashCool += (float)delta;

		if(Input.IsActionJustPressed("clickR") && !isDashing)
		{
			clickPoint = GetGlobalMousePosition();

			targetBody = FindWithPos(clickPoint); //"타겟" 변경 전에 비교를 위해 먼저 "타겟바디"를 바꾼다

			if (targetBody == null) //맨땅을 클릭했다
			{
				destination = clickPoint;
				haveToMove = true;

				curNormalCool += 1f; //땅클릭을 많이 하면 더 빠르게 쏨

				Sprite2D mCirc = moveCirclePrefab.Instantiate<Sprite2D>(); //이동 표시 이펙트
				mCirc.GlobalPosition = destination;
				GameScene.AddChild(mCirc);
				mCirc.GetNode<AnimationPlayer>("AnimationPlayer").Play("move");
				await RemoveMCirc(mCirc);

				fireTrigger = false;
			}
			else //적을 클릭했다
			{
				if(attackSeal)
				{
					return;
				}

				if(target != null && IsInstanceValid(target) && !target.dead && targetBody as SummonObject != target) //이전에 타겟이 있었고 현재 타겟과 다르면 기존 타겟의 표시를 지움
				{
					target.DeCircleAnim();
				}
				
				target = targetBody as SummonObject; //"타겟"까지 변경
				target.CircleAnim();
				fireTrigger = true; //발사 트리거
			}
		}
		if(Input.IsActionJustPressed("clickL") && !isDashing && curDashCool >= maxDashCool) //대쉬
		{
			isDashing = true;
			haveToMove = false;
			
			clickPoint = destination = GetGlobalMousePosition();

			moveDir = GlobalPosition.DirectionTo(clickPoint).Normalized();
			LookAt(destination);

			curNormalCool += 1f; //대시하면 공속 초기화

			col.Disabled = true;
			is_invin = true;

			DashEff.Visible = true;
			
			var tw = GetTree().CreateTween();
			tw.TweenProperty(spr, "modulate", new Color(1,1,1,0.5f), 0.05f); //투명화
			tw.TweenProperty(spr, "modulate", Colors.White, 0.05f).SetDelay(0.2f); //투명화 종료

			await Task.Delay(200);

			DashEff.Visible = false;

			isDashing = false;
			is_invin = false;
			col.Disabled = false;
			curDashCool = 0f;
			maxDashCool = 2f;
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
		if(dead) return;

		if(autoMove)
		{
			haveToMove = false;
			isDashing = false;
			target = null;
			targetBody = null;
			fireTrigger = false;

			GlobalPosition += Vector2.Up * 477 * (float)delta;
			LookAt(Vector2.Up * int.MaxValue);
			return;
		}
		if(fireTrigger && targetBody != null && IsInstanceValid(targetBody)&& !target.dead && !attackSeal)
		{
			if(GlobalPosition.DistanceTo(target.GlobalPosition) <= range) //사거리 안이다
			{
				LookAt(target.GlobalPosition);

				if(curNormalCool >= maxNormalCool)
				{
					curNormalCool = 0;
					maxNormalCool = 0.6f;
					gm.SummonBullet(0, GlobalPosition, Vector2.Zero, 1, 3000f, true, this, target, true, false);
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
		if(isDashing)
		{
			MoveAndCollide(moveDir * moveSpeed * 4f * (float)delta);
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
