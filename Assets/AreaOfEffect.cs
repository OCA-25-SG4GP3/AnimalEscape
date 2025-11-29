using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] float aoeSize = 3.0f;
    [SerializeField] GameObject effectObj;
    private void Awake()
    {
        ParticleSystem particleSys = effectObj.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule newModule = particleSys.main;
        newModule.startSizeMultiplier = aoeSize;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AnimalControlSimple>().EnterStunedState();
        }
    }
}
