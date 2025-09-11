using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Cage : MonoBehaviour
{
    [SerializeField,ReadOnly] List<EscapeAnimalAction> escapeAnimals = new();
    [SerializeField] List<GameObject> cageWalls = new();
    private void Start()
    {
        EscapeAnimalAction[] escapeAnimalsFound = FindObjectsByType<EscapeAnimalAction>(FindObjectsSortMode.None);
        escapeAnimals = escapeAnimalsFound.ToList();
    }
    public void FreeAnimalsFromCage()
    {
        foreach (var escapeAnimal in escapeAnimals)
        {
            escapeAnimal.IsFree = true;
        }
        DestroyCageWalls();
    }

    void DestroyCageWalls()
    {
        foreach(var cageWall in cageWalls )
        {
            Destroy(cageWall);
        }
    }
}
