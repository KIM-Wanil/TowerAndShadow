using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Goblin Shaman 기본 공격
public class FireBall : MonoBehaviour
{
    // Start is called before the first frame update
    private AutoBattleUnit targetUnit;
    public AutoBattleUnit myUnit;
    private Vector3 targetPos;
    void Start()
    {
        targetUnit = myUnit.targetUnit;
        //myUnit = transform.GetComponentInParent<AutoBattleUnit>();
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = new Vector3(targetUnit.transform.position.x, 0f, targetUnit.transform.position.z);
        //targetUnit = transform.GetComponentInParent<AutoBattleUnit>().targetUnit;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 10f * Time.deltaTime);
        if(targetUnit.isDie || myUnit.isDie)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject == targetUnit.transform.gameObject)
        {
            //적을 attack시 MP 10회복
            myUnit.stat.currentMP += myUnit.stat.hitMP;
            //myUnit.mpBarSlider.value = myUnit.currentMP;
            //적을 attack시 적 OnDamage메서드 실행
            targetUnit.OnDamage(myUnit.stat.abilityPower);
            Destroy(gameObject);
        }
        
    }
}
