using UnityEngine;
using System.Collections;

public class CustumGizmo : MonoBehaviour 
{
    public Color m_Color = Color.yellow;
    private float m_Radius = 0.05f;

    void OnDrawGizmos()
    {
        Gizmos.color = m_Color;
        Gizmos.DrawSphere( transform.position, m_Radius );
    }

}
