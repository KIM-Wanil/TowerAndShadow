using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goblin Shaman 기본 공격
public class DarkSkill : MonoBehaviour
{
    // Start is called before the first frame update
    public AutoBattleUnit myUnit;
    private Vector3 targetPos;


    private GameObject darkSkillHit;

    private string targetName;
    IEnumerator myCoroutine;
    void Start()
    {
        darkSkillHit = Resources.Load<GameObject>("Prefabs/Skill&Attack/DarkSkillHit");
        targetName = "TeamEnemy";
        transform.LookAt(targetPos);
        
        //transform.localEulerAngles = new Vector3(0f, transform.rotation.y, transform.rotation.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.GetIsCombating())
        {
            Destroy(this.gameObject);
        }
        Destroy(gameObject,1f);
    }
    void OnTriggerEnter(Collider other)
    {      
        if (other.transform.gameObject.CompareTag(targetName) )
        {
            AutoBattleUnit unit = other.transform.gameObject.GetComponent<AutoBattleUnit>();
            unit.OnDamage(myUnit.stat.abilityPower);
            
            if (myUnit.GetComponent<Player>().stun.isEnhanced)//skill_enforce1 : Explosion
            {
                unit.isStunned = true;
            }
            if (myUnit.GetComponent<Player>().explosion.isEnhanced)//skill_enforce1 : Explosion
            {
                myCoroutine = HitEffect(unit.transform.position, unit);
                StartCoroutine(myCoroutine);              
            }
        }
    }
    public IEnumerator HitEffect(Vector3 targetPos,AutoBattleUnit unit)
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(darkSkillHit, targetPos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        unit.transform.gameObject.GetComponent<AutoBattleUnit>().OnDamage(myUnit.stat.abilityPower);
        StopCoroutine(myCoroutine);
        yield break;
    }


}