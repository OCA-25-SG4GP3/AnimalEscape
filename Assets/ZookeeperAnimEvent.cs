using Unity.Cinemachine;
using UnityEngine;

public class ZookeeperAnimEvent : MonoBehaviour
{
    [SerializeField] public AILogicController _logicController;

    public void AnimEventTryCatchAnimal()
    {
        //無理やりキャストする。注意
        EnemyStateInfiniteChaseSO infState = _logicController.CurrentState as EnemyStateInfiniteChaseSO;
        if (infState) infState.TryCatchAnimal();
    }

}
