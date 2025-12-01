using Unity.VisualScripting;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] float aoeSize = 3.0f;
    [SerializeField] GameObject effectObj;
   

    [SerializeField] float destroyTime;
    const float sphereRadiusPerEffectSize = 0.3333334f; //1 Unit
    private void Awake()
    {
        ParticleSystem particleSys = effectObj.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule newModule = particleSys.main;
        newModule.startSizeMultiplier = aoeSize;

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = aoeSize * sphereRadiusPerEffectSize;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Destroy(this.gameObject,destroyTime);
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    void Test()
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
