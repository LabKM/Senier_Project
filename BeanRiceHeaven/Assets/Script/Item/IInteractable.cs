using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
   void Interact(Transform _transform);

   void OnEffectOfOutline();
   void OffEffectOfOutline();
}
