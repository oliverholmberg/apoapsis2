using UnityEngine;
using System.Runtime.InteropServices;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    bool isSupported;
    float thrustHapticTimer;
    bool wasThrusting;
    int thrustTickCount;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void _HapticPrepare();
    [DllImport("__Internal")] static extern void _HapticImpactLight();
    [DllImport("__Internal")] static extern void _HapticImpactMedium();
    [DllImport("__Internal")] static extern void _HapticImpactHeavy();
    [DllImport("__Internal")] static extern void _HapticNotificationSuccess();
    [DllImport("__Internal")] static extern void _HapticNotificationWarning();
    [DllImport("__Internal")] static extern void _HapticNotificationError();
    [DllImport("__Internal")] static extern void _HapticSelection();
#else
    static void _HapticPrepare() {}
    static void _HapticImpactLight() {}
    static void _HapticImpactMedium() {}
    static void _HapticImpactHeavy() {}
    static void _HapticNotificationSuccess() {}
    static void _HapticNotificationWarning() {}
    static void _HapticNotificationError() {}
    static void _HapticSelection() {}
#endif

    void Awake()
    {
        Instance = this;
#if UNITY_IOS && !UNITY_EDITOR
        isSupported = true;
        _HapticPrepare();
#else
        isSupported = false;
#endif
    }

    public void ThrustTick()
    {
        if (!isSupported) return;

        // Ignition kick
        if (!wasThrusting)
        {
            _HapticImpactHeavy();
            wasThrusting = true;
            thrustTickCount = 0;
            thrustHapticTimer = 0.06f;
            return;
        }

        thrustHapticTimer -= Time.deltaTime;
        if (thrustHapticTimer <= 0f)
        {
            thrustTickCount++;
            // Alternate between light impact and selection for organic rumble
            if (thrustTickCount % 3 == 0)
                _HapticImpactLight();
            else
                _HapticSelection();

            // Slightly randomized interval for organic feel
            thrustHapticTimer = Random.Range(0.06f, 0.10f);
        }
    }

    public void ThrustStop()
    {
        thrustHapticTimer = 0f;
        wasThrusting = false;
        thrustTickCount = 0;
    }

    public void CoinCollect()
    {
        if (!isSupported) return;
        _HapticImpactMedium();
    }

    public void NearMiss()
    {
        if (!isSupported) return;
        _HapticImpactMedium();
    }

    public void OrbitAchieved()
    {
        if (!isSupported) return;
        _HapticNotificationSuccess();
    }

    public void Crash()
    {
        if (!isSupported) return;
        _HapticNotificationError();
    }

    public void LevelComplete()
    {
        if (!isSupported) return;
        _HapticNotificationSuccess();
    }
}
