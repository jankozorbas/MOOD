using UnityEngine;

public class GunSoundManager : MonoBehaviour
{
    private void OnEnable()
    {
        GunSwitcher.OnGunSwitched += PlayCorrectGunSwitchSound;
    }

    private void OnDisable()
    {
        GunSwitcher.OnGunSwitched -= PlayCorrectGunSwitchSound;
    }

    private void PlayCorrectGunSwitchSound(Gun newGun)
    {
        string soundName = GetGunSoundName(newGun.gunType);
        AudioManager.Instance.PlaySound(soundName);
    }

    private string GetGunSoundName(Gun.GunType gunType)
    {
        switch (gunType)
        {
            case Gun.GunType.Pistol:
                return "PistolSwitch";
            case Gun.GunType.Rifle:
                return "RifleSwitch";
            default:
                return "DefaultGun";
        }
    }
}