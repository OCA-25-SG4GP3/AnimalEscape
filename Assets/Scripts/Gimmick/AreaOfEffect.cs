using Unity.VisualScripting;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] float aoeSize = 3.0f;
    [SerializeField, Header("大きくなる円")] GameObject actualEffectObj;
    [SerializeField, Header("最大の円をプレイヤーに見せる")] GameObject effectObjHelper;

    [SerializeField, Header("何秒に入ると、1フレームに、コリジョンチェックを有効になる")] float snapshotTime = 3.0f;
    Cooldown snapshotCd = new();
    const float sphereRadiusPerEffectSize = 0.3333334f; //1 Unit
    private void Awake()
    {
        //中のAoEエフェクトセット
        ParticleSystem particleSys = actualEffectObj.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule actualMainModule = particleSys.main;
        actualMainModule.startSizeMultiplier = aoeSize;
        actualMainModule.startLifetime = snapshotTime; //エフェクトが最大になるまでの何秒

        //外のAoEエフェクト
        ParticleSystem helperParticleSys = effectObjHelper.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule helperMainModule = helperParticleSys.main;
        helperMainModule.startSizeMultiplier = aoeSize;
        helperMainModule.startLifetime = snapshotTime; 

        particleSys.Play();

        //コリジョンセット
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = aoeSize * sphereRadiusPerEffectSize;
    }

    void Start()
    {
        snapshotCd.SetNewCooldown(snapshotTime);
        snapshotCd.StartCooldown();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void Snapshot(Collider other)
    {
        other.GetComponent<AnimalControlSimple>().SetStunnedState();
    }
    private void OnTriggerStay(Collider other)
    {
        //コリジョンをそのままにして、
        //X時間たったら、ダメージトリガーをONにし、破壊する
        if (!snapshotCd.IsCooldown)
        {
            print("asd");
            if (other.CompareTag("Player"))
            {
                Snapshot(other);
            }
            Destroy(this.gameObject);
        }
    }
}
