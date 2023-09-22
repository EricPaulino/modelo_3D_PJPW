using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class player : MonoBehaviour
{

    public float Speed;
    public float SmoothRotTime;
    public float Gravity;
    public float colliderRadius;
    
    private Vector3 moveDirection;
    private float TurnSmoothVelocity;
    private CharacterController Con;
    private Transform Cam;
    public Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Con = GetComponent<CharacterController>();
        Cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        geteMouseinput();
    }

    private void Move()
    {
        if (Con.isGrounded)
        {


            // pega a entrada na horizontal (tecla direita/ esquerda)
            float horizontal = Input.GetAxis("Horizontal");
            // pega a entrada na vertical (tecla cima/baixo)
            float vertical = Input.GetAxis("Vertical");
            // variavel local que armazena o valor do eixo horizontal e vertical
            Vector3 direction = new Vector3(horizontal, 0f, vertical);
            // verifica se o personagem esta se movendo (se for > 0)
            if (direction.magnitude > 0)
            {
                //armazena o ãngulo e rotação da camera
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
                //armazena a rotação mais suave
                float SmoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref TurnSmoothVelocity,
                    SmoothRotTime);
                //rotaciona o personagem
                transform.rotation = Quaternion.Euler(0, SmoothAngle, 0f);
                //aramazena a direção
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * Speed;
                
                Anim.SetInteger("Transition", 0);
            }
            else
            {
                moveDirection = Vector3.zero;
                Anim.SetInteger("Transition", 2);
            }
        }

        moveDirection.y -= Gravity * Time.deltaTime;
        Con.Move(moveDirection * Time.deltaTime);
    }

    void geteMouseinput()
    {
        if (Con.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        Anim.SetInteger("Transition", 1);
        yield return new WaitForSeconds(10f);
    }

    void GetEnemiesList()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            
        }
        
    }

    private void OnGrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position+transform.forward, colliderRadius);
    }
}
