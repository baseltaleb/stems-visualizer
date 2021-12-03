using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolesSpectrum : SimpleSpectrum
{

    public Vector3 moveUnitsPerSecond = new Vector3(1, 0, 0);
    public Vector3 trackStartPoint;
    public Vector3 trackEndpoint;
    public override void OnUpdate()
    {
        base.OnUpdate();

        transform.position += moveUnitsPerSecond * Time.deltaTime;
        //var mod = Mathf.Clamp(transform.position.x % moveUnitsPerSecond.x, 0.1f, 1f);
        var mod = Mathf.Repeat(transform.position.x, 3);
        Debug.Log(mod + " " + (mod == 0f));
        //List<Transform> _bars = new(bars);
        //for (int i = 0; i < barAmount; i++)
        //{
        //    if(_bars[i].position.x > trackEndpoint.x)
        //    {
        //        DestroyImmediate(_bars[i]);
        //        _bars.Insert(0, )
        //    }
        //}
    }
}
