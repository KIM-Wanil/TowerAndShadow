using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goblin Shaman 기본 공격
public class FireSkill : MonoBehaviour
{
    // Start is called before the first frame update
    private AutoBattleUnit skillTargetUnit;
    public AutoBattleUnit myUnit;
    private Vector3 targetPos;
    private Vector3 targetDir;
    Collider collider;
    private GameObject explosionParticle;
    private GameObject fireParticle;
    private string targetName;
    private bool isHit = false;

    void Start()
    {
        //skillTargetUnit = transform.GetComponentInParent<AutoBattleUnit>().skillTargetUnit;
        //myUnit = transform.GetComponentInParent<AutoBattleUnit>();      
        collider = transform.GetComponentInChildren<SphereCollider>();
        explosionParticle = transform.Find("Effect").Find("Explosion").gameObject;
        fireParticle = transform.Find("Effect").Find("Fire").gameObject;
        targetPos = myUnit.farthestUnitPosition;
        targetDir = (targetPos - myUnit.transform.position).normalized;
        targetDir.y = 0f;
        targetName = myUnit.targetName;
        transform.LookAt(targetPos);
        //transform.localEulerAngles = new Vector3(0f, transform.rotation.y, transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        
        //targetUnit = transform.GetComponentInParent<AutoBattleUnit>().targetUnit;
        //collider.transform.position = Vector3.MoveTowards(collider.transform.position, targetPos, 3.5f * Time.deltaTime);
        collider.transform.position += targetDir * Time.deltaTime * 3.5f;
        //transform.Rotate(new Vector3(0f, transform.rotation.y, transform.rotation.z), Space.Self);
        //targetPos = new Vector3(targetUnit.transform.position.x, 0f, targetUnit.transform.position.z);
        //targetUnit = transform.GetComponentInParent<AutoBattleUnit>().targetUnit;
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, 10f * Time.deltaTime);
        //transform.LookAt(skillTargetUnit.transform);
        //transform.Rotate(new Vector3 (0f, 0f, 0f), Space.Self);
        //if (skillTargetUnit.isDie)
        //{
        //    Destroy(gameObject);
        //}
        if (collider.transform.position.x>11 || collider.transform.position.x < -1 || collider.transform.position.z > 10 || collider.transform.position.z < 0)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.CompareTag(targetName) && !isHit)
        {
            //myUnit.mpBarSlider.value = myUnit.currentMP;
            //적을 attack시 적 OnDamage메서드 실행
            isHit = true;
            other.transform.gameObject.GetComponent<AutoBattleUnit>().OnDamage(myUnit.stat.abilityPower);
            explosionParticle.transform.position = collider.transform.position + new Vector3(0f, 1f, 0f) + (targetDir*0.5f);
            explosionParticle.SetActive(true);
            fireParticle.SetActive(false);
            Destroy(gameObject,3f);
        }

    }


}