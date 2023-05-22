using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    const string HIDDEN_PLANE = "HiddenPlane";
    const string TEAM_PLAYER = "TeamPlayer";
    public float blockMoveTime = 0.1f;
    public bool isInField=false;
    public bool GetIsInField() { return isInField; }
    public void SetIsInField(bool state) { isInField = state; }
    RaycastHit hit, hitLayerMask;
    GameObject hitPos;
    Vector3 initPos;

    public GameObject TeamPlayer;
    public GameObject TeamPlayerBench;
    public bool isUnit=true; 
    private void Awake()
    {
        isUnit = true;
        if (TeamPlayer == null)
        {
            TeamPlayer = GameObject.Find("TeamPlayer");
        }
        if (TeamPlayerBench == null)
        {
            TeamPlayerBench = GameObject.Find("TeamPlayerBench");
        }
    }
    private void Start()
    {
        
        if (this.transform.parent==TeamPlayer.transform)
        {
            isInField = true;
        }
        else if (this.transform.parent == TeamPlayerBench.transform)
        {
            isInField = false;
        }
    }
    Vector3 getContactPoint(Vector3 normal, Vector3 planeDot, Vector3 A, Vector3 B)
    {
        Vector3 nAB = (B - A).normalized;
        return A + nAB * Vector3.Dot(normal, planeDot - A) / Vector3.Dot(normal, nAB);
    }
    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            initPos = this.transform.position;
            hitPos = new GameObject("Empty");
            hitPos.transform.position = hit.point;
            this.transform.SetParent(hitPos.transform);

        }
    }
    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layer = 1 << LayerMask.NameToLayer(HIDDEN_PLANE);
        if (Physics.Raycast(ray, out hitLayerMask, Mathf.Infinity, layer))
        {
            Vector3 normal = hitLayerMask.transform.up;
            Vector3 planeDot = hitPos.transform.position;
            Vector3 A = Camera.main.transform.position;
            Vector3 B = hitLayerMask.point;

            Vector3 temp = getContactPoint(normal, planeDot, A, B);
            Vector3 setPos = hitPos.transform.position;
            Mathf.Clamp(setPos.x, -2, 11);
            setPos.x = temp.x;
            setPos.z = temp.z;
            hitPos.transform.position = setPos;
        }
    }

    void OnMouseUp()
    {
        this.transform.parent = null;
        Destroy(hitPos);
        this.gameObject.layer = 0;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layer = 1 << LayerMask.NameToLayer(TEAM_PLAYER);
        if (Physics.Raycast(ray, out hitLayerMask, Mathf.Infinity, layer))
        {
            if(!hitLayerMask.transform.GetComponent<Drag>().isUnit)
            {
                this.transform.position = initPos;
            }
            else if(hitLayerMask.transform.gameObject.layer==6)
            {
                this.transform.position = hitLayerMask.transform.position;
                hitLayerMask.transform.position = initPos;
                //this.gameObject.layer = 6;//6:TeamPlayer
                Debug.Log(hitLayerMask.transform.gameObject.name);
                if (isInField)
                {
                    if(hitLayerMask.transform.GetComponent<Drag>().GetIsInField())
                    {
                        this.transform.SetParent(TeamPlayer.transform);
                    }
                    else//hitLayerMask.isInField==false
                    {
                        this.transform.SetParent(TeamPlayerBench.transform);
                        isInField = false;
                        hitLayerMask.transform.parent = null;
                        hitLayerMask.transform.SetParent(TeamPlayer.transform);
                        hitLayerMask.transform.GetComponent<Drag>().SetIsInField(true);
                    }
                }
                else //isInField==false
                {
                    if (hitLayerMask.transform.GetComponent<Drag>().GetIsInField())
                    {
                        this.transform.SetParent(TeamPlayer.transform);
                        isInField = true;
                        hitLayerMask.transform.parent = null;
                        hitLayerMask.transform.SetParent(TeamPlayerBench.transform);
                        hitLayerMask.transform.GetComponent<Drag>().SetIsInField(false);
                    }
                    else//hitLayerMask.isInField == false
                    {
                        this.transform.SetParent(TeamPlayerBench.transform);
                    }
                }
                return;
            }           
            
        }
        this.gameObject.layer = 6;
        Vector3 tempPos = this.transform.position;
        tempPos.x = Mathf.Round(tempPos.x);
        if (this.transform.position.z >= -1 && this.transform.position.z < 0)
        {
            tempPos.z = 0;
        }
        else if (this.transform.position.z < -1 && this.transform.position.z > -2)
        {
            tempPos.z = -2;
        }
        else
        {
            tempPos.z = Mathf.Round(tempPos.z);
        }

        if(!isInField && StageManager.instance.currentUnitCount >= StageManager.instance.maxUnitCount && tempPos.z>-0.1 &&isUnit)
        {
            this.transform.position = initPos;
            this.transform.SetParent(TeamPlayerBench.transform);
            isInField = false;
            Debug.Log("제자리로" + isUnit);
        }
        else
        {
            if(tempPos.z<-1.9)
            {
                this.transform.SetParent(TeamPlayerBench.transform);
                isInField = false;
                Debug.Log("벤치로" + isUnit);
            }
            else
            {
                this.transform.SetParent(TeamPlayer.transform);
                Debug.Log("필드로"+ isUnit);
                
                isInField = true;
            }
            moveBlock(tempPos);
        }

        
    }

    public void moveBlock(Vector3 targetPosition)
    {
        StartCoroutine(moveBlockCoroutine(targetPosition));
    }

    private IEnumerator moveBlockCoroutine(Vector3 targetPosition)
    {
        float elapsedTime = 0.0f;
        Vector3 currentPosition = transform.position;

        while (elapsedTime < blockMoveTime)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / blockMoveTime);

            yield return null;
        }

        transform.position = targetPosition;
        if (this.transform.position.z <= -1.9)
        {
            this.transform.SetParent(TeamPlayerBench.transform);
        }
        else
        {
            this.transform.SetParent(TeamPlayer.transform);
        }
    }
}
