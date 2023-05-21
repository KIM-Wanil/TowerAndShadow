using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBombSkill : MonoBehaviour
{

    private string targetName;
    public float damage = 500f;
    void Start()
    {
        targetName = "TeamEnemy";
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.GetIsCombating())
        {
            Destroy(this.gameObject);
        }
        Destroy(gameObject, 1f);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.CompareTag(targetName))
        {
            other.transform.gameObject.GetComponent<AutoBattleUnit>().OnDamage(damage);
            Debug.Log("shadowbomb skill hit");
        }
    }
}