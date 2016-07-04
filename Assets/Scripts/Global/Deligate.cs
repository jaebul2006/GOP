using UnityEngine;
using System.Collections;

public class Deligate : MonoBehaviour 
{
    public Transform    m_Del;
    public Transform    m_BG;
    private Vector3     m_vRot = new Vector3();
    private int         m_DisCount;

	// Use this for initialization
	void Start () 
    {        
    }
	
	// Update is called once per frame
	void Update () 
    {        
        m_vRot.z -= (50f*Time.deltaTime);
	    m_Del.localRotation = Quaternion.Euler(m_vRot);        
	}
    
    public void UpCount()
    {
        m_DisCount++;        
    }
    
    public void DownCount()
    {
        m_DisCount--;
        if(m_DisCount <= 0)
            Destroy(gameObject);
    }

    static public Deligate Create()
    {
        GameObject ObjRtn = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Panel_Deligate"));
        ObjRtn.transform.parent = DataMgr.Inst.m_TopObj.transform;
        ObjRtn.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        ObjRtn.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        return ObjRtn.GetComponent<Deligate>();
    }
}
