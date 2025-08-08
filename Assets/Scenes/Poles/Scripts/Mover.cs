using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 moveUnitsPerSecond = new Vector3(1, 0, 0);
    public bool animatePosition;
    public bool animateUvOffset;

    Vector2 curOffset = new Vector2(0, 0);
    void Update()
    {
        if (animatePosition)
            transform.position += moveUnitsPerSecond * Time.deltaTime;
        if (animateUvOffset)
        {
            curOffset += new Vector2(moveUnitsPerSecond.x * Time.deltaTime, moveUnitsPerSecond.y * Time.deltaTime);
            GetComponent<Renderer>().material.SetTextureOffset("_BaseColorMap", curOffset);

        }
    }
}
