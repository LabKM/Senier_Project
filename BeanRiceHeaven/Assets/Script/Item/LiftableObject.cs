using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableObject : MonoBehaviour, IInteractable
{
    public Material Outline;
    public Transform liftedPoint;
    Rigidbody my_rigidbody;
    protected MeshRenderer mesh;
    protected Inventory lifter;
    protected bool onHand;
    List<Collider> colliders;
    private Transform original_parent;
    public virtual void Interact(Transform _transform)
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
        transform.localRotation = Quaternion.identity;
        transform.position = _transform.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lifter = other.GetComponent<BeanController>().inventory;
            lifter.OnCloseObejct(this);
            Debug.Log("Add Item" + transform.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Reomve Item" + transform.name);
            lifter.OutofObject(this);
            lifter = null;
        }
    }

    public bool isOutline { get; private set; }
    public void OnEffectOfOutline(){
        if(!isOutline){
            List<Material> materials = new List<Material>();
            mesh.GetMaterials(materials);
            materials.Add(Outline);
            mesh.materials = materials.ToArray();
            isOutline = true;
        }
    }

    public void OffEffectOfOutline(){
        if(isOutline){
            List<Material> materials = new List<Material>();
            mesh.GetMaterials(materials);
            materials.RemoveAt(1);
            mesh.materials = materials.ToArray();
            isOutline = false;
        }
    }

    void Start()
    { 
        colliders = new List<Collider>(GetComponents<Collider>());
        my_rigidbody = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        lifter = null;
        onHand = false;
    }

    public void PhysicsOff(){
        colliders[0].enabled = false;
        colliders[1].enabled = false;
        my_rigidbody.isKinematic = true;
        my_rigidbody.Sleep();
    }

    public void PhysicsOn(){
        colliders[0].enabled = true;
        colliders[1].enabled = true;
        my_rigidbody.WakeUp();
        my_rigidbody.isKinematic = false;
    }
}
