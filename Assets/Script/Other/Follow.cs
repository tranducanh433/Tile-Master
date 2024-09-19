using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Follow : MonoBehaviour
{
    [SerializeField] float m_moveSpeed = 20;

    public UnityEvent OnFinishFollow = new UnityEvent();
    Transform m_target;

    public void SetFollowTarget(Transform target)
    {
        m_target = target;
    }

    void Update()
    {
        if (m_target == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, m_target.position, m_moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, m_target.position) <= 0)
        {
            OnFinishFollow.Invoke();
            Destroy(this);
        }
    }
}
