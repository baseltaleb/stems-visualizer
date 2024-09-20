using UnityEngine;

public class SegmentTriggerDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SongEvents.TriggerSegementEnter(SegmentLabels.INTRO);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SongEvents.TriggerSegementEnter(SegmentLabels.VERSE);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SongEvents.TriggerSegementEnter(SegmentLabels.CHORUS);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SongEvents.TriggerSegementEnter(SegmentLabels.BRIDGE);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SongEvents.TriggerSegementEnter(SegmentLabels.SOLO);
        }
    }
}
