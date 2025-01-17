#ifndef _MONTECARLO_INC_
#define _MONTECARLO_INC_

#include "Math.cginc"

// [ Duff et al. 2017, "Building an Orthonormal Basis, Revisited" ]
float3x3 GetTangentBasis( float3 TangentZ )
{
	const float Sign = TangentZ.z >= 0 ? 1 : -1;
	const float a = -rcp( Sign + TangentZ.z );
	const float b = TangentZ.x * TangentZ.y * a;
	
	float3 TangentX = { 1 + Sign * a * Pow2( (half)TangentZ.x ), Sign * b, -Sign * TangentZ.x };
	float3 TangentY = { b,  Sign + a * Pow2( (half)TangentZ.y ), -TangentZ.y };

	return float3x3( TangentX, TangentY, TangentZ );
}

float3 TangentToWorld( float3 Vec, float3 TangentZ )
{
	return mul( Vec, GetTangentBasis( TangentZ ) );
}

float3 WorldToTangent(float3 Vec, float3 TangentZ)
{
	return mul(GetTangentBasis(TangentZ), Vec);
}

float2 Hammersley2d(uint idx, uint num)
{
	uint bits = idx;
	bits = (bits << 16u) | (bits >> 16u);
	bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
	bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
	bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
	bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
	const float radicalInverse_VdC = float(bits) * 2.3283064365386963e-10; // / 0x100000000

	return float2(float(idx) / float(num), radicalInverse_VdC);
}

float2 Hammersley( uint Index, uint NumSamples, uint2 Random )
{
	float E1 = frac( (float)Index / NumSamples + float( Random.x & 0xffff ) / (1<<16) );
	float E2 = float( ReverseBits32(Index) ^ Random.y ) * 2.3283064365386963e-10;
	return float2( E1, E2 );
}

float2 Hammersley16( uint Index, uint NumSamples, uint2 Random )
{
	float E1 = frac( (float)Index / NumSamples + float( Random.x ) * (1.0 / 65536.0) );
	float E2 = float( ( ReverseBits32(Index) >> 16 ) ^ Random.y ) * (1.0 / 65536.0);
	return float2( E1, E2 );
}

// http://extremelearning.com.au/a-simple-method-to-construct-isotropic-quasirandom-blue-noise-point-sequences/
float2 R2Sequence( uint Index )
{
	const float Phi = 1.324717957244746;
	const float2 a = float2( 1.0 / Phi, 1.0 / Pow2(Phi) );
	return frac( a * Index );
}

// R2 Jittered point set
// These seem to be garbage so use at your own risk. Jitter is not large enough for low sample counts. Larger jitter overlaps neighboring samples unevenly.
float2 JitteredR2( uint Index, uint NumSamples, float2 Jitter, float JitterAmount = 0.5 )
{
	const float Phi = 1.324717957244746;
	const float2 a = float2( 1.0 / Phi, 1.0 / Pow2(Phi) );
	const float d0 = 0.76;
	const float i0 = 0.7;

	return frac( a * Index + ( JitterAmount * 0.5 * d0 * sqrt(PI) * rsqrt( NumSamples ) ) * Jitter );
}

// R2 Jittered point sequence. Progressive
float2 JitteredR2( uint Index, float2 Jitter, float JitterAmount = 0.5 )
{
	const float Phi = 1.324717957244746;
	const float2 a = float2( 1.0 / Phi, 1.0 / Pow2(Phi) );
	const float d0 = 0.76;
	const float i0 = 0.7;

	return frac( a * Index + ( JitterAmount * 0.25 * d0 * sqrt(PI) * rsqrt( Index - i0 ) ) * Jitter );
}


///////

float2 UniformSampleDisk( float2 E )
{
	float Theta = 2 * PI * E.x;
	float Radius = sqrt( E.y );
	return Radius * float2( cos( Theta ), sin( Theta ) );
}

float2 UniformSampleDiskConcentric( float2 E )
{
	float2 p = 2 * E - 1;
	float Radius;
	float Phi;
	if( abs( p.x ) > abs( p.y ) )
	{
		Radius = p.x;
		Phi = (PI/4) * (p.y / p.x);
	}
	else
	{
		Radius = p.y;
		Phi = (PI/2) - (PI/4) * (p.x / p.y);
	}
	return float2( Radius * cos( Phi ), Radius * sin( Phi ) );
}

// based on the approximate equal area transform from
// http://marc-b-reynolds.github.io/math/2017/01/08/SquareDisc.html
float2 UniformSampleDiskConcentricApprox( float2 E )
{
	float2 sf = E * sqrt(2.0) - sqrt(0.5);	// map 0..1 to -sqrt(0.5)..sqrt(0.5)
	float2 sq = sf*sf;
	float root = sqrt(2.0*max(sq.x, sq.y) - min(sq.x, sq.y));
	if (sq.x > sq.y)
	{
		sf.x = sf.x > 0 ? root : -root;
	}
	else
	{
		sf.y = sf.y > 0 ? root : -root;
	}
	return sf;
}

float4 UniformSampleSphere( float2 E )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = 1 - 2 * E.y;
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;

	float PDF = 1.0 / (4 * PI);

	return float4( H, PDF );
}

float4 UniformSampleHemisphere( float2 E )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = E.y;
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;

	float PDF = 1.0 / (2 * PI);

	return float4( H, PDF );
}

float4 CosineSampleHemisphere( float2 E )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = sqrt( E.y );
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;

	float PDF = CosTheta * (1.0 /  PI);

	return float4( H, PDF );
}

float4 CosineSampleHemisphere( float2 E, float3 N ) 
{
	float3 H = UniformSampleSphere( E ).xyz;
	H = normalize( N + H );

	float PDF = dot(H, N) * (1.0 /  PI);

	return float4( H, PDF );
}

float4 UniformSampleCone( float2 E, float CosThetaMax )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = lerp( CosThetaMax, 1, E.y );
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 L;
	L.x = SinTheta * cos( Phi );
	L.y = SinTheta * sin( Phi );
	L.z = CosTheta;

	float PDF = 1.0 / ( 2 * PI * (1 - CosThetaMax) );

	return float4( L, PDF );
}

float4 ImportanceSampleBlinn( float2 E, float a2 )
{
	float n = 2 / a2 - 2;

	float Phi = 2 * PI * E.x;
	float CosTheta = ClampedPow( E.y, 1 / (n + 1) );
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;

	float D = (n+2) / (2*PI) * ClampedPow( CosTheta, n );
	float PDF = D * CosTheta;

	return float4( H, PDF );
}

float4 ImportanceSampleGGX( float2 E, float a2 )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = sqrt( (1 - E.y) / ( 1 + (a2 - 1) * E.y ) );
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;
	
	float d = ( CosTheta * a2 - CosTheta ) * CosTheta + 1;
	float D = a2 / ( PI*d*d );
	float PDF = D * CosTheta;

	return float4( H, PDF );
}

// [ Heitz 2018, "Sampling the GGX Distribution of Visible Normals" ]
// http://jcgt.org/published/0007/04/01/

float4 ImportanceSampleVisibleGGX( float2 DiskE, float a2, float3 V )
{
	// TODO float2 alpha for anisotropic
	float a = sqrt(a2);

	// stretch
	float3 Vh = normalize( float3( a * V.xy, V.z ) );

	// Orthonormal basis
	// Tangent0 is orthogonal to N.
	#if 1 // Stable tangent basis based on V.
		float3 Tangent0 = (V.z < 0.9999) ? normalize( cross( float3(0, 0, 1), V ) ) : float3(1, 0, 0);
		float3 Tangent1 = normalize(cross( Vh, Tangent0 ));
	#else
		float3 Tangent0 = (Vh.z < 0.9999) ? normalize( cross( float3(0, 0, 1), Vh ) ) : float3(1, 0, 0);
		float3 Tangent1 = cross( Vh, Tangent0 );
	#endif

	float2 p = DiskE;
	float s = 0.5 + 0.5 * Vh.z;
	p.y = (1 - s) * sqrt( 1 - p.x * p.x ) + s * p.y;

	float3 H;
	H  = p.x * Tangent0;
	H += p.y * Tangent1;
	H += sqrt( saturate( 1 - dot( p, p ) ) ) * Vh;

	// unstretch
	H = normalize( float3( a * H.xy, max(0.0, H.z) ) );

	float NoV = V.z;
	float NoH = H.z;
	float VoH = dot(V, H);

	float d = (NoH * a2 - NoH) * NoH + 1;
	float D = a2 / (PI*d*d);

	float G_SmithV = 2 * NoV / (NoV + sqrt(NoV * (NoV - NoV * a2) + a2));

	float PDF = G_SmithV * VoH * D / NoV;

	return float4(H, PDF);
}

// Multiple importance sampling power heuristic of two functions with a power of two. 
// [Veach 1997, "Robust Monte Carlo Methods for Light Transport Simulation"]
float MISWeight( uint Num, float PDF, uint OtherNum, float OtherPDF )
{
	float Weight = Num * PDF;
	float OtherWeight = OtherNum * OtherPDF;
	return Weight * Weight / (Weight * Weight + OtherWeight * OtherWeight);
}

#endif