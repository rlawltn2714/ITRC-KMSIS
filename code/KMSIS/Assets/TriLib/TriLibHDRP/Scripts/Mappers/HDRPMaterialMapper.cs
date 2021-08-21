using System;
using TriLibCore.General;
using TriLibCore.Interfaces;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace TriLibCore.HDRP.Mappers
{
    /// <summary>Represents a Material Mapper that converts TriLib Materials into Unity HDRP Materials.</summary>
    [Serializable]
    [CreateAssetMenu(menuName = "TriLib/Mappers/Material/HDRP Material Mapper", fileName = "HDRPMaterialMapper")]
    public class HDRPMaterialMapper : MaterialMapper
    {
        #region Standard
        public override Material MaterialPreset => Resources.Load<Material>("Materials/HDRP/Standard/TriLibHDRP");

        public override Material CutoutMaterialPreset => Resources.Load<Material>("Materials/HDRP/Standard/TriLibHDRPAlphaCutout");

        public override Material TransparentMaterialPreset => Resources.Load<Material>("Materials/HDRP/Standard/TriLibHDRPAlpha");

        public override Material TransparentComposeMaterialPreset => Resources.Load<Material>("Materials/HDRP/Standard/TriLibHDRPAlpha");
        #endregion

        public override Material LoadingMaterial => Resources.Load<Material>("Materials/HDRP/TriLibHDRPLoading");


        public override bool IsCompatible(MaterialMapperContext materialMapperContext)
        {
            return TriLibSettings.GetBool("HDRPMaterialMapper");
        }


        public override void Map(MaterialMapperContext materialMapperContext)
        {
            materialMapperContext.VirtualMaterial = new HDRPVirtualMaterial();

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
            Dispatcher.InvokeAsyncAndWait(BuildHDRPMask, materialMapperContext);
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
            if (texture != null)
            {
                materialMapperContext.Context.UsedTextures.Add(texture);
            }
            materialMapperContext.VirtualMaterial.SetProperty("_EmissiveColorMap", texture);
            if (texture)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_EMISSIVE_COLOR_MAP");
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                materialMapperContext.VirtualMaterial.SetProperty("_EmissiveIntensity", 1f);
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_EMISSIVE_COLOR_MAP");
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
            materialMapperContext.VirtualMaterial.SetProperty("_NormalMap", texture);
            if (texture != null)
            {
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
                materialMapperContext.VirtualMaterial.SetProperty("_NormalScale", 1f);
            }
            else
            {
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP");
                materialMapperContext.VirtualMaterial.DisableKeyword("_NORMALMAP_TANGENT_SPACE");
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
            ((HDRPVirtualMaterial)materialMapperContext.VirtualMaterial).OcclusionTexture = texture;
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
            ((HDRPVirtualMaterial)materialMapperContext.VirtualMaterial).MetallicTexture = texture;
        }

        private void CheckEmissionColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValueMultiplied(GenericMaterialProperty.EmissionColor);
            materialMapperContext.VirtualMaterial.SetProperty("_EmissiveColor", value);
            materialMapperContext.VirtualMaterial.SetProperty("_EmissiveColorLDR", value);
            if (value != Color.black)
            {
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                materialMapperContext.VirtualMaterial.SetProperty("_EmissiveIntensity", 1f);
            }
            else
            {
                materialMapperContext.VirtualMaterial.GlobalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }

        private void CheckDiffuseColor(MaterialMapperContext materialMapperContext)
        {
            var value = materialMapperContext.Material.GetGenericColorValueMultiplied(GenericMaterialProperty.DiffuseColor);
            value.a *= materialMapperContext.Material.GetGenericFloatValueMultiplied(GenericMaterialProperty.AlphaValue);
            materialMapperContext.VirtualMaterial.HasAlpha |= value.a < 1f;
            materialMapperContext.VirtualMaterial.SetProperty("_BaseColor", value);
            materialMapperContext.VirtualMaterial.SetProperty("_Color", value);
        }

        private void BuildHDRPMask(MaterialMapperContext materialMapperContext)
        {
            if (materialMapperContext.UnityMaterial == null)
            {
                return;
            }
            var hdrpVirtualMaterial = (HDRPVirtualMaterial)materialMapperContext.VirtualMaterial;
            var maskBaseTexture = hdrpVirtualMaterial.MetallicTexture ?? hdrpVirtualMaterial.OcclusionTexture ?? hdrpVirtualMaterial.DetailMaskTexture;
            if (maskBaseTexture == null)
            {
                if (materialMapperContext.Context.Options.UseMaterialKeywords)
                {
                    materialMapperContext.UnityMaterial.DisableKeyword("_MASKMAP");
                }
                return;
            }
            var graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
            var renderTexture = new RenderTexture(maskBaseTexture.width, maskBaseTexture.height, 0, graphicsFormat);
            renderTexture.name = $"{(string.IsNullOrWhiteSpace(maskBaseTexture.name) ? "Unnamed" : maskBaseTexture.name)}_Mask";
            renderTexture.useMipMap = TextureUtils.CanGenerateMips(renderTexture, materialMapperContext.Context);
            renderTexture.autoGenerateMips = false;
            var material = new Material(Shader.Find("Hidden/TriLib/BuildHDRPMask"));
            if (hdrpVirtualMaterial.MetallicTexture != null)
            {
                material.SetTexture("_MetallicTex", hdrpVirtualMaterial.MetallicTexture);
            }
            if (hdrpVirtualMaterial.OcclusionTexture != null)
            {
                material.SetTexture("_OcclusionTex", hdrpVirtualMaterial.OcclusionTexture);
            }
            if (hdrpVirtualMaterial.DetailMaskTexture != null)
            {
                material.SetTexture("_DetailMaskTex", hdrpVirtualMaterial.DetailMaskTexture);
            }
            Graphics.Blit(null, renderTexture, material);
            if (renderTexture.useMipMap)
            {
                renderTexture.GenerateMips();
            }
            if (materialMapperContext.Context.Options.UseMaterialKeywords)
            {
                materialMapperContext.UnityMaterial.EnableKeyword("_MASKMAP");
            }
            materialMapperContext.UnityMaterial.SetTexture("_MaskMap", renderTexture);
            materialMapperContext.VirtualMaterial.TextureProperties.Add("_MaskMap", renderTexture);
            if (Application.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
        }

        public override string GetDiffuseTextureName(MaterialMapperContext materialMapperContext)
        {
            return "_BaseColorMap";
        }
    }
}