using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    [SerializeField] UnityEvent OnUse = new(); //‰½‚ğ‚â‚é‚Ì‚©AInspector‚©‚çİ’è‚µ‚Ü‚· [Zan]
    public void Use()
    {
        OnUse.Invoke();
    }

}
