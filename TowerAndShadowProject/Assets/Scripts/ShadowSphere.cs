using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSphere : MonoBehaviour
{
    private GameObject shadowBomb;
    private float time=0f;
    private float bombTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        shadowBomb = Resources.Load<GameObject>("Prefabs/Skill&Attack/ShadowBombSkill");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GetIsCombating() )
        {
            if(this.gameObject.GetComponent<Drag>().isInField)//전투중일때
            {
                time += Time.deltaTime;
                if (time > bombTime)
                {
                    GameObject instanceShadowBomb = Instantiate(shadowBomb) as GameObject;
                    instanceShadowBomb.transform.position = this.transform.position + new Vector3(0f, 0.5f, 0f);
                    instanceShadowBomb.GetComponent<ShadowBombSkill>().damage = 500f;

                    Destroy(this.gameObject);
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
            
            
        }
        else//전투중이아닐때
        {
            if (time > 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                return;
            }
        }

    }
}
