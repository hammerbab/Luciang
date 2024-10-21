using Godot;
using System;
using System.Threading.Tasks;

public partial class HealthObject : RigidBody2D
{
	public GameManager gm;

	public AudioStreamPlayer HitSound;
	public AudioStreamPlayer DieSound;

	[Export] public Node2D col;
	[Export] protected Sprite2D spr;

	[Export] public int maxHP;
	public int curHP;
	[Export] public float range;
	[Export] public int invin_time;
	public bool is_invin;
	protected bool player_effect_activate;

	public bool dead = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetTree().Root.GetNode<GameManager>("GameScene/GameManager");
		
		curHP = maxHP;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}

	//음수일 경우 회복
	public void GetDamage(int amount, bool ign_invin = false, bool no_invin = false)
	{
		if(amount <= 0) //회복
		{
			curHP -= amount;
			if(curHP >= maxHP) curHP = maxHP;
			return;
		}

		if(!ign_invin && is_invin) return; //무적상태면 안 맞음(ign_invin: 무적을 씹는 공격엔 무력)

		curHP -= amount; //체력 깎고
		player_effect_activate = true;
		if (curHP <= 0)
		{
			Die(); //0 이하 되면 사망
			return;
		}

		HitSound.Play();
		
		Tween tween = GetTree().CreateTween();
		if(no_invin || invin_time == 0) //매끄러운 색상 변경을 위해 무적 발동 여부에 따른 투명도 조정
		{
			tween.TweenProperty(spr, "modulate", Colors.Red, 0.06f);
			tween.TweenProperty(spr, "modulate", Colors.White, 0.06f);
		}
		else
		{
			tween.TweenProperty(spr, "modulate", new Color(1,0,0,0.5f), 0.06f);
			tween.TweenProperty(spr, "modulate", new Color(1,1,1,0.5f), 0.06f);

			Invincible(invin_time, tween); //no_invin: 무적타임을 발동시키지 않는 공격일 경우
		}
	}

	public async void Invincible(int dur, Tween tw) //무적타임
	{
		if(dur == 0) return;

		is_invin = true;

		tw.TweenProperty(spr, "modulate", new Color(1,1,1,0.5f), 0.05f); //투명화
		tw.TweenProperty(spr, "modulate", Colors.White, 0.05f).SetDelay(dur*0.001f); //투명화 종료

		await Task.Delay(dur);

		is_invin = false;
	}

	private async void Die()
	{
		GD.Print(Name, "Dead");

		dead = true;

		is_invin = true;

		DieSound.Play();
		
		col.Free();

		var tween = GetTree().CreateTween();
		tween.TweenProperty(spr, "modulate", new Color(1,0,0,0), 0.4f);
	}
}
