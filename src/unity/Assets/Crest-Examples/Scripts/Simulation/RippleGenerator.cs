﻿using UnityEngine;

public class RippleGenerator : MonoBehaviour
{
    public bool _animate = true;
    public float _warmUp = 3f;
    public float _onTime = 0.2f;
    public float _period = 4f;

    MeshRenderer _mr;
    Material _mat;

    void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        if(_animate)
        {
            _mr.enabled = false;
        }
        _mat = _mr.material;
    }
    
    void Update()
    {
        if(_animate)
        {
            float t = Time.time;
            if (t < _warmUp)
                return;
            t -= _warmUp;
            t = Mathf.Repeat(t, _period);
            _mr.enabled = t < _onTime;
        }

        // which lod is this object in (roughly)?
        Rect thisRect = new Rect(new Vector2(transform.position.x, transform.position.z), Vector3.zero);
        int minLod = Crest.LodDataAnimatedWaves.SuggestDataLOD(thisRect);
        if (minLod == -1)
        {
            // outside all lods, nothing to update!
            return;
        }

        // how many active wave sims currently apply to this object - ideally this would eliminate sims that are too
        // low res, by providing a max grid size param
        int simsPresent, simsActive;
        Crest.LodDataDynamicWaves.CountWaveSims(minLod, out simsPresent, out simsActive);
        if (simsPresent == 0)
        {
            enabled = false;
            return;
        }

        if (simsActive > 0)
        {
            _mat.SetFloat("_SimCount", simsActive);
        }

        _mat.SetFloat("_SimDeltaTime", Mathf.Min(Crest.LodDataPersistent.MAX_SIM_DELTA_TIME, Time.deltaTime));
    }
}
