using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSkill : MonoBehaviour
{
    // Start is called before the first frame update
    public AutoBattleUnit myUnit;
    public bool isSlash = false;
    public Vector3 targetPosition;
    private string targetName;
    void Start()
    {
        targetName = "TeamPlayer";
        transform.LookAt(targetPosition);
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
            other.transform.gameObject.GetComponent<AutoBattleUnit>().OnDamage(myUnit.stat.abilityPower);
            if(isSlash)
            {
                other.transform.gameObject.GetComponent<AutoBattleUnit>().isStunned = true;
            }
            Debug.Log("skill hit");
        }
    }
}