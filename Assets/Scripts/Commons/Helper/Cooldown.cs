using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Cooldown
{
    public Cooldown()
    {

    }
    public Cooldown(float initMaxCooldown)
    {
        cooldownDuration = initMaxCooldown;
    }

    [SerializeField] private float cooldownDuration = 2.0f;
    public float GetCooldownDuration => 2.0f;
    //private float maxCooldown = 2.0f;
    private float minCooldown = 0.0f;
    private float cooldownTick = 0.0f;
    public bool IsCooldown => Time.time < cooldownTick;
    public void SetNewCooldown(float newCooldownValue) => cooldownDuration = newCooldownValue;
    public void SetMinimalCooldown(float newCooldownValue) => minCooldown = newCooldownValue;
    public void ReduceCooldown(float reduceCooldownFactor)
    {
        if (cooldownDuration - reduceCooldownFactor > minCooldown) cooldownDuration -= reduceCooldownFactor;
        else { Debug.Log("CANNOT REDUCE COOLDOWN TO NEGATIVE"); }
    }
    public void StartCooldown() => cooldownTick = Time.time + cooldownDuration;

    public void SetRandomizedCooldown(float minRandCooldownIncl, float maxRandCooldownExcl)
    {
        float newCd = Random.Range(minRandCooldownIncl, maxRandCooldownExcl);
        SetNewCooldown(newCd);
    }
}
