using UnityEngine;
using System.Collections;

public class AppQuit : MonoBehaviour 
{
    private bool  m_Is;
	// Use this for initialization
	void Start () 
    {
	    m_Is = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyUp(KeyCode.Escape) && !m_Is)
        {
            
            if(CMessageBox.Create( "Do you quit app ?", 2, onBtn) != null)
                m_Is = true;
        }
	}

    void onBtn(bool a)
    {
        m_Is = false;
        if(!a) return;
        Application.Quit();
    }
}
