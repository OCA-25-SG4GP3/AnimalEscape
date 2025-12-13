using UnityEngine;
using UnityEngine.InputSystem.Layouts;

public class CatchPosition : MonoBehaviour
{
    [SerializeField] private EnemyStateInfiniteChaseSO enemyState;
    private Transform hand;
    [SerializeField] private Vector3 offset = new Vector3(0.0f, -5.0f, 0.0f);

    void Start()
    {
    }

    public void SetCatch(EnemyStateInfiniteChaseSO enemyState)
    {
        this.enemyState = enemyState;
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Collider>().isTrigger = true;
        hand = FindDeepChild(enemyState.LogicController.ModelObj.transform, "C_Carry");
    }
    Transform FindDeepChild(Transform parent, string name) 
    { 
        foreach (Transform child in parent) 
        { 
            if (child.name == name) 
                return child; 
            Transform result = FindDeepChild(child, name); 
            if (result != null) 
                return result; 
            
        } 
        return null; 
    }
    void LateUpdate()
    {
        if(enemyState == null)
            return;

        // 飼育員がキャッチモーションの時
        if(enemyState.animator.GetBool("IsCatching"))
        {
            Debug.Log("キャッチされた！");
            transform.position = hand.position + offset;
            transform.rotation = hand.rotation;

        }
    }
}
