#pragma once

#include "../NxGpuDevice.h"

#pragma warning(push)
#pragma warning(disable:4005)
#include <d3d11.h>
#include <d3d11_1.h>
#include <d3d11_4.h>
//#include <d3dx11.h>
#include <d3dcompiler.h>
#pragma warning(pop)

NS_BEGIN

namespace NxRHI
{
	inline DXGI_FORMAT FormatToDXFormat(EPixelFormat pixel_fmt)
	{
		switch (pixel_fmt)
		{
		case PXF_UNKNOWN:
			return DXGI_FORMAT_UNKNOWN;
		case PXF_R16_FLOAT:
			return DXGI_FORMAT_R16_FLOAT;
		case PXF_R16_UINT:
			return DXGI_FORMAT_R16_UINT;
		case PXF_R16_SINT:
			return DXGI_FORMAT_R16_SINT;
		case PXF_R16_UNORM:
			return DXGI_FORMAT_R16_UNORM;
		case PXF_R16_SNORM:
			return DXGI_FORMAT_R16_SNORM;
		case PXF_R32_UINT:
			return DXGI_FORMAT_R32_UINT;
		case PXF_R32_SINT:
			return DXGI_FORMAT_R32_SINT;
		case PXF_R32_FLOAT:
			return DXGI_FORMAT_R32_FLOAT;
		case PXF_R8G8B8A8_SINT:
			return DXGI_FORMAT_R8G8B8A8_SINT;
		case PXF_R8G8B8A8_UINT:
			return DXGI_FORMAT_R8G8B8A8_UINT;
		case PXF_R8G8B8A8_UNORM:
			return DXGI_FORMAT_R8G8B8A8_UNORM;
		case PXF_R8G8B8A8_SNORM:
			return DXGI_FORMAT_R8G8B8A8_SNORM;
		case PXF_R16G16_UINT:
			return DXGI_FORMAT_R16G16_UINT;
		case PXF_R16G16_SINT:
			return DXGI_FORMAT_R16G16_SINT;
		case PXF_R16G16_FLOAT:
			return DXGI_FORMAT_R16G16_FLOAT;
		case PXF_R16G16_UNORM:
			return DXGI_FORMAT_R16G16_UNORM;
		case PXF_R16G16_SNORM:
			return DXGI_FORMAT_R16G16_SNORM;
		case PXF_R16G16B16A16_UINT:
			return DXGI_FORMAT_R16G16B16A16_UINT;
		case PXF_R16G16B16A16_SINT:
			return DXGI_FORMAT_R16G16B16A16_SINT;
		case PXF_R16G16B16A16_FLOAT:
			return DXGI_FORMAT_R16G16B16A16_FLOAT;
		case PXF_R16G16B16A16_UNORM:
			return DXGI_FORMAT_R16G16B16A16_UNORM;
		case PXF_R16G16B16A16_SNORM:
			return DXGI_FORMAT_R16G16B16A16_SNORM;
		case PXF_R32G32B32A32_UINT:
			return DXGI_FORMAT_R32G32B32A32_UINT;
		case PXF_R32G32B32A32_SINT:
			return DXGI_FORMAT_R32G32B32A32_SINT;
		case PXF_R32G32B32A32_FLOAT:
			return DXGI_FORMAT_R32G32B32A32_FLOAT;
		case PXF_R32G32B32_UINT:
			return DXGI_FORMAT_R32G32B32_UINT;
		case PXF_R32G32B32_SINT:
			return DXGI_FORMAT_R32G32B32_SINT;
		case PXF_R32G32B32_FLOAT:
			return DXGI_FORMAT_R32G32B32_FLOAT;
		case PXF_R32G32_UINT:
			return DXGI_FORMAT_R32G32_UINT;
		case PXF_R32G32_SINT:
			return DXGI_FORMAT_R32G32_SINT;
		case PXF_R32G32_FLOAT:
			return DXGI_FORMAT_R32G32_FLOAT;
		case PXF_D24_UNORM_S8_UINT:
			return DXGI_FORMAT_D24_UNORM_S8_UINT;
		case PXF_D32_FLOAT:
			return DXGI_FORMAT_D32_FLOAT;
		case PXF_D32_FLOAT_S8X24_UINT:
			return DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
		case PXF_D16_UNORM:
			return DXGI_FORMAT_D16_UNORM;
		case PXF_B8G8R8A8_UNORM:
			return DXGI_FORMAT_B8G8R8A8_UNORM;
		case PXF_R11G11B10_FLOAT:
			return DXGI_FORMAT_R11G11B10_FLOAT;
		case PXF_R8G8_UNORM:
			return DXGI_FORMAT_R8G8_UNORM;
		case PXF_R8_UNORM:
			return DXGI_FORMAT_R8_UNORM;
		case PXF_R32_TYPELESS:
			return DXGI_FORMAT_R32_TYPELESS;
		case PXF_R32G32B32A32_TYPELESS:
			return DXGI_FORMAT_R32G32B32A32_TYPELESS;
		case PXF_R32G32B32_TYPELESS:
			return DXGI_FORMAT_R32G32B32_TYPELESS;
		case PXF_R16G16B16A16_TYPELESS:
			return DXGI_FORMAT_R16G16B16A16_TYPELESS;
		case PXF_R32G32_TYPELESS:
			return DXGI_FORMAT_R32G32_TYPELESS;
		case PXF_R32G8X24_TYPELESS:
			return DXGI_FORMAT_R32G8X24_TYPELESS;
		case PXF_R10G10B10A2_TYPELESS:
			return DXGI_FORMAT_R10G10B10A2_TYPELESS;
		case PXF_R10G10B10A2_UNORM:
			return DXGI_FORMAT_R10G10B10A2_UNORM;
		case PXF_R10G10B10A2_UNORM_SRGB:
			break;
		case PXF_R10G10B10A2_SNORM:
			break;
		case PXF_R10G10B10A2_UINT:
			return DXGI_FORMAT_R10G10B10A2_UINT;
		case PXF_R10G10B10A2_SINT:
			break;
		case PXF_B10G10R10A2_TYPELESS:
			break;
		case PXF_B10G10R10A2_UNORM:
			break;
		case PXF_B10G10R10A2_UNORM_SRGB:
			break;
		case PXF_B10G10R10A2_SNORM:
			break;
		case PXF_B10G10R10A2_UINT:
			break;
		case PXF_B10G10R10A2_SINT:
			break;
		case PXF_R8G8B8A8_TYPELESS:
			return DXGI_FORMAT_R8G8B8A8_TYPELESS;
		case PXF_R8G8B8A8_UNORM_SRGB:
			return DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
		case PXF_R16G16_TYPELESS:
			return DXGI_FORMAT_R16G16_TYPELESS;
		case PXF_R24G8_TYPELESS:
			return DXGI_FORMAT_R24G8_TYPELESS;
		case PXF_R24_UNORM_X8_TYPELESS:
			return DXGI_FORMAT_R24_UNORM_X8_TYPELESS;
		case PXF_X24_TYPELESS_G8_UINT:
			return DXGI_FORMAT_X24_TYPELESS_G8_UINT;
		case PXF_R8G8_TYPELESS:
			return DXGI_FORMAT_R8G8_TYPELESS;
		case PXF_R8G8_UINT:
			return DXGI_FORMAT_R8G8_UINT;
		case PXF_R8G8_SNORM:
			return DXGI_FORMAT_R8G8_SNORM;
		case PXF_R8G8_SINT:
			return DXGI_FORMAT_R8G8_SINT;
		case PXF_R16_TYPELESS:
			return DXGI_FORMAT_R16_TYPELESS;
		case PXF_R8_TYPELESS:
			return DXGI_FORMAT_R8_TYPELESS;
		case PXF_R8_UINT:
			return DXGI_FORMAT_R8_UINT;
		case PXF_R8_SNORM:
			return DXGI_FORMAT_R8_SNORM;
		case PXF_R8_SINT:
			return DXGI_FORMAT_R8_SINT;
		case PXF_A8_UNORM:
			return DXGI_FORMAT_A8_UNORM;
		case PXF_B8G8R8X8_UNORM:
			return DXGI_FORMAT_B8G8R8X8_UNORM;
		case PXF_B8G8R8A8_TYPELESS:
			return DXGI_FORMAT_B8G8R8A8_TYPELESS;
		case PXF_B8G8R8A8_UNORM_SRGB:
			return DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
		case PXF_B8G8R8A8_SNORM:
			break;
		case PXF_B8G8R8A8_UINT:
			break;
		case PXF_B8G8R8A8_SINT:
			break;
		case PXF_B8G8R8X8_TYPELESS:
			return DXGI_FORMAT_B8G8R8X8_TYPELESS;
		case PXF_B8G8R8X8_UNORM_SRGB:
			return DXGI_FORMAT_B8G8R8X8_UNORM_SRGB;
		case PXF_B5G6R5_UNORM:
			return DXGI_FORMAT_B5G6R5_UNORM;
		case PXF_B4G4R4A4_UNORM:
			return DXGI_FORMAT_B4G4R4A4_UNORM;
		default:
			break;
		}
		return DXGI_FORMAT_UNKNOWN;
	}
}

NS_END