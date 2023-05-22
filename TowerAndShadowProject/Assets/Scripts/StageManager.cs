using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
public class StageManager : Singleton<StageManager>
{
    public GameObject TeamEnemy;
    private AutoBattleUnit[] TeamEnemyUnits;
    public GameObject TeamPlayer;
    public GameObject TeamPlayerBench;
    private AutoBattleUnit[] TeamPlayerUnits;
    private AutoBattleUnit[] TeamPlayerBenchUnits;
    public AutoBattleUnit Player;
    private GameObject goblinWarrior;
    private GameObject goblinShaman;
    private GameObject goblinRogue;
    private GameObject goblinWarchief;
    public GameObject instanceEnemyUnit;
    public GameObject shadowGoblinWarrior;
    public GameObject shadowGoblinShaman;
    public GameObject shadowGoblinRogue;
    public GameObject shadowSphere;
    public GameObject instanceShadowUnit;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject clearPanel;
    public GameObject pickPanel;
    public GameObject tutorialPanel;
    public GameObject systemWindow;
    public GameObject tutorialWindow;
    public GameObject tutorialWindowButton;
    public GameObject startButton;
    public GameObject nextButton;
    public GameObject pickButton;
    
    public GameObject levelUpButton;
    public GameObject shop;

    public GameObject buttonPrefab;
    public Sprite goblinWarriorImage;
    public Sprite goblinShamanImage;
    public Sprite goblinRogueImage;
    public Sprite shieldImage;
    public Sprite explosionImage;
    public Sprite stunImage;
    public Sprite hpImage;
    public Sprite mpImage;
    public Sprite adImage;
    public Sprite asImage;
    public Sprite shadowSphereImage;


    public GameObject[] shopButtons;
    Vector2[] shopButtonsPos;

    public GameObject skillPanel;
    public GameObject skillButton;
    public GameObject[] skillButtons;
    public int spawnSkillIndex = 0;
    public int passiveSkillIndex = 4;
    Vector2[] skillButtonsPos;

    public GameObject buffPanel;
    public GameObject buffIcon;
    public GameObject[] buffIcons;
    public int buffIndex = 0;
    Vector2[] buffIconsPos;

    public bool isPause = false;
    public enum GameCode { NONE = -1, SHADOW_GOBLIN_WARRIOR , SHADOW_GOBLIN_SHAMAN , SHADOW_GOBLIN_ROGUE , SHADOW_SPHERE,
        ADD_SHIELD, ADD_EXPLOSION, ADD_STUN ,
        ENHANCE_HP , ENHANCE_MP, ENHANCE_AD, ENHANCE_AS, AD_BUFF_SKILL, AS_BUFF_SKILL, HP_BUFF_SKILL, MP_BUFF_SKILL };
    
    RaycastHit hit, hitLayerMask;

    public int stageCount { get; set; }
    public int gold { get; set; }

    public int maxShadowPoint { get; set; }
    public int currentShadowPoint { get; set; }

    public int maxUnitCount { get; set; }
    
    public int currentUnitCount { get; set; }
    public int flag = 0;

    public class ShopItem
    {
        public ShopItem()
        {
        }
        public ShopItem(GameCode Code,Action<Transform,ShopItem> Action1, Action<Transform, ShopItem> Action2, string Name, string Info, int GoldCost,int ManaCost, Sprite Image)
        {
            code = Code;
            PickShopAction = Action1;
            UseSkillAction = Action2;
            name = Name;
            info = Info;
            goldCost = GoldCost;
            manaCost = ManaCost;
            image = Image; 
        }
        public GameCode code;
        public Action<Transform,ShopItem> PickShopAction;
        public Action<Transform, ShopItem> UseSkillAction;
        public string name;
        public string info;
        public int goldCost;
        public int manaCost;
        public Sprite image;


    }
    ShopItem ShadowGoblinWarriorItem = new ShopItem();
    ShopItem ShadowGoblinShamanItem = new ShopItem();
    ShopItem ShadowGoblinRogueItem = new ShopItem();
    ShopItem AddShieldToSkillItem = new ShopItem();
    ShopItem AddExplosionToSkillItem = new ShopItem();
    ShopItem AddStunToSkillItem = new ShopItem();
    ShopItem EnhanceHpItem = new ShopItem();
    ShopItem EnhanceMpItem = new ShopItem();
    ShopItem EnhanceAdItem = new ShopItem();
    ShopItem EnhanceAsItem = new ShopItem();

    ShopItem AdBuffSkillItem = new ShopItem();
    ShopItem AsBuffSkillItem = new ShopItem();
    ShopItem HpBuffSkillItem = new ShopItem();
    ShopItem MpBuffSkillItem = new ShopItem();
    ShopItem ShadowSphereItem = new ShopItem();
    public List<GameCode> RemainingActiveSkills = new List<GameCode>();
    public List<GameCode> RemainingPassiveSkills = new List<GameCode>();
    public List<GameCode> UsedPassiveSkills = new List<GameCode>();
    public List<GameCode> EnhanceablStats = new List<GameCode>();

    public string[] tutorialMessages;
    public TextMeshProUGUI systemText;
    public TextMeshProUGUI tutorialText;

    private int currentMessageIndex = 0;
    private bool isTutorial = true;

    // Start is called before the first frame update
    void Start()
    {
        tutorialPanel = GameObject.Find("Canvas").transform.Find("TutorialPanel").gameObject;
        systemWindow = tutorialPanel.transform.Find("SystemWindow").gameObject;
        tutorialWindow = tutorialPanel.transform.Find("TutorialWindow").gameObject;
        tutorialWindowButton = tutorialWindow.transform.Find("Button").gameObject;
        startButton = GameObject.Find("Canvas").transform.Find("StartButton").gameObject;
        victoryPanel = GameObject.Find("Canvas").transform.Find("VictoryPanel").gameObject;
        defeatPanel = GameObject.Find("Canvas").transform.Find("DefeatPanel").gameObject;
        clearPanel = GameObject.Find("Canvas").transform.Find("ClearPanel").gameObject;
        pickPanel = GameObject.Find("Canvas").transform.Find("PickPanel").gameObject;
        nextButton = pickPanel.transform.Find("NextButton").gameObject;
        levelUpButton = GameObject.Find("Canvas").transform.Find("LevelUpButton").gameObject;
        levelUpButton.SetActive(false);

        tutorialMessages = new string[] 
        {
            "ž�� ���� ���� ȯ���մϴ�.",
            "����� ȹ���� �ɷ��� '�׸��� �ɷ�'����,\nóġ�� ���� �׸��� �������� ������ �� �ֽ��ϴ�.", 
            "�� �ɷ��� Ȱ���Ͽ� ž�� Ŭ�����Ͻʽÿ�.",
            "�� ���� ���� ��⼮�Դϴ�. �÷��̾ �巡�� �Ͽ� �������� �̵���Ű����.",
            "������ ��Ÿ�����ϴ�. ������ �����ϰ� ���� ��ġ�� �÷��̾ ��ġ�ϼ���.",
            "���۹�ư�� ������ ������ ���۵˴ϴ�.",
            "",
            "��� ���ֵ��� MP�� �������� ��ų�� ����մϴ�.",
            "�������� �¸��ϸ� ��带 ȹ���մϴ�.",
            "���� �÷��̾� ��ų �� �ɷ�ġ ��ȭ, ��ų ȹ�濡 ����� �� �ֽ��ϴ�.",
            "���� ���� ���� �÷��̾��� HP�� ������Ű����.",//10
            "���� ��ư�� ���� ���� ������ �غ��ϼ���.",
            "�� �������� ������ ����Ʈ�� ����Ͽ� óġ�� �� ������ �׸��� �������� ��ȯ�� �� �ֽ��ϴ�.",
            "�׸��� ��� ���縦 ��ȯ�� �� ���忡 ��ġ�ϼ���.",
            "",
            "���� �нú꽺ų�� ���� �˷��帮�ڽ��ϴ�.",//15
            "�нú� ��ų�� ���� ���� ����ϴ� ��ų�̸� ���� ��ų, ��ȯ ��ų ���� �ֽ��ϴ�.",
            "�׸��� �� ��ȯ ��ų��ȹ���غ�����",
            "",
            "������ ����Ʈ�� ����Ͽ� �׸��� ���� ��ȯ�� �� ���忡 ��ġ�ϼ���.",
            //"",//20
            "���� ���� �±޿� ���� �˷��帮�ڽ��ϴ�.",
            "���� �׸��� ������ 3�� ��ȯ�ϸ� 2�� �׸��� �������� �±��մϴ�.",
            "�׸��� ��� ���縦 3�� ��ȯ�غ�����.",
            "",
            ""
        };
        

        goblinWarriorImage = Resources.Load<Sprite>("UI/goblinWarriorImage");
        goblinShamanImage = Resources.Load<Sprite>("UI/goblinShamanImage");
        goblinRogueImage = Resources.Load<Sprite>("UI/goblinRogueImage");
        shieldImage = Resources.Load<Sprite>("UI/shieldImage");
        explosionImage = Resources.Load<Sprite>("UI/explosionImage");
        stunImage = Resources.Load<Sprite>("UI/stunImage");
        hpImage = Resources.Load<Sprite>("UI/hpImage");
        mpImage = Resources.Load<Sprite>("UI/mpImage");
        adImage = Resources.Load<Sprite>("UI/adImage");
        asImage = Resources.Load<Sprite>("UI/asImage");
        shadowSphereImage = Resources.Load<Sprite>("UI/shadowSphereImage");

        ShadowGoblinWarriorItem = new ShopItem(GameCode.SHADOW_GOBLIN_WARRIOR, null,SpawnUnit, "���� ���",null, 0, 10, goblinWarriorImage);
        ShadowGoblinShamanItem = new ShopItem(GameCode.SHADOW_GOBLIN_SHAMAN,null, SpawnUnit, "������ ���",null, 0,15, goblinShamanImage);
        ShadowGoblinRogueItem = new ShopItem(GameCode.SHADOW_GOBLIN_ROGUE,null, SpawnUnit, "���� ���",null, 0,15, goblinRogueImage);

        AddShieldToSkillItem = new ShopItem(GameCode.ADD_SHIELD, PickItem,null,"[��ų ��ȭ]\n��ȣ��", "��ų ���� �� �ִ� ü���� 10%��ŭ ��ȣ���� �����մϴ�.", 50,0, shieldImage);
        AddExplosionToSkillItem = new ShopItem(GameCode.ADD_EXPLOSION, PickItem,null, "[��ų ��ȭ]\n����","��ų ���� �� ������ ������ 100%�� �߰� ���ظ� �����ϴ�.", 50,0, explosionImage);
        AddStunToSkillItem = new ShopItem(GameCode.ADD_STUN, PickItem,null, "[��ų ��ȭ]\n����","��ų ���� �� ���� 1�ʵ��� ������ŵ�ϴ�.", 50,0, stunImage);
        EnhanceHpItem = new ShopItem(GameCode.ENHANCE_HP, PickItem,null,"[�ɷ�ġ ��ȭ]\nü��", "�ִ� ü���� 500��ŭ ȹ���մϴ�.", 20,0, hpImage);
        EnhanceMpItem = new ShopItem(GameCode.ENHANCE_MP, PickItem,null, "[�ɷ�ġ ��ȭ]\n����", "�⺻ ������ 10��ŭ ȹ���մϴ�.", 20,0, mpImage);
        EnhanceAdItem = new ShopItem(GameCode.ENHANCE_AD, PickItem,null,"[�ɷ�ġ ��ȭ]\n���ݷ�", "�⺻ ������ �������� 100��ŭ �����մϴ�.", 20, 0,adImage);
        EnhanceAsItem = new ShopItem(GameCode.ENHANCE_AS, PickItem,null,"[�ɷ�ġ ��ȭ]\n���ݼӵ�", "�⺻ ���ݼӵ��� 10% �����մϴ� .", 20,0, asImage);

        AdBuffSkillItem = new ShopItem(GameCode.AD_BUFF_SKILL, PickItem, UsePassiveSkill,"[�нú� ��ų]\n���ݷ� ��ȭ", "��� �� �̹� �������� �Ʊ��� ���ݷ��� 10% �����ϴ� ��ų�� ȹ���մϴ�.", 30,20, adImage);
        AsBuffSkillItem = new ShopItem(GameCode.AS_BUFF_SKILL, PickItem,UsePassiveSkill, "[�нú� ��ų]\n���ݼӵ� ��ȭ","��� �� �̹� �������� �Ʊ��� ���ݼӵ��� 10% �����ϴ� ��ų�� ȹ���մϴ�..", 30,20, asImage);
        HpBuffSkillItem = new ShopItem(GameCode.HP_BUFF_SKILL,PickItem, UsePassiveSkill, "[�нú� ��ų]\nü�� ��ȭ","��� �� �̹� �������� �Ʊ��� ü���� 10% �����ϴ� ��ų�� ȹ���մϴ�.", 30,20, hpImage);
        MpBuffSkillItem = new ShopItem(GameCode.MP_BUFF_SKILL, PickItem,UsePassiveSkill, "[�нú� ��ų]\n���� ��ȭ","��� �� �̹� �������� �Ʊ��� ���� ������ 10 �����ϴ� ��ų�� ȹ���մϴ�.", 30,20, mpImage);
        ShadowSphereItem = new ShopItem(GameCode.SHADOW_SPHERE,PickItem, UsePassiveSkill, "[�нú� ��ų]\n�׸��� �� ��ȯ","3�� �ڿ� �����Ͽ� ������ 500�� �������� ������ �׸��� ���� ��ȯ�մϴ�.", 50,10, shadowSphereImage);
        stageCount = 1;
        gold = 0;
        maxShadowPoint = 0;
        currentShadowPoint = maxShadowPoint;
        maxUnitCount = 0;
        currentUnitCount = 0;
        TeamEnemyUnits = TeamEnemy.GetComponentsInChildren<AutoBattleUnit>();
        TeamPlayerUnits = TeamPlayer.GetComponentsInChildren<AutoBattleUnit>();
        TeamPlayerBenchUnits = TeamPlayerBench.GetComponentsInChildren<AutoBattleUnit>();
        Player = GameObject.Find("Player").GetComponent<AutoBattleUnit>();
        pickButton = Resources.Load<GameObject>("Prefabs/Button/PickButton");
        goblinWarrior = Resources.Load<GameObject>("Prefabs/Unit/EnemyGoblinWarrior");
        goblinShaman = Resources.Load<GameObject>("Prefabs/Unit/EnemyGoblinShaman");
        goblinRogue = Resources.Load<GameObject>("Prefabs/Unit/EnemyGoblinRogue");
        goblinWarchief = Resources.Load<GameObject>("Prefabs/Unit/EnemyGoblinWarchief");
        shadowGoblinWarrior = Resources.Load<GameObject>("Prefabs/Unit/ShadowGoblinWarrior"); 
        shadowGoblinShaman = Resources.Load<GameObject>("Prefabs/Unit/ShadowGoblinShaman");
        shadowGoblinRogue = Resources.Load<GameObject>("Prefabs/Unit/ShadowGoblinRogue");
        shadowSphere = Resources.Load<GameObject>("Prefabs/Unit/ShadowSphere");
        maxUnitCount = 1;
        shopButtons = new GameObject[4];
        shopButtonsPos = new Vector2[4];
        shopButtonsPos[0] = new Vector2(-525f, 0f);
        shopButtonsPos[1] = new Vector2(-175f, 0f);
        shopButtonsPos[2] = new Vector2(175f, 0f);
        shopButtonsPos[3] = new Vector2(525f, 0f);

        skillPanel = GameObject.Find("Canvas").transform.Find("SkillPanel").gameObject;
        skillButton = Resources.Load<GameObject>("Prefabs/Button/SkillButton");
        skillButtons = new GameObject[8];//index0~3:spawn skill //index4~7:combat skill
        skillButtonsPos = new Vector2[8];
        for (int i=0;i<4;i++)
        {
            skillButtonsPos[i] = new Vector2(-550f + 150*i, -430f);
        }
        for (int i = 4; i < 8; i++)
        {
            skillButtonsPos[i] = new Vector2(100f + 150 * (i-4), -430f);
        }
        for (int i = 0; i < 8; i++)
        {
            MakeSkillButton(i);
        }
        buffPanel = GameObject.Find("Canvas").transform.Find("BuffPanel").gameObject;
        buffIcon = Resources.Load<GameObject>("Prefabs/Button/BuffIcon");
        buffIcons = new GameObject[4];
        buffIconsPos = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            buffIconsPos[i] = new Vector2(-870f + 100 * i, 220f);
        }

        for (int i = 0; i < 8; i++)
        {
            MakeSkillButton(i);
        }
        RemainingActiveSkills.Add(GameCode.ADD_SHIELD);
        RemainingActiveSkills.Add(GameCode.ADD_EXPLOSION);
        RemainingActiveSkills.Add(GameCode.ADD_STUN);

        RemainingPassiveSkills.Add(GameCode.AD_BUFF_SKILL);
        RemainingPassiveSkills.Add(GameCode.AS_BUFF_SKILL);
        RemainingPassiveSkills.Add(GameCode.HP_BUFF_SKILL);
        RemainingPassiveSkills.Add(GameCode.MP_BUFF_SKILL);
        RemainingPassiveSkills.Add(GameCode.SHADOW_SPHERE);

        EnhanceablStats.Add(GameCode.ENHANCE_HP);
        EnhanceablStats.Add(GameCode.ENHANCE_MP);
        EnhanceablStats.Add(GameCode.ENHANCE_AD);
        EnhanceablStats.Add(GameCode.ENHANCE_AS);

        SetTutorial();
    }

    public void SetTutorial()
    {
        
        if (currentMessageIndex ==0)
        {
            isPause = true;
            isTutorial = true;
            systemWindow.SetActive(true);
            systemText = systemWindow.transform.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            systemText.text = tutorialMessages[currentMessageIndex++];
            startButton.SetActive(false);    
        }
        else if (currentMessageIndex <= 2)
        {
            systemText.text = tutorialMessages[currentMessageIndex++];
        }
        else if (currentMessageIndex == 3)
        {

            isPause = false;
            systemWindow.SetActive(false);
            tutorialText = tutorialWindow.transform.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.SetActive(true);  
        }
        else if (currentMessageIndex == 4)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350f, 150f);
        }
        else if (currentMessageIndex == 5)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(720f, -250f);
            startButton.SetActive(true);
        }
        else if (currentMessageIndex == 6)
        {
            currentMessageIndex++;
            tutorialWindow.SetActive(false);
            startButton.SetActive(false);
        }
        else if (currentMessageIndex == 7)
        {
            isPause = true;
            Time.timeScale = 0;
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = Player.status.GetComponent<RectTransform>().anchoredPosition;
            tutorialWindow.GetComponent<RectTransform>().Translate(new Vector2(-350f,0f));
            tutorialWindow.SetActive(true);
            StartCoroutine("Test");
        }
        else if (currentMessageIndex == 8)
        {
            tutorialWindow.SetActive(true);
            tutorialWindowButton.SetActive(true);
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-660f, -400f);
            
            
        }
        else if (currentMessageIndex == 9)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
        }
        else if (currentMessageIndex == 10)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            shopButtons[0].GetComponent<Button>().interactable = true;
            tutorialWindowButton.SetActive(false);
        }
        else if (currentMessageIndex == 11 )
        {
           
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            nextButton.SetActive(true);
            
        }
        else if (currentMessageIndex == 12)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-660f, -100f);
            tutorialWindowButton.SetActive(true);
            skillButtons[0].GetComponent<Button>().interactable = false;
        }
        else if (currentMessageIndex == 13)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindowButton.SetActive(false);
            skillButtons[0].GetComponent<Button>().interactable = true;
        }


        else if (currentMessageIndex == 14)
        {
            currentMessageIndex++;
            tutorialWindow.SetActive(false);
            startButton.SetActive(true);
        }
        else if (currentMessageIndex == 15)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            startButton.SetActive(false);
            tutorialWindowButton.SetActive(true);
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-660f, -400f);
            tutorialWindow.SetActive(true);

            nextButton.SetActive(false);
        }
        else if (currentMessageIndex == 16)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            
        }
        else if (currentMessageIndex == 17)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindowButton.SetActive(false);
            shopButtons[3].GetComponent<Button>().interactable = true;
        }
        else if (currentMessageIndex == 18)
        {
            currentMessageIndex++;
            tutorialWindow.SetActive(false);
            nextButton.SetActive(true);
            skillButtons[0].GetComponent<Button>().interactable = false;
        }
        else if (currentMessageIndex == 19)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindow.SetActive(true);
            tutorialWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-660f, 0f);
        }
        else if (currentMessageIndex == 20)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindowButton.SetActive(true);
        }
        else if (currentMessageIndex == 21)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
        }
        else if (currentMessageIndex == 22)
        {
            tutorialText.text = tutorialMessages[currentMessageIndex++];
            tutorialWindowButton.SetActive(false);
            skillButtons[0].GetComponent<Button>().interactable = true;
        }
        else if (currentMessageIndex == 23)
        {
            currentMessageIndex++;
            tutorialWindow.SetActive(false);
            startButton.SetActive(true);
            isTutorial=false;
        }
    }
    public IEnumerator Test()
    {
        yield return new WaitForSecondsRealtime(2f);
        isPause = false;
        Time.timeScale = 1;
        tutorialWindow.SetActive(false);
        yield break;
    }

    public void TutorialUpdate()
    {
        if (currentMessageIndex==4 && currentUnitCount==1)
        {
            SetTutorial();
            SetStage();
        }
        else if (currentMessageIndex == 5)
        {
            if (currentUnitCount == 0)
            {
                flag = 1;
            }
            else
            {
                if (flag == 1)
                {
                    SetTutorial();
                }
            }
        }
        else if (currentMessageIndex == 6 && GameManager.instance.GetIsCombating())
        {
            SetTutorial();         
        }
        else if (currentMessageIndex == 7 && Player.isReadySkill)
        {
            SetTutorial();
        }
        else if (currentMessageIndex == 12 && victoryPanel.activeSelf==false)
        {
            SetTutorial();
        }
        else if (currentMessageIndex == 14 && currentUnitCount == 2)
        {
            SetTutorial();
        }
        else if (currentMessageIndex == 20 && TeamPlayer.GetComponentsInChildren<Drag>().Length==2)
        {
            
            SetTutorial();
        }
    }

    public void PickItem(Transform tr, ShopItem item)
    {
        if(currentMessageIndex==11 || currentMessageIndex == 18)
        {
            SetTutorial();
        }
        if (gold < item.goldCost)
        {
            Debug.Log("����Ʈ�� �����մϴ�");
            return;
        }  
        tr.Find("SoldOut").gameObject.SetActive(true);
        tr.GetComponent<Button>().onClick.RemoveAllListeners();
        gold -= item.goldCost;
        switch(item.code)
        {
            case GameCode.ADD_SHIELD:
                Player.GetComponent<Player>().shield.Enhance();
                RemainingActiveSkills.Remove(GameCode.ADD_SHIELD);
                Debug.Log("���� ȹ��");
                break;
            case GameCode.ADD_EXPLOSION:
                Player.GetComponent<Player>().explosion.Enhance();
                RemainingActiveSkills.Remove(GameCode.ADD_EXPLOSION);
                Debug.Log("���� ȹ��");
                break;
            case GameCode.ADD_STUN:
                Player.GetComponent<Player>().stun.Enhance();
                RemainingActiveSkills.Remove(GameCode.ADD_STUN);
                Debug.Log("���� ȹ��");
                break;
            case GameCode.ENHANCE_HP:
                Player.stat.maxHP += 500;
                Player.stat.currentHP = Player.stat.maxHP;
                Player.updateStatus();
                Debug.Log("HP ȹ��");
                break;
            case GameCode.ENHANCE_MP:
                Player.stat.currentMP +=10;
                Player.updateStatus();
                Debug.Log("MP ����");
                break;
            case GameCode.ENHANCE_AD:
                Player.stat.attackDamage += 100;
                Debug.Log("AD ȹ��");
                break;
            case GameCode.ENHANCE_AS:
                Player.stat.attackSpeed += 0.1f;
                Debug.Log("AS ȹ��");
                break;
            case GameCode.AD_BUFF_SKILL:
                if(passiveSkillIndex<8) SetSkillButton(passiveSkillIndex++, AdBuffSkillItem);
                Debug.Log("AD ���� ��ų ȹ��");
                break;
            case GameCode.AS_BUFF_SKILL:
                if (passiveSkillIndex < 8) SetSkillButton(passiveSkillIndex++, AsBuffSkillItem);
                Debug.Log("AS ���� ��ų ȹ��");
                break;
            case GameCode.HP_BUFF_SKILL:
                if (passiveSkillIndex < 8) SetSkillButton(passiveSkillIndex++, HpBuffSkillItem);
                Debug.Log("HP ���� ��ų ȹ��");
                break;
            case GameCode.MP_BUFF_SKILL:
                if (passiveSkillIndex < 8) SetSkillButton(passiveSkillIndex++, MpBuffSkillItem);
                Debug.Log("MP ���� ��ų ȹ��");
                break;
            case GameCode.SHADOW_SPHERE:
                if (passiveSkillIndex < 8) SetSkillButton(passiveSkillIndex++, ShadowSphereItem);
                Debug.Log("�׸��� �� ��ȯ ��ų ȹ��");
                break;

            default:
                break;
        }
    }
    public void SpawnUnit(Transform tr, ShopItem item)
    {
        if (currentShadowPoint < item.manaCost)
        {
            Debug.Log("������ �����մϴ�");
            return;
        }
        currentShadowPoint -= item.manaCost;
        switch (item.code)
        {
            case GameCode.SHADOW_GOBLIN_WARRIOR:
            case GameCode.SHADOW_GOBLIN_SHAMAN:
            case GameCode.SHADOW_GOBLIN_ROGUE:
                AddShadowUnit(item.code);
                Debug.Log(item.code + "ȹ��");
                break;
            default:
                break;
        }
    }
    public void UsePassiveSkill (Transform tr, ShopItem item)
    {
        if (currentShadowPoint < item.manaCost)
        {
            Debug.Log("������ �����մϴ�");
            return;
        }
        currentShadowPoint -= item.manaCost;
        tr.GetComponent<Button>().interactable = false;
        tr.Find("SoldOut").gameObject.SetActive(true);
        switch (item.code)
        {
            case GameCode.AD_BUFF_SKILL:
                UsedPassiveSkills.Add(GameCode.AD_BUFF_SKILL);
                SetBuffIcon(buffIndex++, AdBuffSkillItem);
                Debug.Log("�Ʊ� AD ����");
                break;
            case GameCode.AS_BUFF_SKILL:
                UsedPassiveSkills.Add(GameCode.AS_BUFF_SKILL);
                SetBuffIcon(buffIndex++, AsBuffSkillItem);
                Debug.Log("�Ʊ� AS ����");
                break;
            case GameCode.HP_BUFF_SKILL:
                UsedPassiveSkills.Add(GameCode.HP_BUFF_SKILL);
                SetBuffIcon(buffIndex++, HpBuffSkillItem);
                Debug.Log("�Ʊ� HP ����");
                break;
            case GameCode.MP_BUFF_SKILL:
                UsedPassiveSkills.Add(GameCode.MP_BUFF_SKILL);
                SetBuffIcon(buffIndex++, MpBuffSkillItem);
                Debug.Log("�Ʊ� ���� MP ����");
                break;
            case GameCode.SHADOW_SPHERE:
                AddShadowUnit(GameCode.SHADOW_SPHERE);
                Debug.Log("�Ʊ� ���� MP ����");
                break;
            default:
                break;
        }
    }

    public void SetStage()
    {
        if(currentMessageIndex==19)
        {
            SetTutorial();
        }
        isPause = false;

        if (stageCount == 1)
        {
            maxShadowPoint = 0;
        }
        else if (stageCount==2)
        {
            maxShadowPoint = 10;
        }
        else if (stageCount == 3)
        {
            maxShadowPoint = 40;
        }
        else
        {
            maxShadowPoint =  stageCount * 20;
        }
        //maxShadowPoint += 100;
        currentShadowPoint = maxShadowPoint;
        maxUnitCount = 1 + stageCount / 2;
        if (shop != null) Destroy(shop);
        if (victoryPanel.activeSelf) victoryPanel.SetActive(false);
        if (pickPanel.activeSelf) pickPanel.SetActive(false);

        if (buffIcons != null)
        {
            for(int i=0;i<4;i++)
            {
                Destroy(buffIcons[i]);
            }
            buffIndex = 0;
        }        

        for (int i=4; i<8; i++)
        {
            skillButtons[i].GetComponent<Button>().interactable = true;
            skillButtons[i].transform.Find("SoldOut").gameObject.SetActive(false);
        }
        
        if (stageCount == 1) SetStage1();
        else if (stageCount == 2) SetStage2();
        else if (stageCount == 3) SetStage3();
        else if (stageCount == 4) SetStage4();
        else if (stageCount == 5) SetStage5();
        else if (stageCount == 6) SetStage6();
        else if (stageCount == 7) SetStage7();
        else if (stageCount == 8) SetStage8();
        else if (stageCount == 9) SetStage9();
        else
        {
            Debug.Log("GameClear");
        }

    }
    public void MakeSkillButton(int num)
    {
        skillButtons[num] = (GameObject)Instantiate(skillButton);
        skillButtons[num].transform.SetParent(skillPanel.transform);
        skillButtons[num].transform.GetComponent<RectTransform>().anchoredPosition = skillButtonsPos[num];
        skillButtons[num].transform.Find("Image").gameObject.SetActive(false);
        skillButtons[num].transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = null;
    }
    public void SetBuffIcon(int num,ShopItem item)
    {
        buffIcons[num] = (GameObject)Instantiate(buffIcon);
        buffIcons[num].transform.SetParent(buffPanel.transform);
        buffIcons[num].transform.GetComponent<RectTransform>().anchoredPosition = buffIconsPos[num];
        buffIcons[num].transform.Find("Image").GetComponent<Image>().sprite = item.image;
    }
    public void SetSkillButton(int num, ShopItem item)
    {
        if (item == null)
        {
            Debug.Log("SetSkillButton Error");
            return;
        }
        skillButtons[num].GetComponent<Button>().onClick.AddListener(() => item.UseSkillAction(skillButtons[num].transform, item)); //PickGoblinWarrior(shopButtons[num].transform));
        skillButtons[num].transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = item.manaCost.ToString() + "P";
        skillButtons[num].transform.Find("Image").gameObject.SetActive(true);
        skillButtons[num].transform.Find("Image").GetComponent<Image>().sprite = item.image;

    }
    public void SetShopButton(int num, ShopItem item)
    {
        shopButtons[num] = (GameObject)Instantiate(pickButton);
        shopButtons[num].transform.SetParent(shop.transform);
        shopButtons[num].transform.GetComponent<RectTransform>().anchoredPosition = shopButtonsPos[num];
        shopButtons[num].GetComponent<Button>().onClick.AddListener(() => item.PickShopAction(shopButtons[num].transform,item)); //PickGoblinWarrior(shopButtons[num].transform));
        shopButtons[num].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.name;
        shopButtons[num].transform.Find("Info").GetComponent<TextMeshProUGUI>().text = item.info;
        shopButtons[num].transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = item.goldCost.ToString() + "G";
        shopButtons[num].transform.Find("Image").GetComponent<Image>().sprite = item.image;
    }
   
    public void SetAddActiveSkillShop(int posIndex)
    {
        System.Random rnd = new System.Random();
        // Get a random index from the list.
        int index = rnd.Next(RemainingActiveSkills.Count);
        // Get the element at the random index.
        GameCode code = RemainingActiveSkills[index];
        switch (code)
        {
            case GameCode.ADD_SHIELD:
                SetShopButton(posIndex, AddShieldToSkillItem);
                break;
            case GameCode.ADD_EXPLOSION:
                SetShopButton(posIndex, AddExplosionToSkillItem);
                break;
            case GameCode.ADD_STUN:
                SetShopButton(posIndex, AddStunToSkillItem);
                break;
            default:
                break;
        }
    }
    public void SetAddPassiveSkillShop(int posIndex)
    {
        System.Random rnd = new System.Random();

        // Get a random index from the list.
        int index = rnd.Next(RemainingPassiveSkills.Count);
        // Get the element at the random index.
        GameCode code = RemainingPassiveSkills[index];
        switch (code)
        {
            case GameCode.AD_BUFF_SKILL:
                SetShopButton(posIndex++, AdBuffSkillItem);
                break;
            case GameCode.AS_BUFF_SKILL:
                SetShopButton(posIndex++, AsBuffSkillItem);
                break;
            case GameCode.HP_BUFF_SKILL:
                SetShopButton(posIndex++, HpBuffSkillItem);
                break;
            case GameCode.MP_BUFF_SKILL:
                SetShopButton(posIndex++, MpBuffSkillItem);
                break;
            case GameCode.SHADOW_SPHERE:
                SetShopButton(posIndex++, AdBuffSkillItem);
                break;
            default:
                break;
        }
    }
    public void SetEnhanceStatShop(int index1, int index2)
    {
        List<GameCode> copyEnhanceableStats = EnhanceablStats.ToList();
        System.Random rnd = new System.Random();

        // Get the first random element.
        int randomIndex1 = rnd.Next(copyEnhanceableStats.Count);
        GameCode stat1 = copyEnhanceableStats[randomIndex1];
        copyEnhanceableStats.RemoveAt(randomIndex1);

        //// Get the second random element.
        int randomIndex2 = rnd.Next(copyEnhanceableStats.Count);
        GameCode stat2 = copyEnhanceableStats[randomIndex2];

        //Get the element at the random index.
        switch (stat1)
        {
            case GameCode.ENHANCE_AD:
                SetShopButton(index1, EnhanceAdItem);
                break;
            case GameCode.ENHANCE_AS:
                SetShopButton(index1, EnhanceAsItem);
                break;
            case GameCode.ENHANCE_HP:
                SetShopButton(index1, EnhanceHpItem);
                break;
            case GameCode.ENHANCE_MP:
                SetShopButton(index1, EnhanceMpItem);
                break;
            default:
                break;
        }
        switch (stat2)
        {
            case GameCode.ENHANCE_AD:
                SetShopButton(index2, EnhanceAdItem);
                break;
            case GameCode.ENHANCE_AS:
                SetShopButton(index2, EnhanceAsItem);
                break;
            case GameCode.ENHANCE_HP:
                SetShopButton(index2, EnhanceHpItem);
                break;
            case GameCode.ENHANCE_MP:
                SetShopButton(index2, EnhanceMpItem);
                break;
            default:
                break;
        }
    }
    void Update()
    {
        if (isTutorial) TutorialUpdate();
        if (isPause) return;
        //�������� �ƴ� ��쿡�� ���
        if (!GameManager.instance.GetIsCombating())
        {
            int count = 0;
            TeamPlayerUnits = TeamPlayer.GetComponentsInChildren<AutoBattleUnit>();
            foreach (AutoBattleUnit unit in TeamPlayerUnits)
            {
                if (unit.gameObject.CompareTag("TeamPlayer")) count++;
            }
            currentUnitCount = count;
            return;
        }
        //�������� �����Ҷ��� ���        
        if (TeamEnemyUnits.Length ==0) return;
        if (Player.isDie == true)
        {
            isPause = true;
            Invoke("DefeatStage", 3f);
        }

        foreach (AutoBattleUnit unit in TeamEnemyUnits)
        {
            if (unit.gameObject.activeSelf) return; 
        }
        isPause = true;
        Invoke("VictoryStage",1.5f);
    }
    public void StartStage()
    {
        GameManager.instance.SetIsCombating(true);
        TeamEnemyUnits = TeamEnemy.GetComponentsInChildren<AutoBattleUnit>();
        TeamPlayerUnits = TeamPlayer.GetComponentsInChildren<AutoBattleUnit>();
        TeamPlayerBenchUnits = TeamPlayerBench.GetComponentsInChildren<AutoBattleUnit>();
        foreach (AutoBattleUnit unit in TeamEnemyUnits)
        {
            unit.SaveStatOrigin();
            unit.StartCombat();
            
        }
        foreach (AutoBattleUnit unit in TeamPlayerUnits)
        {
            unit.GetComponent<Drag>().enabled = false;
            unit.SaveStatOrigin();
            unit.StartCombat();
            if(UsedPassiveSkills==null)
            {
                continue;
            }
            foreach (GameCode code in UsedPassiveSkills)
            {
                switch(code)
                {
                    case GameCode.AD_BUFF_SKILL:
                        unit.stat.attackDamage *= 1.1f;
                        break;
                    case GameCode.AS_BUFF_SKILL:
                        unit.stat.attackSpeed *= 1.1f;
                        break;
                    case GameCode.HP_BUFF_SKILL:
                        unit.stat.maxHP *= 1.1f;
                        Debug.Log(unit.stat.maxHP);
                        unit.stat.currentHP = unit.stat.maxHP;
                        unit.updateStatus();
                        break;
                    case GameCode.MP_BUFF_SKILL:
                        unit.stat.currentMP += 10f;
                        break;
                    default:
                        break;
                }
                
            }             
        }
        UsedPassiveSkills.Clear();
        currentShadowPoint = 0;
        
    }
    public void VictoryStage()
    {
        GameManager.instance.SetIsCombating(false);
        foreach (AutoBattleUnit unit in TeamEnemyUnits)
        {
            Destroy(unit.status);
            Destroy(unit.gameObject);
            Debug.Log("Delete " + unit.gameObject.name);
        }
        foreach (AutoBattleUnit unit in TeamPlayerUnits)
        {
            if (unit == Player)
            {
                unit.GetComponent<Drag>().enabled = true;
                unit.EndCombat();
                unit.InitUnit();
            }
            else
            {
                Destroy(unit.gameObject);
                Destroy(unit.status);
            }
        }
        foreach (AutoBattleUnit unit in TeamPlayerBenchUnits)
        {
            if (unit == Player)
            {
                unit.GetComponent<Drag>().enabled = true;
                unit.EndCombat();
                unit.InitUnit();
            }
            else
            {
                Destroy(unit.gameObject);
                Destroy(unit.status);
            }
        }
        stageCount++;
        if (stageCount == 10)
        {
            clearPanel.SetActive(true);
        }
        else
        {
            victoryPanel.SetActive(true);
            pickPanel.SetActive(true);
        }
        shop = new GameObject("shop");
        shop.transform.SetParent(pickPanel.transform);
        shop.transform.localPosition = new Vector3(0f, 0f, 0f);
        if (currentMessageIndex == 8 || currentMessageIndex == 15)
        {
            SetTutorial();
        }
        if (stageCount == 2)
        {
            shopButtons = new GameObject[4];
            SetShopButton(0, EnhanceHpItem);
            SetShopButton(1, EnhanceAsItem);
            SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
            SetAddPassiveSkillShop(3);
            foreach (GameObject button in shopButtons)
            {
                button.GetComponent<Button>().interactable = false;
            }

        }
        else if (stageCount == 3)
        {
            shopButtons = new GameObject[4];
            SetEnhanceStatShop(0,1);//�ε���3���� ���� ��ȭ
            SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
            SetShopButton(3, ShadowSphereItem);
            foreach (GameObject button in shopButtons)
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
        else if (stageCount >= 4)
        {
            shopButtons = new GameObject[4];
            SetEnhanceStatShop(0, 1);//�ε���3���� ���� ��ȭ
            SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
            SetAddPassiveSkillShop(3);
        }
        gold += 30 + (stageCount / 2) * 10;
        //else if (stageCount == 5)
        //{
        //    shopButtons = new GameObject[4];
        //    SetEnhanceStatShop(0, 1);//�ε���3���� ���� ��ȭ
        //    SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
        //    SetAddPassiveSkillShop(3);
        //    gold += 40;
        //}
        //else if (stageCount == 6)
        //{
        //    shopButtons = new GameObject[4];
        //    SetEnhanceStatShop(0, 1);//�ε���3���� ���� ��ȭ
        //    SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
        //    SetAddPassiveSkillShop(3);
        //    gold += 40;
        //}
        //else if (stageCount == 7)
        //{
        //    SetEnhanceStatShop(0, 1);//�ε���3���� ���� ��ȭ
        //    SetAddActiveSkillShop(2);//�ε���2���� ��ų ��ȭ
        //    SetAddPassiveSkillShop(3);
        //    gold += 40;
        //}
    }
    public void DefeatStage()
    {
        GameManager.instance.SetIsCombating(false);
        if (!defeatPanel.activeSelf) defeatPanel.SetActive(true);
        foreach (AutoBattleUnit unit in TeamEnemyUnits)
        {
            Destroy(unit.status);
            Destroy(unit.gameObject);
            Debug.Log("Delete " + unit.gameObject.name);
        }
        foreach (AutoBattleUnit unit in TeamPlayerUnits)
        {
            if (unit == Player)
            {
                unit.GetComponent<Drag>().enabled = true;
                unit.EndCombat();
                unit.InitUnit();
            }
            else
            {
                Destroy(unit.gameObject);
                Destroy(unit.status);
            }
        }
        foreach (AutoBattleUnit unit in TeamPlayerBenchUnits)
        {
            if (unit == Player)
            {
                unit.GetComponent<Drag>().enabled = true;
                unit.EndCombat();
                unit.InitUnit();
            }
            else
            {
                Destroy(unit.gameObject);
                Destroy(unit.status);
            }
        }
        Debug.Log("DefeatStage");
    }
    public void RetryStage()
    {
        if(!startButton.activeSelf) startButton.SetActive(true);
        defeatPanel.SetActive(false);
        SetStage();
        Debug.Log("RetryStage");
    }
    public void ClearStage()
    {
        clearPanel.SetActive(true);
    }
    public void SpawnGoblinWarrior(int randX)
    {
        int rand = UnityEngine.Random.Range(0, 3);
        GameObject unit = null;
        if (rand == 0)
        {
            unit = Instantiate(goblinWarrior, TeamEnemy.transform);
        }
        else if (rand == 1)
        {
            unit = Instantiate(goblinShaman, TeamEnemy.transform);
        }
        else if (rand == 2)
        {
            unit = Instantiate(goblinRogue, TeamEnemy.transform);
        }

        unit.transform.position = new Vector3(randX, 0f, 8f);
        unit.GetComponent<AutoBattleUnit>().isSpawn = true;
        TeamEnemyUnits = TeamEnemy.GetComponentsInChildren<AutoBattleUnit>();
    }
    public void SetStage1()
    {
        
        GameObject[] unit = new GameObject[2];
        unit[0] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(4f, 0f, 6f);     
        unit[1] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(6f, 0f, 6f);
    }

    public void SetStage2()
    {
        SetSkillButton(0, ShadowGoblinWarriorItem);
        GameObject[] unit = new GameObject[3];
        unit[0] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(8f, 0f, 6f);
        unit[1] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(2f, 0f, 6f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(6f, 0f, 6f);
    }

    public void SetStage3()
    {
        GameObject[] unit = new GameObject[4];
        unit[0] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(8f, 0f, 8f);
        unit[1] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(2f, 0f, 8f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(4f, 0f, 6f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(6f, 0f, 6f);
    }
    public void SetStage4()
    {
        SetSkillButton(1, ShadowGoblinShamanItem);
        GameObject[] unit = new GameObject[4];
        unit[0] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(2f, 0f, 8f);
        unit[1] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(8f, 0f, 8f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(4f, 0f, 5f);
        unit[3] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[3].transform.position = new Vector3(6f, 0f, 5f);
    }
    public void SetStage5()
    {
        SetSkillButton(2, ShadowGoblinRogueItem);
        GameObject[] unit = new GameObject[4];
        unit[0] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(0f, 0f, 8f);
        unit[1] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(9f, 0f, 8f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(3f, 0f, 5f);
        unit[2].GetComponent<AutoBattleUnit>().LevelUp();
        unit[3] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[3].transform.position = new Vector3(5f, 0f, 5f);
        unit[3].GetComponent<AutoBattleUnit>().LevelUp();
    }
    public void SetStage6()
    {
        GameObject[] unit = new GameObject[4];
        unit[0] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(1f, 0f, 8f);
        unit[0].GetComponent<AutoBattleUnit>().LevelUp();
        unit[1] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(2f, 0f, 8f);
        unit[1].GetComponent<AutoBattleUnit>().LevelUp();
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(6f, 0f, 5f);
        unit[3] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[3].transform.position = new Vector3(8f, 0f, 5f);
    }
    public void SetStage7()
    {
        GameObject[] unit = new GameObject[5];
        unit[0] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(0f, 0f, 8f);
        unit[1] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(9f, 0f, 8f);
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(5f, 0f, 5f);
        unit[2].GetComponent<AutoBattleUnit>().LevelUp();
        unit[3] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[3].transform.position = new Vector3(7f, 0f, 5f);
        unit[3].GetComponent<AutoBattleUnit>().LevelUp();
        unit[4] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[4].transform.position = new Vector3(5f, 0f, 8f);
        unit[4].GetComponent<AutoBattleUnit>().LevelUp();
    }
    public void SetStage8()
    {
        GameObject[] unit = new GameObject[6];
        unit[0] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[0].transform.position = new Vector3(0f, 0f, 8f);
        unit[0].GetComponent<AutoBattleUnit>().LevelUp();
        unit[1] = Instantiate(goblinShaman, TeamEnemy.transform);
        unit[1].transform.position = new Vector3(9f, 0f, 8f);
        unit[1].GetComponent<AutoBattleUnit>().LevelUp();
        unit[2] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[2].transform.position = new Vector3(7f, 0f, 5f);
        unit[2].GetComponent<AutoBattleUnit>().LevelUp();
        unit[3] = Instantiate(goblinWarrior, TeamEnemy.transform);
        unit[3].transform.position = new Vector3(5f, 0f, 5f);
        unit[3].GetComponent<AutoBattleUnit>().LevelUp();
        unit[4] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[4].transform.position = new Vector3(5f, 0f, 8f);
        unit[4] = Instantiate(goblinRogue, TeamEnemy.transform);
        unit[4].transform.position = new Vector3(6f, 0f, 8f);
    }
    public void SetStage9()
    {
        GameObject unit = Instantiate(goblinWarchief, TeamEnemy.transform);
        unit.transform.position = new Vector3(4f, 0f, 6f);
    }
    public void spawnShadowSphere()
    {
        SetSkillButton(4, ShadowSphereItem);
        SetSkillButton(5, AsBuffSkillItem);
        SetSkillButton(6, HpBuffSkillItem);
        SetSkillButton(7, MpBuffSkillItem);
    }
    public void AddShadowUnit(GameCode unit_name)
    {
        TeamPlayerUnits = TeamPlayer.GetComponentsInChildren<AutoBattleUnit>();
        TeamPlayerBenchUnits = TeamPlayerBench.GetComponentsInChildren<AutoBattleUnit>();
        AutoBattleUnit[] AlreadyUnits = new AutoBattleUnit[2];
        int count = 0;
        bool isLevel2 = false;
        foreach (AutoBattleUnit unit in TeamPlayerUnits)
        {
            if((GameCode)unit.gameCode == unit_name && unit.stat.level==1)
            {
                AlreadyUnits[count] = unit;
                count++;
            }
        }
        foreach (AutoBattleUnit unit in TeamPlayerBenchUnits)
        {
            if ((GameCode)unit.gameCode == unit_name && unit.stat.level == 1)
            {
                AlreadyUnits[count] = unit;
                count++;
            }
        }
        if (count == 2)
        {
            isLevel2 = true;
            foreach (AutoBattleUnit unit in AlreadyUnits)
            {
                Destroy(unit.status);
                Destroy(unit.gameObject);
            }
        }

        if (unit_name == GameCode.SHADOW_GOBLIN_WARRIOR)
        {
            instanceShadowUnit = Instantiate(shadowGoblinWarrior) as GameObject;
        }
        else if (unit_name == GameCode.SHADOW_GOBLIN_SHAMAN)
        {
            instanceShadowUnit = Instantiate(shadowGoblinShaman) as GameObject;
        }
        else if (unit_name == GameCode.SHADOW_GOBLIN_ROGUE)
        {
            instanceShadowUnit = Instantiate(shadowGoblinRogue) as GameObject;
        }
        else if (unit_name == GameCode.SHADOW_SPHERE)
        {
            instanceShadowUnit = Instantiate(shadowSphere) as GameObject;
            instanceShadowUnit.GetComponent<Drag>().isUnit = false;
        }
        else
        {
            Debug.Log("Fail to add unit");
            return;
        }
        int layer = 1 << LayerMask.NameToLayer("TeamPlayer");
        Vector3 tempPos = new Vector3(0.0f, 5.0f, -2.0f); 
        for (int i=0;i<10;i++)
        {
            if (Physics.Raycast(tempPos + new Vector3(i,0.0f,0.0f), new Vector3(0.0f,-1.0f,0.0f) ,out hit, Mathf.Infinity, layer) )
            {
                continue;
            }
            else
            {
                
                tempPos = tempPos + new Vector3(i, -5.0f, 0.0f);
                break;
            }
        }
        if (isLevel2)
        {
            instanceShadowUnit.GetComponent<AutoBattleUnit>().LevelUp();
        }
        instanceShadowUnit.transform.position = tempPos;
        instanceShadowUnit.transform.localEulerAngles = new Vector3(0, 0, 0);
        instanceShadowUnit.transform.SetParent(TeamPlayerBench.transform);
        instanceShadowUnit = null;
        if (currentMessageIndex == 23 && isLevel2)
        {
            SetTutorial();
        }
        Debug.Log("count:"+count);
        Debug.Log("isLevel2" + isLevel2);
    }
    public int[] levelUpCost = new int[] {5,10,20,40,60,80,100};
    public int playerLevel = 1;
    public void playerLevelUp()
    {
        if (gold < levelUpCost[playerLevel-1])
        {
            Debug.Log("����Ʈ�� �����մϴ�");
            return;
        }
        gold -= levelUpCost[playerLevel - 1];
        playerLevel++;
    }
}
