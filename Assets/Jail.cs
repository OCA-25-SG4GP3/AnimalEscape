using UnityEngine;

public class Jail : MonoBehaviour
{
    [SerializeField] public Transform jailedObjectSlotT;
    void OnAddPrisoner(GameObject newPrisoner)
    {
    }
    void OnReleasePrisoner()
    {
        UpdateCandidateObjects();
    }
    void UpdateCandidateObjects()
    {
        //logicCon.targetCandidateObjects.Add(candidate => candidate.GetComponent<PlayerInfo>().hasCaught);
    }
}
