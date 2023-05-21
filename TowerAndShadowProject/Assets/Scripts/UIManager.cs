using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
//using System;
public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI stageCountText;
    public TextMeshProUGUI shadowPointText;
    public Image shadowPointGauge;
    public TextMeshProUGUI goldText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (unitCountText != null)
        { 
            unitCountText.text = StageManager.instance.currentUnitCount.ToString() + "/" + StageManager.instance.maxUnitCount.ToString(); 
        }
        if (stageCountText != null)
        {
            stageCountText.text = "STAGE " + StageManager.instance.stageCount.ToString();
        }
        if (shadowPointText != null)
        {
            shadowPointText.text = StageManager.instance.currentShadowPoint.ToString()+ "/"+StageManager.instance.maxShadowPoint.ToString();
            if (StageManager.instance.maxShadowPoint == 0)
            {
                shadowPointGauge.fillAmount = 0f;
            }
            else
            {
                shadowPointGauge.fillAmount = (float)StageManager.instance.currentShadowPoint / (float)StageManager.instance.maxShadowPoint;
            }
        }

        if (goldText != null)
        {
            goldText.text = StageManager.instance.gold.ToString();
        }
    }
    public void LoadStage1()
    {
        SceneManager.LoadScene("stage1"); 
    }
    public void LoadTitle()
    {
        SceneManager.LoadScene("title"); 
    }
}
