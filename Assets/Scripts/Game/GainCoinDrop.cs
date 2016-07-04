using UnityEngine;
using System.Collections;

public class GainCoinDrop : MonoBehaviour 
{    
    private float _firing_angle;
    private float _gravity = 1.5f;//9.8f;

    [System.NonSerialized] public GameObject    m_Target;
    [System.NonSerialized] public Transform     _projectile;
    [System.NonSerialized] public Transform     _my_tm;    
    [System.NonSerialized] public Defines.Delegate_None m_Send;
    

    void Awake()
    {        
    }

    public void DirSet(bool right)
    {
    
        if (right)
            StartCoroutine(FlyCoin(right));
        else
            StartCoroutine(FlyCoin(right));
    }

    IEnumerator FlyCoin(bool isright)
    {
        _projectile.position = _my_tm.position + new Vector3(0f, 0f, 0f);
         
        _firing_angle               = Random.Range(40f, 80f);
        float target_distance_y     = Random.Range(0.4f, 0.5f);
        float target_distance_x     = target_distance_y/3f;
                
        float vx                    = Mathf.Sqrt(target_distance_x) * Mathf.Cos(_firing_angle * Mathf.Deg2Rad);
        float vy                    = Mathf.Sqrt(target_distance_y) * Mathf.Sin(_firing_angle * Mathf.Deg2Rad);
       
        float flight_duration = target_distance_x / vx;
        //Debug.Log("체공시간:" + flight_duration);
        if (flight_duration > 1.2f)
            flight_duration = 1.2f;

        if (!isright)
            vx = -vx;
            
        float elapse_time = 0;

        while (elapse_time < flight_duration)
        {
            _projectile.Translate(vx * Time.deltaTime, (vy - (_gravity * elapse_time)) * Time.deltaTime, 0f);
            elapse_time += Time.deltaTime;
            yield return null;
            if(elapse_time > flight_duration)
            {
                MagnetEffect(); // 빨려들어가는 애니메이션시작.
                //DestroyImmediate(gameObject); 
                break;
            }
        }
    }

    private void MagnetEffect()
    {        
        StartCoroutine(MagnetMove(m_Target));
    }

    private IEnumerator MagnetMove(GameObject target)
    {
        Vector3 direction;
        float end_distance = 0.1f;

        while(true)
        {
            yield return null;
            float distance = (transform.position - target.transform.position).magnitude;
            direction = (target.GetComponent<Collider>().bounds.center - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 2.5f;       

            if (distance <= end_distance)
            {             
                m_Send();
                DestroyImmediate(gameObject);
                break;
            }
        }
    }
}