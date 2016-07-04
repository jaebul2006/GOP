using UnityEngine;
using System.Collections;

public class CNetWebMgr : MonoBehaviour 
{
    private string m_Address = "http://localhost:8080/HelloWord/Hello.jsp"; 

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}


    

    public void CreateWebPostDate()
    {
        WWWForm _WWWForm = new WWWForm();

        _WWWForm.AddField("UID", "Hanttl1004");
        _WWWForm.AddField("Ruby", 1000);        

        WWW _www = new WWW(m_Address, _WWWForm);

        StartCoroutine( WaitForRequest(_www) );
        
    }

    private IEnumerator WaitForRequest(WWW www) 
    { 
        yield return www;        

        // check for errors 
        if (www.error == null) 
        { 
            int H1 = www.text.IndexOf("<h1>");
            int H2 = www.text.IndexOf("</h1>");
            int Start = H1+"<h1>".Length;
            int End = H2-Start;
            string  Text = www.text.Substring( Start, End );

            Debug.Log("WWW Ok!1:" + www.text); 
            Debug.Log("------------------------------"); 
            Debug.Log("WWW Ok!2:" + Text);             
        } 
        else 
        { 
            Debug.Log("WWW Error: " + www.error); 
        } 
    }


}
