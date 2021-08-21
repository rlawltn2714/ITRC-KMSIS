using System;
using TriLibCore.General;
using TriLibCore.Interfaces;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.URP.Mappers
{
    /// <summary>Represents a Material Mapper that converts TriLib Materials into Unity UniversalRP Materials.</summary>
    [Serializable]
    [CreateAssetMenu(menuName = "TriLib/Mappers/Material/Universal RP Material Mapper", fileName = "UniversalRPMaterialMapper")]
    public class UniversalRPMaterialMapper : MaterialMapper
    {
        #region Standard
        public override Material MaterialPreset => Resources.Load<Material>("Materials/UniversalRP/Standard/TriLibUniversalRP");

        public override Material CutoutMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/Standard/TriLibUniversalRPAlphaCutout");

        public override Material TransparentMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/Standard/TriLibUniversalRPAlpha");

        public override Material TransparentComposeMaterialPreset => Resources.Load<Material>("Materials/UniversalRP/Standard/TriLibUniversalRPAlpha");
        #endregion

        public override Material LoadingMaterial => Resources.Load<Material>("Materials/UniversalRP/TriLibUniversalRPLoading");


        public override bool IsCompatible(MaterialMapperContext materialMapperContext)
        {
            return TriLibSettings.GetBool("UniversalRPMaterialMapper");
        }


        public override void Map(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.VirtualMaterial = new VirtualMaterial();

            CheckTransparencyMapTexture(materialMapperContext);
            CheckSpecularMapTexture(materialMapperContext);
            CheckDiffuseColor(materialMapperContext);
            CheckDiffuseMapTexture(materialMapperContext);
            CheckNormalMapTexture(materialMapperContext);
            CheckEmissionColor(materialMapperContext);
            CheckEmissionMapTexture(materialMapperContext);
            CheckOcclusionMapTexture(materialMapperContext);
            CheckGlossinessValue(materialMapperContext);
            CheckGlossinessMapTexture(materialMapperContext);
            CheckMetallicValue(materialMapperContext);
            CheckMetallicGlossMapTexture(materialMapperContext);
            Dispatcher.InvokeAsyncAndWait(BuildMaterial, materialMapperContext);
        }

        private void CheckDiffuseMapTexture(MaterialMapperContext materialMapperContext)
        {
            var diffuseTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.DiffuseMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(diffuseTexturePropertyName);
            var texture = LoadTexture(materialMapperContext, TextureType.Diffuse, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
            ApplyDiffuseMapTexture(materialMapperContext, TextureType.Diffuse, texture, textureValue);
        }

        private void ApplyDiffuseMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture, ITexture triLibTexture)
        {
            if (texture != null)
            {
                materialMapperContext.Context.UsedTextures.Add(texture);
            }
            materialMapperContext.VirtualMaterial.SetProperty(GetDiffuseTextureName(materialMapperContext), texture);
            if (triLibTexture != null && materialMapperContext.Context.Options.ApplyTexturesOffsetAndScaling)
            {
                materialMapperContext.VirtualMaterial.Tiling = triLibTexture.Tiling; 
                materialMapperContext.VirtualMaterial.Offset = triLibTexture.Offset;
            }
        }
        
        private void CheckGlossinessValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericFloatValueMultiplied(GenericMaterialProperty.Glossiness);
            materialMapperContext.VirtualMaterial.SetProperty("_Smoothness", value);
        }

        private void CheckMetallicValue(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericFloatValueMultiplied(GenericMaterialProperty.Metallic);
            materialMapperContext.VirtualMaterial.SetProperty("_Metallic", value);
        }

        private void CheckEmissionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var emissionTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.EmissionMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(emissionTexturePropertyName);
            var texture = LoadTexture(materialMapperContext, TextureType.Emission, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
            ApplyEmissionMapTexture(materialMapperContext, TextureType.Emission, texture);
        }

        private void ApplyEmissionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            materialMapperContext.VirtualMaterial.SetProperty("_EmissionMap", texture);
            if (texture)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }

        private void CheckNormalMapTexture(MaterialMapperContext materialMapperContext)
        {
            var normalMapTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.NormalMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(normalMapTexturePropertyName);
            var texture = LoadTexture(materialMapperContext, TextureType.NormalMap, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
            ApplyNormalMapTexture(materialMapperContext, TextureType.NormalMap, texture);
        }

        private void ApplyNormalMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            if (texture != null)
            {
                materialMapperContext.Context.UsedTextures.Add(texture);
            }
            materialMapperContext.VirtualMaterial.SetProperty("_BumpMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.SetProperty("_NormalScale", 1f);
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP");
            }
        }

        private void CheckTransparencyMapTexture(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.VirtualMaterial.HasAlpha |= materialMapperContext.Material.UsesAlpha;
            var transparencyTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.TransparencyMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(transparencyTexturePropertyName);
            var texture = LoadTexture(materialMapperContext, TextureType.Transparency, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
        }

        private void CheckSpecularMapTexture(MaterialMapperContext materialMapperContext)
        {
            var specularTexturePropertyName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.SpecularMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(specularTexturePropertyName);
            var texture = LoadTexture(materialMapperContext, TextureType.Specular, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
        }

        private void CheckOcclusionMapTexture(MaterialMapperContext materialMapperContext)
        {
            var occlusionMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.OcclusionMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(occlusionMapTextureName);
            var texture = LoadTexture(materialMapperContext, TextureType.Occlusion, textureValue, out var textureLoaded); 
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
            ApplyOcclusionMapTexture(materialMapperContext, TextureType.Occlusion, texture);
        }

        private void ApplyOcclusionMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            if (texture != null)
            {
                materialMapperContext.Context.UsedTextures.Add(texture);
            }
            materialMapperContext.VirtualMaterial.SetProperty("_OcclusionMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_OCCLUSIONMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_OCCLUSIONMAP");
            }
        }

        private void CheckGlossinessMapTexture(MaterialMapperContext materialMapperContext)
        {
            var auxiliaryMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.GlossinessOrRoughnessMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(auxiliaryMapTextureName);
            var texture = LoadTexture(materialMapperContext, TextureType.GlossinessOrRoughness, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
        }

        private void CheckMetallicGlossMapTexture(MaterialMapperContext materialMapperContext)
        {
            var metallicGlossMapTextureName = materialMapperContext.Material.GetGenericPropertyName(GenericMaterialProperty.MetallicMap);
            var textureValue = materialMapperContext.Material.GetTextureValue(metallicGlossMapTextureName);
            var texture = LoadTexture(materialMapperContext, TextureType.Metalness, textureValue, out var textureLoaded);
            CheckTextureOffsetAndScaling(materialMapperContext, textureValue, textureLoaded);
            ApplyMetallicGlossMapTexture(materialMapperContext, TextureType.Metalness, texture);
        }

        private void ApplyMetallicGlossMapTexture(MaterialMapperContext materialMapperContext, TextureType textureType, Texture texture)
        {
            if (texture != null)
            {
                materialMapperContext.Context.UsedTextures.Add(texture);
            }
            materialMapperContext.VirtualMaterial.SetProperty("_MetallicGlossMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_METALLICGLOSSMAP");
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_METALLICGLOSSMAP");
            }
        }

        private void CheckEmissionColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValueMultiplied(GenericMaterialProperty.EmissionColor);
            materialMapperContext.VirtualMaterial.SetProperty("_EmissionColor", value);
            if (value != Color.black)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                materialMapperContext.VirtualMaterial.SetProperty("_EmissiveIntensity", 1f);
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSION");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }

        private void CheckDiffuseColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValueMultiplied(GenericMaterialProperty.DiffuseColor);
            value.a *= materialMapperContext.Material.GetGenericFloatValueMultiplied(GenericMaterialProperty.AlphaValue);
            materialMapperContext.VirtualMaterial.HasAlpha |= value.a < 1f;
            materialMapperContext.VirtualMaterial.SetProperty("_BaseColor", value);
        }
		
        public override string GetDiffuseTextureName(MaterialMapperContext materialMapperContext) 
		{
           return "_BaseMap";
		}

    }
}
