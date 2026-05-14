using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    public Transform m_pointFrom;
    public Transform m_pointTo;

    // Update is called once per frame
    void Update()
    {
        if (m_pointTo.gameObject.activeInHierarchy && m_pointFrom.gameObject.activeInHierarchy)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            Vector3 pos = m_pointFrom.position;
            Vector3 target = m_pointTo.position;
            Vector3 delta = target - pos;
            transform.position = pos;
            transform.LookAt(target);
            float dist = delta.magnitude;
            transform.localScale = new Vector3(1.0f, 1.0f, dist);
        }
        else
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
