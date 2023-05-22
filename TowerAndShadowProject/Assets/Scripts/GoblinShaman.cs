using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;

public class GoblinShaman : AutoBattleUnit
{
    private GameObject fireBall;
    private GameObject instanceFireBall;
    private GameObject fireSkill;
    private GameObject instanceFireSkill;

    

    protected override void SetupStat()
    {
        string fileName = "GoblinShamanStat";
        //string path = Application.dataPath + "/Resources/Data/" + fileName + ".Json";
        string path = Application.streamingAssetsPath + "/Data/" + fileName + ".Json";
        string data = File.ReadAllText(path);
        stat = JsonUtility.FromJson<Stat>(data);
        fireBall = Resources.Load<GameObject>("Prefabs/Skill&Attack/FireBall");
        fireSkill = Resources.Load<GameObject>("Prefabs/Skill&Attack/FireSkill");
        lastAttackTime = -(1/stat.attackSpeed);

    }
    protected override void Start()
    {
        base.Start();
        if (gameObject.CompareTag("TeamPlayer"))
        {
            gameCode = (int)StageManager.GameCode.SHADOW_GOBLIN_SHAMAN;
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5.0f);
    }
    protected override void OnAttackEvent()
    {
        if (targetUnit != null)
        {
            //적을 attack시 MP 10회복
            if (stat.currentMP + stat.hitMP < stat.maxMP)
            {
                stat.currentMP += stat.hitMP;
            }
            else
            {
                stat.currentMP = stat.maxMP;
                isReadySkill = true;
            }
            mpBarSlider.fillAmount = stat.currentMP / stat.maxMP;
            //적을 attack시 fireBall 프리팹 생성

            instanceFireBall = Instantiate(fireBall) as GameObject;
            instanceFireBall.transform.position = new Vector3(transform.position.x, 0f, transform.position.z) + (transform.forward * 0.5f);
            instanceFireBall.GetComponent<FireBall>().myUnit = this.GetComponent<AutoBattleUnit>();
            //instanceFireBall.transform.SetParent(this.transform);
        }
        lastAttackTime = Time.time;
        isAttacking = false;
    }
    protected override void Skill()
    {
        //base.Skill();
        if (isSkilling)
        {
            return;
        }
        if (isAttacking)
        {
            return;
        }
        isSkilling = true;
        canMove = false;
        
        FindFarthestTarget();
       
        Animator.SetTrigger("Skill");
        //Debug.Log(farthestUnit);
    }
    private void SkillEvent2()
    {
        if (farthestUnit==null)
        {
            return;
        }
        this.transform.LookAt(farthestUnit.transform);
    }
    protected override void SkillEvent()
    {
        base.SkillEvent();
        //가장 먼 적에게 스킬 사용 시 fireSkill 프리팹 생성
        
        instanceFireSkill = Instantiate(fireSkill) as GameObject;
        instanceFireSkill.transform.position = new Vector3(transform.position.x, 0f, transform.position.z) + (transform.forward * 0.5f);
        instanceFireSkill.GetComponent<FireSkill>().myUnit = this.GetComponent<AutoBattleUnit>();

        lastAttackTime = Time.time;
        isSkilling = false;
    }

    
}