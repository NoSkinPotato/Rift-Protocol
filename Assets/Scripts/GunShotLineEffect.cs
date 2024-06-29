using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShotLineEffect : MonoBehaviour
{
    public float animationDuration = 0.1f;
    public void gunShot (Vector3 startpoint, Vector3 endpoint)
    {

        LineRenderer lr = getLineRenderer();

        if(lr != null)
        {

            StartCoroutine(shot(lr, startpoint, endpoint));
        }

    }

    private LineRenderer getLineRenderer()
    {
        LineRenderer[] allLineRenderers = GetComponentsInChildren<LineRenderer>();
        foreach(LineRenderer lineRenderer in allLineRenderers)
        {
            if(lineRenderer.enabled == false) {

                return lineRenderer;
            }
        }
        return null;
    }

    private IEnumerator shot(LineRenderer lr, Vector3 startpoint, Vector3 endpoint)
    {
        float startTime = Time.time;
        lr.enabled = true;
        lr.SetPosition(0, startpoint);
        lr.SetPosition(1, endpoint);

        
        while(Time.time < startTime + animationDuration)
        {
            yield return null;
        }



        lr.enabled = false;

    }
}
