using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;
using ACF.Tests;
public class GoblinWarchief : AutoBattleUnit
{
    private GameObject axeSkill;
    private GameObject instanceAxeSkill;
    private GameObject slashSkill;
    private GameObject instanceSlashSkill;
    public int skillNum = 0;
    protected override void SetupStat()
    {
        string fileName = "GoblinWarchiefStat";
        //string path = Application.dataPath + "/Resources/Data/" + fileName + ".Json";
        string path = Application.streamingAssetsPath + "/Data/" + fileName + ".Json";
        string data = File.ReadAllText(path);
        stat = JsonUtility.FromJson<Stat>(data);
        axeSkill = Resources.Load<GameObject>("Prefabs/Skill&Attack/AxeSkill");
        slashSkill = Resources.Load<GameObject>("Prefabs/Skill&Attack/SlashSkill");
    }
    protected override void Skill()
    {
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
        //int rand = Random.Range(0, 3);
        //if (rand == 0)
        //{ 
        //    Animator.SetTrigger("Skill"); 
        //}
        //else if (rand == 1)
        //{
        //    Animator.SetTrigger("Skill2");
        //}
        //else if (rand == 2)
        //{
        //    Animator.SetTrigger("Skill3");
        //}
        if (skillNum %3 == 0)
        {
            Animator.SetTrigger("Skill");
        }
        else if (skillNum % 3 == 1)
        {
            Animator.SetTrigger("Skill2");
        }
        else if (skillNum % 3 == 2)
        {
            Animator.SetTrigger("Skill3");
        }
        skillNum++;
        //rand++;
    }
    protected override void SkillEvent()
    {
        base.SkillEvent();

        instanceAxeSkill = Instantiate(axeSkill) as GameObject;
        instanceAxeSkill.transform.position = this.transform.position + new Vector3(0f, 0.5f, 0f);
        instanceAxeSkill.transform.localEulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        instanceAxeSkill.GetComponent<AxeSkill>().myUnit = this.GetComponent<AutoBattleUnit>();
        instanceAxeSkill.GetComponent<AxeSkill>().isSlash = false;
        lastAttackTime = Time.time;
        isSkilling = false;
    }
    private void SkillEvent2()
    {
        base.SkillEvent();
        int randX = Random.Range(0, 8);
        StageManager.instance.SpawnGoblinWarrior(randX);
       
        lastAttackTime = Time.time;
        isSkilling = false;
    }
    private void SkillEvent3()
    {
        base.SkillEvent();

        instanceSlashSkill = Instantiate(slashSkill,this.transform) as GameObject;
        instanceSlashSkill.transform.position = this.transform.position + this.transform.forward+ new Vector3(0f, 0.5f, 0f);
        instanceSlashSkill.GetComponent<AxeSkill>().myUnit = this.GetComponent<AutoBattleUnit>();
        instanceSlashSkill.GetComponent<AxeSkill>().isSlash = true;
        instanceSlashSkill.GetComponent<AxeSkill>().targetPosition = targetUnit.transform.position + this.transform.forward*10f;

        lastAttackTime = Time.time;
        isSkilling = false;
    }
}