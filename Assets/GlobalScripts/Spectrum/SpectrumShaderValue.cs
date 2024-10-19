using UnityEngine;

public class SpectrumShaderValue : MonoBehaviour
{
    public OutputVolume outputVolume;
    public string shaderValueReference = "_Amblitude";
    public float multiplier = 1;
    Material material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        outputVolume = GetComponent<OutputVolume>();
    }

    void Update()
    {
        material.SetFloat(shaderValueReference, outputVolume.outputValue * multiplier);
    }
}
