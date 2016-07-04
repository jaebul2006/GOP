using UnityEngine;
using System.Collections;

public class CloudMgr : MonoBehaviour 
{
    public Transform []  m_TopCloud;    
    public Transform []  m_BottomCloud;    

    private Vector3 [] m_vTopCloud;
    private Vector3 [] m_vBottomCloud;    

	private float m_fSpeed = 10;

    // Use this for initialization
	void Start () 
    {

        m_vTopCloud = new Vector3[2];
        m_vBottomCloud = new Vector3[2];

        for(int i=0; i<m_TopCloud.Length; i++)
            m_vTopCloud[i] = new Vector3(0f, m_TopCloud[i].localPosition.y, m_TopCloud[i].localPosition.z);
        for(int i=0; i<m_BottomCloud.Length; i++)
            m_vBottomCloud[i] = new Vector3(0f, m_BottomCloud[i].localPosition.y, m_BottomCloud[i].localPosition.z);
	}
	
	// Update is called once per frame
	void Update()
    {
        for(int i=0; i<m_TopCloud.Length; i++)
        {            
            m_vTopCloud[i].x = m_TopCloud[i].localPosition.x - (m_fSpeed * Time.deltaTime);
	        m_TopCloud[i].localPosition = m_vTopCloud[i];            

            int Index = (i+1) % m_TopCloud.Length;
            if(m_vTopCloud[i].x < 0f && m_vTopCloud[Index].x < 0f )
            {            
                m_vTopCloud[Index].x = 715;               
                m_TopCloud[Index].localPosition = m_vTopCloud[Index];
            }
        }
        
        for(int i=0; i<m_BottomCloud.Length; i++)
        {            
            m_vBottomCloud[i].x = m_BottomCloud[i].localPosition.x - (m_fSpeed * Time.deltaTime);
	        m_BottomCloud[i].localPosition = m_vBottomCloud[i];            

            int Index = (i+1) % m_BottomCloud.Length;
            if(m_vBottomCloud[i].x < 0f && m_vBottomCloud[Index].x < 0f )
            {            
                m_vBottomCloud[Index].x = 715;               
                m_BottomCloud[Index].localPosition = m_vBottomCloud[Index];
            }
        }
        
	}
}
