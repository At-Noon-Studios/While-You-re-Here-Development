using Interactable.Holdable;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public interface IObjectHolder
    {
        public void ClearHeldObject(GameObject obj);
    }
}