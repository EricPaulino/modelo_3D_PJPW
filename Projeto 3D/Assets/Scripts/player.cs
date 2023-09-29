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
    public List<Transform> enemyList = new List<Transform>();
        
        
    private Vector3 moveDirection;
    private float TurnSmoothVelocity;
    private CharacterController Con;
    private Transform Cam;
    private bool isWalking;
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
                if (!Anim.GetBool("Attack"))
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
                    isWalking = true;
                }

                else
                {
                    moveDirection = Vector3.zero;
                    Anim.SetBool("Walk", false);
                }
                
            }
            else if(isWalking)
            {
                Anim.SetInteger("Transition", 2);
                Anim.SetBool("Walk", false);
                moveDirection = Vector3.zero;
                isWalking = false;
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
                if (Anim.GetBool("Walk"))
                {
                    Anim.SetBool("Walk", false);
                    Anim.SetInteger("Transition", 0);
                }

                if (!Anim.GetBool("Walk"))
                {
                    StartCoroutine("Attack");
                }
            }
        }
    }

    IEnumerator Attack()
    {
        Anim.SetBool("Attack", true);
        Anim.SetInteger("Transition", 1);
        yield return new WaitForSeconds(1f);
        GetEnemiesList();

        foreach (Transform e in enemyList)
        {
            Debug.Log(e.name);
        }

        yield return new WaitForSeconds(0.5f);
            Anim.SetInteger("Transition", 0);
            Anim.SetBool("Attack", false);
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}
