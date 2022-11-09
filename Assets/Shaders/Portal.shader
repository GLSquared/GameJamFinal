Shader "Custom/Portal"
{
    SubShader
    {
        Tags {"Queue" = "Geometry-1"}
        ColorMask 0
        ZWrite off

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        }
    }
}
