//Taken off unity docs and then modified for something and then taken from there and modified.

Shader"Custom/RenderNpc"
{
    SubShader
    {
        Pass
        {
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma instancing_options assumeuniformscaling procedural:Instance

#include "UnityCG.cginc"

struct v2f
{
	float4 pos : SV_POSITION;
};
            
            
uniform StructuredBuffer<float3> FinalNpcPos_bNpcSurf;

v2f vert(appdata_base v, uint instanceID : SV_InstanceID)
{
	v2f o;
                //float4 wpos = mul(_ObjectToWorld, v.vertex + float4(instanceID, 0, 0, 0));
	float4 wpos = v.vertex + float4(FinalNpcPos_bNpcSurf[instanceID], 0.0);
	o.pos = mul(UNITY_MATRIX_VP, wpos);
	
	return o;
}

float4 frag(v2f i) : SV_Target
{
	return float4(1.0, 1.0, 1.0, 1.0);

}
            ENDCG
        }
    }
}