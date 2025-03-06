using UnityEngine;

public class Recoil : MonoBehaviour
{
    // Tutorial used https://www.youtube.com/watch?v=geieixA4Mqc&list=PLx3BamUdxzPwYOmhagGf3MpD62RwfyXVg&index=303

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Gun gun;

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, FindObjectOfType<Gun>().returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, FindObjectOfType<Gun>().snappiness * Time.fixedDeltaTime); //fixeddeltatime????

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilOnShoot()
    {
        gun = FindObjectOfType<Gun>().GetComponent<Gun>();

        // make this subscribed to an event OnShoot that you make in gun script to avoid referencing
        targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.recoilY, gun.recoilY), Random.Range(-gun.recoilZ, gun.recoilZ));

        /*targetRotation += new Vector3(FindObjectOfType<Gun>().recoilHipX, 
                                        Random.Range(-FindObjectOfType<Gun>().recoilHipY, FindObjectOfType<Gun>().recoilHipY), 
                                        Random.Range(-FindObjectOfType<Gun>().recoilHipZ, FindObjectOfType<Gun>().recoilHipZ));*/
    }
}