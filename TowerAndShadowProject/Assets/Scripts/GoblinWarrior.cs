using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;

public class GoblinWarrior : AutoBattleUnit
{
    protected override void SetupStat()
    {
        string fileName = "GoblinWarriorStat";
        //string path = Application.dataPath + "/Resources/Data/" + fileName + ".Json";
        string path = Application.streamingAssetsPath + "/Data/" + fileName + ".Json";
        string data = File.ReadAllText(path);
        stat = JsonUtility.FromJson<Stat>(data);

    }
    protected override void Start()
    {
        base.Start();
        if (gameObject.CompareTag("TeamPlayer"))
        {
            gameCode = (int)StageManager.GameCode.SHADOW_GOBLIN_WARRIOR;
        }
    }
    protected override void SkillEvent()
    {
        base.SkillEvent();
        if (!isDie)
        {
            stat.shield += 300f;
            hpBarSlider.shield = stat.shield;
        }
        
    }
}