using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SpectrumColorHandler
{
    public void UpdateColor(SimpleSpectrum spectrum, int barIndex)
    {
        Color color;
        if (spectrum.useColorGradient)
        {
            var value = spectrum.spectrumOutputData[barIndex];
            color = Color.Lerp(spectrum.colorMin, spectrum.colorMax, value);
        }
        else
        {
            color = spectrum.colorMin;
        }
        HDMaterial.SetEmissiveColor(spectrum.barMaterials[barIndex], color);
    }
}
