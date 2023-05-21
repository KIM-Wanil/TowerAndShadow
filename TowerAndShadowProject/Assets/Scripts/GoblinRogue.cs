using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;
using System;

public class GoblinRogue : AutoBattleUnit
{
    private bool isInfiltrating = false;
    private Vector3 startPos, endPos;
    //땅에 닫기까지 걸리는 시간
    protected float timer;
    protected float timeToFloor;

    protected override void SetupStat()
    {
        string fileName = "GoblinRogueStat";
        string path = Application.dataPath + "/Resources/Data/" + fileName + ".Json";
        string data = File.ReadAllText(path);
        stat = JsonUtility.FromJson<Stat>(data);
    }
    protected override void Start()
    {
        base.Start();
        if (gameObject.CompareTag("TeamPlayer"))
        {
            gameCode = (int)StageManager.GameCode.SHADOW_GOBLIN_ROGUE;
        }
    }
    protected override void MakeNode()
    {
        _rootNode =
            Selector
            (
                Sequence
                (
                    IfAction(IsDie, Die)
                ),
                Selector//안죽었을때
                (
                    Sequence//타겟없으면->타겟찾기
                    (
                        IfNotAction(CanTargeting, StartInfiltrate)
                    ),
                    Sequence//타겟없으면->타겟찾기
                    (
                        IfNotAction(HasTarget, FindTarget),
                        Action(LookTarget)
                    ),
                    Selector
                    (
                        Sequence//스킬이 준비된 상태일때 타겟이 범위 안에있으면 스킬 사용, 범위 안에 없으면 추적
                        (
                            Condition(IsTargetInSkillRange),
                            Condition(IsReadySkill),
                            Action(StopChase),
                            Action(Skill)
                        ),
                        Sequence//타겟이 공격 범위 안에 있고, 공격이 가능한 상태면 공격
                        (
                            Condition(IsTargetInAttackRange),
                            Action(StopChase),
                            Action(Attack)
                        )
                    ),
                    Sequence//가장 가까운 타겟 업데이트하면서 타겟 추적
                    (
                        Action(ChaseTarget)
                    )
                )
            );
    }
    protected override void Awake()
    {
        base.Awake();
        canTargeting = false;
    }
    private void StartInfiltrate()
    {
        if (isInfiltrating)
        {
            //StartCoroutine("Infiltrate");
            return;
        }
        pathFinder.enabled = false;
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        isInfiltrating = true;
        float zPos = 0f;
        if (targetName == "TeamPlayer")
        {
            zPos = 0f;
        }
        else //if(targetName=="TeamEnemy")
        {
            zPos = 9f;
        }
        //Vector3 pos = new Vector3(transform.position.x, transform.position.y, zPos);
        startPos = transform.position;
        endPos = new Vector3(transform.position.x, transform.position.y, zPos);
        StartCoroutine("Infiltrate");
    }
    private IEnumerator Infiltrate()
    {

        Animator.SetTrigger("Jump");
        timer = 0;
        while (transform.position.y >= startPos.y)
        {
            timer += Time.deltaTime;
            Vector3 tempPos = Parabola(startPos, endPos, 1, timer);
            transform.position = tempPos;
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.002f);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("이동 끝");
        yield return new WaitForSeconds(0.2f);
        isInfiltrating = false;
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        
        canTargeting = true;
        pathFinder.enabled = true;
        
        yield break;
    }
    //포물선 이동 코드 출처 : https://ajh322.tistory.com/273
    protected static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    protected override void SkillEvent()
    {
        base.SkillEvent();
        if(targetUnit==null)
        {
            return;
        }
        if (targetUnit.isDie)
        {
            return;
        }
        targetUnit.OnDamage(stat.abilityPower);
        if (!targetUnit.isStunned && !targetUnit.isStunning)
        {
            targetUnit.isStunned = true;
        }
    }
}