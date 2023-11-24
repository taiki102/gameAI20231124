using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
	public float life = 1;
	
	bool isInvincible = false;
	bool isHitted = false;
	// 移動速度
	Vector2 moveSpeed = Vector2.zero;
	// 向き： true の時に左向き、false の時に右向き
	bool facingLeft = true;
	// 自身のanimatorの保持
	Animator animator;
	// プレイヤー GameObject の保持
	GameObject player;
	// 出現位置
	Vector3 startPosition;
	
	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();
		player = GameObject.Find("Player");
		startPosition = transform.position;
		currentmove = MoveMent.None;
		currentmove = MoveMent.Targetting;
	}

	// Update is called once per frame
	void Update()
	{
		// 死亡チェック
		if (life <= 0) {
			animator.SetBool("IsDead", true);
			StartCoroutine(DestroyEnemy());
			// 死体GameObject を出してすぐに消えるという手もある
			return;
		}
		
		// ヒットストップ
		if(isHitted){
			return;
		}
		
		// ===================================================
		// プレイヤーに体当たりするAIを作りましょう
		
		// 例：プレイヤーの方を向く
		Vector3 player_position = player.transform.position;
		if(player_position.x > transform.position.x){
			facingLeft = false;
		}else{
			facingLeft = true;
		}
		
		//moveSpeed.x = -1.0f;    // 右が+、左が-になります
		//moveSpeed.y = -1.0f;    // 上が+、下が-になります
		
		// ===================================================
	}

	enum MoveMent
	{
		None,
		Targetting,
		Normal,
	}

	MoveMent movement;

	private MoveMent currentmove
    {
		get {return movement; }
		set
		{
			movement = value;
			if (value != movement)
			{
				StartCoroutine(AttackMotion(transform.position, player.transform.position));
			};
		}
    }
	
	// 自キャラの移動処理をまとめています
	void Movement(){
		// 向き処理
		GetComponent<SpriteRenderer>().flipX = facingLeft;
		// 移動処理
		Vector3 position = transform.position;
		position.x += moveSpeed.x * Time.deltaTime;
		position.y += moveSpeed.y * Time.deltaTime;
		// 簡易地形アタリ判定
		if(position.y < 0.5f){
			position.y = 0.5f;
		}
		transform.position = position;	
	}

	IEnumerator AttackMotion(Vector2 currentpos, Vector2 targetpos)
    {
		yield return new WaitForSeconds(1.0f);

		while (currentpos != targetpos)
		{
			// 向かう方向を計算
			Vector3 direction = (currentpos - targetpos).normalized;
			currentpos += direction * moveSpeed * Time.deltaTime;
			transform.position = currentpos;

			// 1フレーム待つ
			yield return null;
		}

		/*

		while (transform.position.x >= targetpos2.x && transform.position.y >= targetpos2.y)
        {
			position.x += moveSpeed.x * Time.deltaTime;
			position.y += moveSpeed.y * Time.deltaTime;
			yield return null;
        }

		yield return new WaitForSeconds(1.0f);

		while (transform.position.x >= targetpos2.x && transform.position.y >= targetpos2.y)
		{
			yield return null;

		}*/
	}
	
	// ダメージを受ける：プレイヤー側がコールする仕組みになっています
	public void ApplyDamage(float damage) {
		if (!isInvincible) 
		{
			// 攻撃を受けた方向が取れる仕組みになっています
			float direction = damage / Mathf.Abs(damage);
			damage = Mathf.Abs(damage);
			life -= damage;
			StartCoroutine(HitTime());
		}
	}
	
	// 無敵時間の設定 : WaitForSecondsで設定している間、isHittedとisInvinsibleをtrueにする
	IEnumerator HitTime()
	{
		isHitted = true;
		isInvincible = true;
		yield return new WaitForSeconds(0.5f);
		isHitted = false;
		isInvincible = false;
	}
	
	// プレイヤーとの接触時の処理
	void OnTriggerEnter2D(Collider2D collider)
	{
		// プレイヤーに体当たり攻撃
		if (collider.gameObject.tag == "Player" && life > 0)
		{
			collider.gameObject.GetComponent<CharacterController2D>().ApplyDamage(2f, transform.position);
		}
	}
	
	// 死亡処理
	IEnumerator DestroyEnemy()
	{
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
		EnemyManager.HasEnemy = true;
	}
	
}
