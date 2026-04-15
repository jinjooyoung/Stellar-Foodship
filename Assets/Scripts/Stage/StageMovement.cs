using UnityEngine;

public class StageMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    Vector3 m_Movement;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = (Vector3.forward * vertical) + (Vector3.right * horizontal);

        this.transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);

        if (m_Movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(m_Movement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }
}
