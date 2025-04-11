Shader "Masked/DepthMask"
{
    SubShader
    {
        // Render the mask after regular geometry, but before water and transparency
        Tags { "Queue" = "Geometry+10" }

        // Don't write color, only depth
        ColorMask 0
        ZWrite On
        ZTest LEqual

        Pass {}
    }
}
