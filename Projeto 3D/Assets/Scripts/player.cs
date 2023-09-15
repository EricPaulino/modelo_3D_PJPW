using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class player : MonoBehaviour
{
    public float Speed;
    
    
    public float SmoothRotTime;
    private float TurnSmoothVelocity;

    private CharacterController Con;
    // Start is called before the first frame update
    void Start()
    {
        Con = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        if (direction.magnitude > 0)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref TurnSmoothVelocity, SmoothRotTime);
            transform.rotation = Quaternion.Euler(0, SmoothAngle,0f);
            Con.Move(direction * Speed * Time.deltaTime);
        }
    }
}
