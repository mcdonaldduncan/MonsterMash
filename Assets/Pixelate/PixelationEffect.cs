using UnityEngine;
using UnityEngine.Rendering;

[VolumeComponentMenu("Post-processing/PixelationEffect")]
public class PixelationEffect : VolumeComponent
{
    [Tooltip("Pixel Size")]
    public ClampedIntParameter pixelSize = new ClampedIntParameter(8, 1, 128);
}

