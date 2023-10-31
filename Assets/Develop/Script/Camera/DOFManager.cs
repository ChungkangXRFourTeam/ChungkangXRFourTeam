using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOFManager : MonoBehaviour
{
    // Start is called before the first frame update

    Volume cameraVolume;
    VolumeProfile dofProfile;

    float focalValue = 50f;

    bool isBlured;

    void Start()
    {
        cameraVolume = GetComponent<Volume>();
        isBlured = true;

        dofProfile = cameraVolume.sharedProfile;

        if (!dofProfile.TryGet<DepthOfField>(out var dof))
            dof = dofProfile.Add<DepthOfField>(false);

        dof.focalLength.value = focalValue;

        StartCoroutine(UpdateBlur(dof));
    }

    IEnumerator UpdateBlur(DepthOfField dof)
    {
        float dt = 0;
        while (true)
        {
            dt += Time.deltaTime;
            if (isBlured)
            {

                dof.focalLength.value += 2;
            }
            else
            {
                dof.focalLength.value -= 2;
            }

            if (dt > 3f)
            {
                dt = 0;
                isBlured = !isBlured;
            }

            yield return new WaitForSeconds(Time.deltaTime);


        }
    }
}
