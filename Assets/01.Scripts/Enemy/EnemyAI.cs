using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    private Animator animator;

    public float attackDist = 5.0f; // 공격거리
    public float traceDis = 10.0f;  // 쫓아가는거리

    public bool isDie = false;

    private WaitForSeconds ws;// 코루틴 지연시간 변수

    private MoveAgent moveAgent;

    private EnemyFire enemyFire;

    // 애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");


    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null) playerTr = player.GetComponent <Transform>();

        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();  
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();

        ws = new WaitForSeconds(0.3f);
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        // 적 캐릭터 사망할때까지 무한루프
        while(!isDie)
        {
            yield return ws;

            switch(state)
            {
                case State.PATROL:
                    // 총알 발사 정지
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    if(enemyFire.isFire == false) enemyFire.isFire = true;
                    break;
                case State.DIE:
                    moveAgent.Stop();
                    break;
            }

        }
    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if(dist <= attackDist)      state = State.ATTACK;
            else if(dist <= traceDis)   state = State.TRACE;
            else                        state = State.PATROL;

            yield return ws;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}
