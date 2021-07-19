using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bean : LivingEntity
{
    //Behavior behavior;
    public Animator animator;
    [Header("-About Movement")]
    public float MoveSpeed = 1;

    public Vector3 Movement { get; set; }

    public void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public override void Start()
    {
        base.Start();
        //behavior = null;
    }

    void Update()
    {
        UpdateMovement();
    }

    public void UpdateMovement()
    {
        Vector3 move_vector_normal = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        bool run = Input.GetKey(KeyCode.LeftShift);
        move_vector_normal = move_vector_normal.normalized;
        transform.LookAt(Movement + transform.position);
        //transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime * (Movement.sqrMagnitude > 0 ? 1 : 0) * (run ? 2.5f : 1));
        animator.SetFloat("Walk", move_vector_normal.sqrMagnitude);
        animator.SetBool("Run", run);
        
        Movement = Vector3.zero;
    }

    protected override void Die()
    {
        dead = true;
        GameObject.Find("Option").GetComponent<Option>().GameOver();
    }
}
