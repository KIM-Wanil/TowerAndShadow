using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;

public class Player : AutoBattleUnit
{
    private GameObject darkSkill;
    private GameObject instanceDarkSkill;
    public class SkillEnhancement
    {
        public string skillName; //스킬 이름
        public bool isEnhanced; //스킬 강화 여부

        public SkillEnhancement(string name)
        {
            skillName = name;
            isEnhanced = false;
        }

        public void Enhance()
        {
            isEnhanced = true;
        }
    }
    public SkillEnhancement shield = new SkillEnhancement("Shield");
    public SkillEnhancement explosion = new SkillEnhancement("Explosion");
    public SkillEnhancement stun = new SkillEnhancement("Stun");

    protected override void SetupStat()
    {
        string fileName = "PlayerStat";
        string path = Application.dataPath + "/Resources/Data/" + fileName + ".Json";
        string data = File.ReadAllText(path);
        stat = JsonUtility.FromJson<Stat>(data);
        darkSkill = Resources.Load<GameObject>("Prefabs/Skill&Attack/DarkSkill");
    }

    protected override void Update()
    {
        
    }

    protected override void SkillEvent()
    {
        base.SkillEvent();
        if(shield.isEnhanced)
        {
            stat.shield += stat.maxHP/10;
            hpBarSlider.shield = stat.shield;
        }
        instanceDarkSkill = Instantiate(darkSkill) as GameObject;
        instanceDarkSkill.transform.position = this.transform.position + new Vector3(0f, 0.5f, 0f);//new Vector3(transform.position.x, 0f, transform.position.z) + (transform.forward * 0.5f);
        instanceDarkSkill.transform.localEulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        instanceDarkSkill.GetComponent<DarkSkill>().myUnit = this.GetComponent<AutoBattleUnit>();
        lastAttackTime = Time.time;
        isSkilling = false;
    }
}