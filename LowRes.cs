using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LowRes : VolumeComponent, IPostProcessComponent
{
    public BoolParameter on = new BoolParameter(false);

    public IntParameter height = new IntParameter(180);

    public bool IsActive() => (bool)on;

    public bool IsTileCompatible() => false;
}
