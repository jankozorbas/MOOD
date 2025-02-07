using UnityEngine;

public class Recoil : MonoBehaviour
{
    // Tutorial used https://www.youtube.com/watch?v=geieixA4Mqc&list=PLx3BamUdxzPwYOmhagGf3MpD62RwfyXVg&index=303

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    //private bool isAiming = false;

    private void Update()
    {
        //isAiming = aiming; check if you are aiming down sight in gun script or player script or somewhere and set isAiming to that
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
        // make this subscribed to an event OnShoot that you make in gun script to avoid referencing
        // when ADS is setup, use this
        /*if (isAiming) targetRotation += new Vector3(recoilAimX, Random.Range(-recoilAimY, recoilAimY), Random.Range(-recoilAimZ, recoilAimZ));
        else targetRotation += new Vector3(recoilHipX, Random.Range(-recoilHipY, recoilHipY), Random.Range(-recoilHipZ, recoilHipZ));*/

        targetRotation += new Vector3(FindObjectOfType<Gun>().recoilHipX, 
                                        Random.Range(-FindObjectOfType<Gun>().recoilHipY, FindObjectOfType<Gun>().recoilHipY), 
                                        Random.Range(-FindObjectOfType<Gun>().recoilHipZ, FindObjectOfType<Gun>().recoilHipZ));
    }
}