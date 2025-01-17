#ifndef _SSM_
#define _SSM_

#include "../../Inc/VertexLayout.cginc"
#include "../../Inc/LightCommon.cginc"
#include "../../Inc/Math.cginc"

#include "Material"
#include "MdfQueue"

#include "../../Inc/SysFunctionDefImpl.cginc"

/**Meta Begin:(VS_Main)
HLSL=none
Meta End:(VS_Main)**/
PS_INPUT VS_Main(VS_INPUT input1)
{
	VS_MODIFIER input = VS_INPUT_TO_VS_MODIFIER(input1);
	PS_INPUT output = (PS_INPUT)0;
	Default_VSInput2PSInput(output, input);

	MTL_OUTPUT mtl = (MTL_OUTPUT)0;
	//mtl template stuff;
	{
#ifdef MDFQUEUE_FUNCTION
		MdfQueueDoModifiers(output, input);
#endif

#ifndef DO_VS_MATERIAL
		DO_VS_MATERIAL(output, mtl);
#endif
	}

#if !defined(VS_NO_WorldTransform)
	output.vPosition.xyz += mtl.mVertexOffset;
	
	matrix ShadowWVPMtx = mul(WorldMatrix, GetViewPrjMtx(false));
	output.vPosition = mul(float4(output.vPosition.xyz, 1), ShadowWVPMtx);
#else
	matrix ShadowWVPMtx = GetViewPrjMtx(false);
	output.vPosition = mul(float4(output.vPosition.xyz, 1), ShadowWVPMtx);
#endif
	
	output.vPosition.z = output.vPosition.z + gDepthBiasAndZFarRcp.x;

	return output;
}

struct PS_OUTPUT
{
	float4 RT0 : SV_Target0;
};

/**Meta Begin:(PS_Main)
HLSL=none
Meta End:(PS_Main)**/
PS_OUTPUT PS_Main(PS_INPUT input)
{
	PS_OUTPUT output = (PS_OUTPUT)0;

	MTL_OUTPUT mtl = Default_PSInput2Material(input);
	//mtl template stuff;
	{
#ifndef DO_PS_MATERIAL
#define DO_PS_MATERIAL DoDefaultPSMaterial
#endif
		DO_PS_MATERIAL(input, mtl);
	}

#ifdef ALPHA_TEST
	//half Alpha = mtl.mAlpha;
	//half AlphaTestThreshold = mtl.mAlphaTest;
	clip(mtl.mAlpha - mtl.mAlphaTest);
#endif 

	output.RT0.rgb = mtl.mAlbedo;
	output.RT0.a = mtl.mAlpha;
	
	return output;
}

#endif//#ifndef _SSM_