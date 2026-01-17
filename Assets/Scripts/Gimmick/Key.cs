using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    [SerializeField] UnityEvent OnUse = new(); //�������̂��AInspector����ݒ肵�܂� [Zan]
    public void Use()
    {
        OnUse.Invoke();
        Destroy(gameObject);
    }

}
