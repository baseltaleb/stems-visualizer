using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSpectrum : OutputVolume
{
    public bool useSkyBox;
    public Material material;
    public string materialValueId;
    public float colorMultiplier;

    public override void OnUpdate()
    {
        base.OnUpdate();


            float newColorVal = colorCurve.Evaluate(outputValue) * colorMultiplier;
            if (newColorVal > oldColorVal) //it's attacking
            {
                if (colorAttackDamp != 1)
                {
                    newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorAttackDamp);
                }
            }
            else //it's decaying
            {
                if (colorDecayDamp != 1)
                {
                    newColorVal = Mathf.Lerp(oldColorVal, newColorVal, colorDecayDamp);
                }
            }
            if (useSkyBox)
            {
                var color = Color.Lerp(MinColor, MaxColor, newColorVal);
                RenderSettings.skybox.SetColor(materialValueId, color);
            }
            else 
                material.SetFloat(materialValueId, newColorVal);
            oldColorVal = newColorVal;
        
    }
}
