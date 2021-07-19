using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableObject : MonoBehaviour, ILiftable
{
    public Material Outline;
    public Transform liftedPoint;
    Rigidbody my_rigidbody;
    MeshRenderer mesh;
    BeanController lifter;
    bool onHand;
    List<BoxCollider> colliders;
    private Transform original_parent;

    public void LeftShift(Transform _transform)
    { 
        onHand = !onHand;
        if (onHand)
        {   
            PhysicsOff();
            original_parent = transform.parent;
            transform.parent = _transform;
        }
        else
        {   
            PhysicsOn();
            transform.parent = original_parent;
        }
        Vector3 offset = transform.position - liftedPoint.position;
        transform.position = _transform.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!onHand && lifter == null && other.tag == "Player")
        {
            lifter = other.GetComponent<BeanController>();
            lifter.liftObject = this;
            List<Material> materials = new List<Material>();
            mesh.GetMaterials(materials);
            materials.Add(Outline);
            mesh.materials = materials.ToArray();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!onHand && lifter != null && other.tag == "Player")
        {
            lifter.liftObject = null;
            lifter = null;
            List<Material> materials = new List<Material>();
            mesh.GetMaterials(materials);
            materials.RemoveAt(materials.Count-1);
            mesh.materials = materials.ToArray();
        }
    }

    void Start()
    { 
        colliders = new List<BoxCollider>(GetComponents<BoxCollider>());
        my_rigidbody = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        lifter = null;
        onHand = false;
    }

    void PhysicsOff(){
        colliders[0].enabled = false;
        colliders[1].enabled = false;
        my_rigidbody.isKinematic = true;
        my_rigidbody.Sleep();
    }

    void PhysicsOn(){
        colliders[0].enabled = true;
        colliders[1].enabled = true;
        my_rigidbody.WakeUp();
        my_rigidbody.isKinematic = false;
    }
}
