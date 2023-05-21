using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
using UnityEngine.UI;
using Rito.BehaviorTree;
using static Rito.BehaviorTree.NodeHelper;
using ACF.Tests;
using System;
public class AutoBattleUnit : MonoBehaviour, ICore
{
    protected INode _rootNode;

    private LayerMask whatIsTarget; 
    public AutoBattleUnit targetUnit;
    public AutoBattleUnit farthestUnit;
    public Vector3 farthestUnitPosition;
    protected NavMeshAgent pathFinder;
    protected Animator Animator;
    public int gameCode = -1;
    
    public class Stat
    {
        public string name;
        public float height;
        public int level;
        public float attackDamage;
        public float abilityPower;
        public float attackSpeed;
        public float attackRange;
        public float skillRange;
        public float maxHP;
        public float maxMP;
        public float currentHP;
        public float currentMP;
        public float beHitMP;
        public float hitMP;
        public float shield;
        public float armor;
        public float magicResistance;
        public Vector3 position;
        public Vector3 rotation;
    }
    public Stat stat = new Stat();
    public Stat statOrigin = new Stat();
    public Transform tr;

    protected float lastAttackTime;
    private float dist;
    private Vector3 targetDir = Vector3.zero;

    protected GameObject statusPrefab;
    public GameObject levelImage;
    protected Canvas uiCanvas;
    protected HealthBarTest hpBarSlider;
    public Image mpBarSlider;

    public GameObject status;
    protected AutoBattleUnit[] targetTeamUnits;
    public string targetName;
    protected bool canMove;
    protected bool canTargeting=true;
    protected bool isAttacking=false;
    protected bool isSkilling=false;
    public bool isReadySkill=false;
    public bool isDie = false; //{ get; protected set; } //사망 상태
    public bool isSpawn = false;
    public bool isStunned = false;//스턴스킬에 맞았는 지 여부
    public bool isStunning = false;
    public GameObject stunnedText;

    IEnumerator myCoroutine;

    private bool hasTarget
    {
        get
        {
            if (targetUnit != null && !targetUnit.isDie)
                return true;
            return false;
        }
    }
    protected bool isTargetInAttackRange
    {
        get
        {
            if (hasTarget && dist < stat.attackRange)
                return true;
            return false;
        }
    }
    protected bool isTargetInSkillRange
    {
        get
        {
            if (hasTarget && dist < stat.skillRange)
                return true;
            return false;
        }
    }

    private bool canAttack
    {
        get
        {
            if (lastAttackTime + (1/stat.attackSpeed) <= Time.time)
                return true;
            return false;
        }
    }
    public bool CanTargeting()
    {
        return canTargeting;
    }
    public bool CanAttack()
    {
        return canAttack;
    }
    public bool IsTargetInAttackRange()
    {
        return isTargetInAttackRange;
    }
    public bool IsTargetInSkillRange()
    {
        return isTargetInSkillRange;
    }
    public bool IsDie()
    {
        return isDie;
    }
    public bool HasTarget()
    {
        return hasTarget;
    }
    public bool IsReadySkill()
    {
        return isReadySkill;
    }
    public bool IsStunned()
    {
        return isStunned;
    }
    protected virtual void MakeNode()
    {
        _rootNode =
            Selector
            (
                Sequence//죽었을때
                (
                    IfAction(IsDie, Die)
                ),
                Selector//안죽었을때
                (
                    Selector
                    (
                        Sequence//cc맞은 상태면 cc함수 실행
                        (
                            IfAction(IsStunned, executeStunnedEffect)
                        ),
                        Sequence//타겟없으면->타겟찾기
                        (
                            IfNotAction(HasTarget, FindTarget),
                            Action(LookTarget)
                        ),

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
                            Action(LookTarget),
                            Action(Attack)                       
                        )
                    ),
                    Sequence//타겟 추적
                    (
                        Action(ChaseTarget)
                    )
                )
            );
    }


    protected virtual void Awake()
    {
        isDie = false;
        isAttacking = false;
        tr = GetComponent<Transform>();
        SetupStat();
        pathFinder = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        if (gameObject.CompareTag("TeamEnemy"))
        {
            whatIsTarget = LayerMask.GetMask("TeamPlayer");
            targetName = "TeamPlayer";
            stat.rotation = new Vector3(0f, 180f, 0f);
        }
        else //if (gameObject.CompareTag("TeamPlayer"))
        {
            whatIsTarget = LayerMask.GetMask("TeamEnemy");
            targetName = "TeamEnemy";
            stat.rotation = new Vector3(0f, 0f, 0f);
        }
        targetTeamUnits = GameObject.Find(targetName).GetComponentsInChildren<AutoBattleUnit>();
        pathFinder.enabled = false;

        if (stat.currentMP >= stat.maxMP)
        {
            isReadySkill = true;
        }
        else
        {
            isReadySkill = false;
        }

        MakeNode();
        uiSetup();
    }
    protected virtual void SetupStat()
    {
        stat.height = 2.0f;
        stat.level = 1;
        stat.attackDamage = 50f;
        stat.abilityPower = 0f;
        stat.attackSpeed = 0.5f;
        stat.attackRange = 2f;
        stat.skillRange =5f;
        stat.maxHP = 1000f;
        stat.maxMP = 100f;
        stat.currentHP = stat.maxHP;
        stat.currentMP = 0f;
        stat.beHitMP = 5f;
        stat.hitMP = 10f;
        stat.shield = 0f;
        stat.armor = 0f;
        stat.magicResistance =0f;

        statOrigin.rotation = new Vector3(tr.rotation.x, tr.rotation.y, tr.rotation.z);

    }
    public void SaveStatOrigin()
    {
        statOrigin.level = stat.level;
        statOrigin.attackDamage = stat.attackDamage;
        statOrigin.abilityPower = stat.abilityPower;
        statOrigin.attackSpeed = stat.attackSpeed;
        statOrigin.attackRange = stat.attackRange;
        statOrigin.skillRange = stat.skillRange;
        statOrigin.maxHP = stat.maxHP;
        statOrigin.maxMP = stat.maxMP;
        statOrigin.currentHP = stat.currentHP;
        statOrigin.currentMP = stat.currentMP;
        statOrigin.beHitMP = stat.beHitMP;
        statOrigin.hitMP = stat.hitMP;
        statOrigin.shield = stat.shield;
        statOrigin.armor = stat.armor;
        statOrigin.magicResistance = stat.magicResistance;
        statOrigin.position = tr.position;
        statOrigin.rotation = stat.rotation;
    }
    public void LoadStatOrigin()
    {
        stat.level = statOrigin.level;
        stat.attackDamage = statOrigin.attackDamage;
        stat.abilityPower = statOrigin.abilityPower;
        stat.attackSpeed = statOrigin.attackSpeed;
        stat.attackRange = statOrigin.attackRange;
        stat.skillRange = statOrigin.skillRange;
        stat.maxHP = statOrigin.maxHP;
        stat.maxMP = statOrigin.maxMP;
        stat.currentHP = statOrigin.currentHP;
        stat.currentMP = statOrigin.currentMP;
        stat.beHitMP = statOrigin.beHitMP;
        stat.hitMP = statOrigin.hitMP;
        stat.shield = statOrigin.shield;
        stat.armor = statOrigin.armor;
        stat.magicResistance = statOrigin.magicResistance;

        tr.position = statOrigin.position;
        tr.rotation = Quaternion.Euler(statOrigin.rotation);
    }
    public void updateStatus()
    {
        hpBarSlider.shield = stat.shield;
        hpBarSlider.maxHP = stat.maxHP;
        hpBarSlider.currentHP = stat.maxHP;
        mpBarSlider.fillAmount = stat.currentMP / stat.maxMP;
    }
    public virtual void InitUnit()
    {
        if(!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }

        if (!pathFinder.isStopped)
        {
            pathFinder.isStopped = true;
        }
        pathFinder.velocity = Vector3.zero;
        pathFinder.enabled = false;
        targetUnit = null;
        canMove = false;
        Animator.SetBool("CanMove", canMove);
        isAttacking = false;
        isSkilling = false;
        
        if (stat.currentMP  >= stat.maxMP)
        {
            isReadySkill = true;
        }
        else
        {
            isReadySkill = false;
        }
        isDie = false;
        LoadStatOrigin();
        if (!status.activeSelf)
        { 
            status.SetActive(true); 
        }
        stat.shield = 0;
        updateStatus();
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        isStunned = false;
        isStunning = false;
        stunnedText.SetActive(false);
    }
    public virtual void uiSetup()
    {
        statusPrefab = Resources.Load<GameObject>("Prefabs/UIStatBar");
        uiCanvas = GameObject.Find("uiCanvas").GetComponent<Canvas>();
        status = Instantiate<GameObject>(statusPrefab, uiCanvas.transform);
        var _status = status.GetComponent<StatusUi>();
        _status.enemyTr = this.gameObject.transform;
        _status.enemyHeight = stat.height;

        hpBarSlider = status.transform.Find("UIHealthBar").GetComponent<HealthBarTest>();

        mpBarSlider = status.transform.Find("UIManaBar").Find("Mana").GetComponent<Image>();
        updateStatus();
        if (gameObject.CompareTag("TeamEnemy"))
        {
            status.transform.Find("UIHealthBar").Find("Hp").GetComponent<Image>().color = new Color(0.8f, 0.0f, 0.0f);
        }
        levelImage = status.transform.Find("LevelImage").gameObject;
        stunnedText = status.transform.Find("StunnedText").gameObject;
    }
    public void LevelUp()
    {
        levelImage.transform.Find("1star").gameObject.SetActive(false);
        levelImage.transform.Find("2star").gameObject.SetActive(true);
        stat.maxHP *= 2.0f;
        stat.currentHP = stat.maxHP;
        stat.attackDamage *= 2.0f;
        stat.abilityPower *= 1.5f;
        stat.attackSpeed *= 1.2f;
        stat.height = 2.5f;
        status.GetComponent<StatusUi>().enemyHeight = stat.height;
        stat.level += 1;
        this.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        SaveStatOrigin();
        updateStatus();
    }
    protected virtual void Start()
    {
        myCoroutine = Run();
        if(isSpawn)
        {
            StartCombat();
        }
    }

    private IEnumerator Run()
    {
        while (true)
        {
            Animator.SetBool("CanMove", canMove);
            if (hasTarget)
            {
                dist = Vector3.Distance(tr.position, targetUnit.tr.position);
            }
            _rootNode.Run();
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void StartCombat()
    {
        targetTeamUnits = GameObject.Find(targetName).GetComponentsInChildren<AutoBattleUnit>();
        pathFinder.enabled = true;
        StartCoroutine(myCoroutine);
    }
    public void EndCombat()
    {
        StopCoroutine(myCoroutine);
        targetTeamUnits = null;
    }


    protected virtual void Update()
    {
          
    }

    protected void LookTarget()
    {
        if(!hasTarget)
        {
            return;
        }
        this.transform.LookAt(targetUnit.tr);
    }
    protected void StopChase()
    {      
        canMove = false;
        pathFinder.isStopped = true;
        pathFinder.velocity = Vector3.zero;
    }
    protected void ChaseTarget()
    {
        canMove = true;        
        pathFinder.isStopped = false;
        pathFinder.SetDestination(targetUnit.tr.position);
    }

    protected void FindTarget()
    {
        targetTeamUnits = GameObject.Find(targetName).GetComponentsInChildren<AutoBattleUnit>();
        if (targetTeamUnits.Length == 0)
        {
            return;
        }
        float shortDist = float.PositiveInfinity;
        int shortNum = -1;
        for (int i = 0; i < targetTeamUnits.Length; i++)
        {

            if (!targetTeamUnits[i].gameObject.activeSelf || targetTeamUnits[i].isDie || !targetTeamUnits[i].canTargeting)
            { 
                continue; 
            }
            Vector3 offset = targetTeamUnits[i].tr.position - tr.position;
            float tempDist = offset.sqrMagnitude;
            if (tempDist < shortDist)
            {
                shortNum = i;
                shortDist = tempDist;
            }
        }
        if (shortNum==-1)
        {
            targetUnit = null;
        }
        else
        {
            targetUnit = targetTeamUnits[shortNum];
        }
    }
    protected void FindFarthestTarget()
    {
        float longDist = 0f;
        int longNum = 0;
        for (int i = 0; i < targetTeamUnits.Length; i++)
        {

            if (targetTeamUnits[i].gameObject.activeSelf == false || targetTeamUnits[i].isDie)
            {
                continue;
            }
            Vector3 offset = targetTeamUnits[i].tr.position - tr.position;
            float tempDist = offset.sqrMagnitude;
            if (tempDist > longDist)
            {
                longNum = i;
                longDist = tempDist;
            }
        }
        AutoBattleUnit livingUnit = targetTeamUnits[longNum];
        if (livingUnit != null && !livingUnit.isDie)
        {
            farthestUnit = livingUnit;
            farthestUnitPosition = livingUnit.tr.position;
        }
        else
        {
            farthestUnit = null;
        }
    }


    protected virtual void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        if (isAttacking)
        {
            return;
        }
        isAttacking = true;
        Animator.SetTrigger("Attack");
    }
    protected virtual void Skill()
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
        
        Animator.SetTrigger("Skill");
    }
    protected virtual void OnAttackEvent()
    {
        lastAttackTime = Time.time;
        isAttacking = false;

        if (targetUnit == null) return;
        if (targetUnit.isDie) return;

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
        //적을 attack시 적 OnDamage메서드 실행
        targetUnit.OnDamage(stat.attackDamage);     
    }
    protected virtual void SkillEvent()
    {
        isSkilling = false;
        stat.currentMP = 0f;
        mpBarSlider.fillAmount = stat.currentMP / stat.maxMP;
        isReadySkill = false;
    }
    public void OnDamage(float damage)
    {
        //적에게 attack 당할 시 공격 데미지 만큼 HP감소, MP 5회복
        if (stat.shield > 0f)
        {
            if (stat.shield >= damage)
            {
                stat.shield -= damage;
            }
            else
            {
                stat.currentHP -= (damage - stat.shield);
                stat.shield = 0f;
            }
        }

        else
        {
            stat.currentHP -= damage;
        }
        stat.currentMP += stat.beHitMP;

        if (stat.currentMP + stat.beHitMP < stat.maxMP)
        {
            stat.currentMP += stat.beHitMP;
        }
        else
        {
            stat.currentMP = stat.maxMP;
            isReadySkill = true;
        }
        if (stat.currentHP <= 0 && !isDie)
        {
            isDie = true;
        }
        hpBarSlider.shield = stat.shield;
        hpBarSlider.currentHP = stat.currentHP;
        mpBarSlider.fillAmount = stat.currentMP / stat.maxMP;
    }


    public void Die()
    {
        pathFinder.isStopped = true;
        if(stat.shield>0)
        {
            stat.shield = 0;
        }
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }      
        Animator.SetTrigger("Die");
        StopCoroutine(myCoroutine);
    }
    protected virtual void DieEvent()
    {
        gameObject.SetActive(false);
        status.SetActive(false);

    }
    public void executeStunnedEffect()
    {
        if(isStunning)
        {
            return;
        }
        isStunning = true;
        Animator.SetTrigger("Stunned");
        stunnedText.SetActive(true);
    }
    public void StunnedEvent()
    {
        isStunned = false;
        isStunning = false;
        stunnedText.SetActive(false);
        if(isSkilling)
        {
            isSkilling = false;
        }
        if (isAttacking)
        {
            isAttacking = false;
        }
    }
}