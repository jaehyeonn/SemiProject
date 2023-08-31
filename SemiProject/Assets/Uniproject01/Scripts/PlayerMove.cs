using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float speed = 10f;
    public float jumForce = 5f;
    public float dash = 30f;
    public float rotSpeed = 20f;

    private Vector3 dir = Vector3.zero;

    private bool ground = false;
    public LayerMask layer;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        dir.Normalize();

        CheckGround();

        bool isMove = dir != Vector3.zero;
        animator.SetBool("isMove", isMove);

        if (Input.GetButtonDown("Jump") && ground)
        {
            // 점프 애니메이션 제어
            animator.SetTrigger("Jump");

            Vector3 jumPower = Vector3.up * jumForce;
            rigidbody.AddForce(jumPower, ForceMode.VelocityChange);
        }
        
        if(Input.GetButtonDown("Dash"))
        {

            rigidbody.drag = 20f;
            // 대쉬 애니메이션 제어
            animator.SetTrigger("Dash");

            Vector3 dashPower = transform.forward * -Mathf.Log(1 / rigidbody.drag) * dash;
            rigidbody.AddForce(dashPower, ForceMode.VelocityChange);

            StartCoroutine(enumerator());

        }
        
    }
    IEnumerator enumerator()
    {
        yield return new WaitForSeconds(0.1f);

        rigidbody.drag = 0f;
    }

    private void FixedUpdate()
    {
        if (dir != Vector3.zero)
        {
            // 지금 바라보는 방향 부호 != 나아갈 방향 부호
            if(Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
        }

        rigidbody.MovePosition(gameObject.transform.position + dir * speed * Time.deltaTime);
    }

    void CheckGround()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.4f, layer))
        {
            ground = true;
            Debug.LogFormat("땅인가?");

        }
        else
        {
            ground = false;
            Debug.LogFormat("점프했나?");

        }
    }
}
