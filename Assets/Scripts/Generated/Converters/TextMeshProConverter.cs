using System;
using System.Collections;
using System.Collections.Generic;

using Bayat.Json.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

namespace Bayat.Json.Converters
{

    public class TextMeshProConverter : ObjectJsonConverter
    {

        public override string[] GetObjectProperties()
        {
            return new string[] { "sortingLayerID", "sortingOrder", "autoSizeTextContainer", "maskType", "text", "isRightToLeftText", "font", "fontSharedMaterial", "fontSharedMaterials", "fontMaterial", "fontMaterials", "color", "alpha", "enableVertexGradient", "colorGradient", "colorGradientPreset", "spriteAsset", "tintAllSprites", "overrideColorTags", "faceColor", "outlineColor", "outlineWidth", "fontSize", "fontWeight", "enableAutoSizing", "fontSizeMin", "fontSizeMax", "fontStyle", "alignment", "characterSpacing", "wordSpacing", "lineSpacing", "lineSpacingAdjustment", "paragraphSpacing", "characterWidthAdjustment", "enableWordWrapping", "wordWrappingRatios", "overflowMode", "linkedTextComponent", "isLinkedTextComponent", "enableKerning", "extraPadding", "richText", "parseCtrlCharacters", "isOverlay", "isOrthographic", "enableCulling", "ignoreRectMaskCulling", "ignoreVisibility", "horizontalMapping", "verticalMapping", "mappingUvLineOffset", "renderMode", "geometrySortingOrder", "vertexBufferAutoSizeReduction", "firstVisibleCharacter", "maxVisibleCharacters", "maxVisibleWords", "maxVisibleLines", "useMaxVisibleDescender", "pageToDisplay", "margin", "havePropertiesChanged", "isUsingLegacyAnimationComponent", "isVolumetricText", "onCullStateChanged", "maskable", "isMaskingGraphic", "raycastTarget", "material", "useGUILayout", "runInEditMode", "enabled", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TMPro.TMP_Text).IsAssignableFrom( objectType);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (TMPro.TMP_Text)value;
            // writer.WriteProperty("sortingLayerID", instance.sortingLayerID);
            // writer.WriteProperty("sortingOrder", instance.sortingOrder);
            writer.WriteProperty("autoSizeTextContainer", instance.autoSizeTextContainer);
            // internalWriter.SerializeProperty(writer, "maskType", instance.maskType);
            writer.WriteProperty("text", instance.text);
            writer.WriteProperty("isRightToLeftText", instance.isRightToLeftText);
            internalWriter.SerializeProperty(writer, "font", instance.font);
            try
            {
                internalWriter.SerializeProperty(writer, "fontSharedMaterial", instance.fontSharedMaterial);
                // internalWriter.SerializeProperty(writer, "fontSharedMaterials", instance.fontSharedMaterials);
            }
            catch (Exception ex)
            {
                // Handle cases where the TMPro text component maybe disabled or have no textInfo available in UGUI
                Debug.LogWarning("The TextMeshPro component is disabled or has no textInfo available, so there are no Shared materials accessible for saving, if this is intended you can ignore this.", instance);
            }

            //internalWriter.SerializeProperty(writer, "fontMaterial", instance.fontMaterial);
            //internalWriter.SerializeProperty(writer, "fontMaterials", instance.fontMaterials);
            internalWriter.SerializeProperty(writer, "color", instance.color);
            writer.WriteProperty("alpha", instance.alpha);
            writer.WriteProperty("enableVertexGradient", instance.enableVertexGradient);
            internalWriter.SerializeProperty(writer, "colorGradient", instance.colorGradient);
            internalWriter.SerializeProperty(writer, "colorGradientPreset", instance.colorGradientPreset);
            internalWriter.SerializeProperty(writer, "spriteAsset", instance.spriteAsset);
            writer.WriteProperty("tintAllSprites", instance.tintAllSprites);
            writer.WriteProperty("overrideColorTags", instance.overrideColorTags);
            internalWriter.SerializeProperty(writer, "faceColor", instance.faceColor);
            internalWriter.SerializeProperty(writer, "outlineColor", instance.outlineColor);
            writer.WriteProperty("outlineWidth", instance.outlineWidth);
            writer.WriteProperty("fontSize", instance.fontSize);
            internalWriter.SerializeProperty(writer, "fontWeight", instance.fontWeight);
            writer.WriteProperty("enableAutoSizing", instance.enableAutoSizing);
            writer.WriteProperty("fontSizeMin", instance.fontSizeMin);
            writer.WriteProperty("fontSizeMax", instance.fontSizeMax);
            internalWriter.SerializeProperty(writer, "fontStyle", instance.fontStyle);
            internalWriter.SerializeProperty(writer, "alignment", instance.alignment);
            writer.WriteProperty("characterSpacing", instance.characterSpacing);
            writer.WriteProperty("wordSpacing", instance.wordSpacing);
            writer.WriteProperty("lineSpacing", instance.lineSpacing);
            writer.WriteProperty("lineSpacingAdjustment", instance.lineSpacingAdjustment);
            writer.WriteProperty("paragraphSpacing", instance.paragraphSpacing);
            writer.WriteProperty("characterWidthAdjustment", instance.characterWidthAdjustment);
            writer.WriteProperty("enableWordWrapping", instance.enableWordWrapping);
            writer.WriteProperty("wordWrappingRatios", instance.wordWrappingRatios);
            internalWriter.SerializeProperty(writer, "overflowMode", instance.overflowMode);
            internalWriter.SerializeProperty(writer, "linkedTextComponent", instance.linkedTextComponent);
            //writer.WriteProperty("isLinkedTextComponent", instance.isLinkedTextComponent);
            writer.WriteProperty("enableKerning", instance.enableKerning);
            writer.WriteProperty("extraPadding", instance.extraPadding);
            writer.WriteProperty("richText", instance.richText);
            writer.WriteProperty("parseCtrlCharacters", instance.parseCtrlCharacters);
            writer.WriteProperty("isOverlay", instance.isOverlay);
            writer.WriteProperty("isOrthographic", instance.isOrthographic);
            writer.WriteProperty("enableCulling", instance.enableCulling);
            //writer.WriteProperty("ignoreRectMaskCulling", instance.ignoreRectMaskCulling);
            writer.WriteProperty("ignoreVisibility", instance.ignoreVisibility);
            internalWriter.SerializeProperty(writer, "horizontalMapping", instance.horizontalMapping);
            internalWriter.SerializeProperty(writer, "verticalMapping", instance.verticalMapping);
            writer.WriteProperty("mappingUvLineOffset", instance.mappingUvLineOffset);
            internalWriter.SerializeProperty(writer, "renderMode", instance.renderMode);
            internalWriter.SerializeProperty(writer, "geometrySortingOrder", instance.geometrySortingOrder);
            writer.WriteProperty("vertexBufferAutoSizeReduction", instance.vertexBufferAutoSizeReduction);
            writer.WriteProperty("firstVisibleCharacter", instance.firstVisibleCharacter);
            writer.WriteProperty("maxVisibleCharacters", instance.maxVisibleCharacters);
            writer.WriteProperty("maxVisibleWords", instance.maxVisibleWords);
            writer.WriteProperty("maxVisibleLines", instance.maxVisibleLines);
            writer.WriteProperty("useMaxVisibleDescender", instance.useMaxVisibleDescender);
            writer.WriteProperty("pageToDisplay", instance.pageToDisplay);
            internalWriter.SerializeProperty(writer, "margin", instance.margin);
            writer.WriteProperty("isUsingLegacyAnimationComponent", instance.isUsingLegacyAnimationComponent);
            writer.WriteProperty("isVolumetricText", instance.isVolumetricText);
            writer.WriteProperty("maskable", instance.maskable);
            writer.WriteProperty("isMaskingGraphic", instance.isMaskingGraphic);
            writer.WriteProperty("raycastTarget", instance.raycastTarget);
            internalWriter.SerializeProperty(writer, "textWrappingMode", instance.textWrappingMode);
            internalWriter.SerializeProperty(writer, "fontFeatures", instance.fontFeatures);
            //internalWriter.SerializeProperty(writer, "material", instance.material);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (TMPro.TMP_Text)targetObject;
            switch (memberName)
            {
                case "sortingLayerID":
                    reader.ReadProperty<System.Int32>();
                    break;
                case "sortingOrder":
                    reader.ReadProperty<System.Int32>();
                    break;
                case "autoSizeTextContainer":
                    instance.autoSizeTextContainer = reader.ReadProperty<System.Boolean>();
                    break;
                case "maskType":
                    internalReader.DeserializeProperty<TMPro.MaskingTypes>(reader);
                    break;
                case "text":
                    instance.text = reader.ReadProperty<System.String>();
                    break;
                case "isRightToLeftText":
                    instance.isRightToLeftText = reader.ReadProperty<System.Boolean>();
                    break;
                case "font":
                    instance.font = internalReader.DeserializeProperty<TMPro.TMP_FontAsset>(reader);
                    break;
                case "fontSharedMaterial":
                    instance.fontSharedMaterial = internalReader.DeserializeProperty<UnityEngine.Material>(reader);
                    break;
                case "fontSharedMaterials":
                    UnityEngine.Material[] materials = internalReader.DeserializeProperty<UnityEngine.Material[]>(reader);
                    if (materials != null)
                    {
                        instance.fontSharedMaterials = materials;
                    }
                    break;
                //case "fontMaterial":
                //    instance.fontMaterial = internalReader.DeserializeProperty<UnityEngine.Material>(reader);
                //    break;
                //case "fontMaterials":
                //    instance.fontMaterials = internalReader.DeserializeProperty<UnityEngine.Material[]>(reader);
                //    break;
                case "color":
                    instance.color = internalReader.DeserializeProperty<UnityEngine.Color>(reader);
                    break;
                case "alpha":
                    instance.alpha = reader.ReadProperty<System.Single>();
                    break;
                case "enableVertexGradient":
                    instance.enableVertexGradient = reader.ReadProperty<System.Boolean>();
                    break;
                case "colorGradient":
                    instance.colorGradient = internalReader.DeserializeProperty<TMPro.VertexGradient>(reader);
                    break;
                case "colorGradientPreset":
                    instance.colorGradientPreset = internalReader.DeserializeProperty<TMPro.TMP_ColorGradient>(reader);
                    break;
                case "spriteAsset":
                    instance.spriteAsset = internalReader.DeserializeProperty<TMPro.TMP_SpriteAsset>(reader);
                    break;
                case "tintAllSprites":
                    instance.tintAllSprites = reader.ReadProperty<System.Boolean>();
                    break;
                case "overrideColorTags":
                    instance.overrideColorTags = reader.ReadProperty<System.Boolean>();
                    break;
                case "faceColor":
                    instance.faceColor = internalReader.DeserializeProperty<UnityEngine.Color32>(reader);
                    break;
                case "outlineColor":
                    instance.outlineColor = internalReader.DeserializeProperty<UnityEngine.Color32>(reader);
                    break;
                case "outlineWidth":
                    instance.outlineWidth = reader.ReadProperty<System.Single>();
                    break;
                case "fontSize":
                    instance.fontSize = reader.ReadProperty<System.Single>();
                    break;
                case "fontWeight":
                    instance.fontWeight = internalReader.DeserializeProperty<TMPro.FontWeight>(reader);
                    break;
                case "enableAutoSizing":
                    instance.enableAutoSizing = reader.ReadProperty<System.Boolean>();
                    break;
                case "fontSizeMin":
                    instance.fontSizeMin = reader.ReadProperty<System.Single>();
                    break;
                case "fontSizeMax":
                    instance.fontSizeMax = reader.ReadProperty<System.Single>();
                    break;
                case "fontStyle":
                    instance.fontStyle = internalReader.DeserializeProperty<TMPro.FontStyles>(reader);
                    break;
                case "alignment":
                    instance.alignment = internalReader.DeserializeProperty<TMPro.TextAlignmentOptions>(reader);
                    break;
                case "characterSpacing":
                    instance.characterSpacing = reader.ReadProperty<System.Single>();
                    break;
                case "wordSpacing":
                    instance.wordSpacing = reader.ReadProperty<System.Single>();
                    break;
                case "lineSpacing":
                    instance.lineSpacing = reader.ReadProperty<System.Single>();
                    break;
                case "lineSpacingAdjustment":
                    instance.lineSpacingAdjustment = reader.ReadProperty<System.Single>();
                    break;
                case "paragraphSpacing":
                    instance.paragraphSpacing = reader.ReadProperty<System.Single>();
                    break;
                case "characterWidthAdjustment":
                    instance.characterWidthAdjustment = reader.ReadProperty<System.Single>();
                    break;
                case "enableWordWrapping":
                    instance.enableWordWrapping = reader.ReadProperty<System.Boolean>();
                    break;
                case "wordWrappingRatios":
                    instance.wordWrappingRatios = reader.ReadProperty<System.Single>();
                    break;
                case "overflowMode":
                    instance.overflowMode = internalReader.DeserializeProperty<TMPro.TextOverflowModes>(reader);
                    break;
                case "linkedTextComponent":
                    instance.linkedTextComponent = internalReader.DeserializeProperty<TMPro.TMP_Text>(reader);
                    break;
                case "isLinkedTextComponent":
                    //instance.isLinkedTextComponent = reader.ReadProperty<System.Boolean>();
                    break;
                case "enableKerning":
                    instance.enableKerning = reader.ReadProperty<System.Boolean>();
                    break;
                case "extraPadding":
                    instance.extraPadding = reader.ReadProperty<System.Boolean>();
                    break;
                case "richText":
                    instance.richText = reader.ReadProperty<System.Boolean>();
                    break;
                case "parseCtrlCharacters":
                    instance.parseCtrlCharacters = reader.ReadProperty<System.Boolean>();
                    break;
                case "isOverlay":
                    instance.isOverlay = reader.ReadProperty<System.Boolean>();
                    break;
                case "isOrthographic":
                    instance.isOrthographic = reader.ReadProperty<System.Boolean>();
                    break;
                case "enableCulling":
                    instance.enableCulling = reader.ReadProperty<System.Boolean>();
                    break;
                case "ignoreRectMaskCulling":
                    //instance.ignoreRectMaskCulling = reader.ReadProperty<System.Boolean>();
                    break;
                case "ignoreVisibility":
                    instance.ignoreVisibility = reader.ReadProperty<System.Boolean>();
                    break;
                case "horizontalMapping":
                    instance.horizontalMapping = internalReader.DeserializeProperty<TMPro.TextureMappingOptions>(reader);
                    break;
                case "verticalMapping":
                    instance.verticalMapping = internalReader.DeserializeProperty<TMPro.TextureMappingOptions>(reader);
                    break;
                case "mappingUvLineOffset":
                    instance.mappingUvLineOffset = reader.ReadProperty<System.Single>();
                    break;
                case "renderMode":
                    instance.renderMode = internalReader.DeserializeProperty<TMPro.TextRenderFlags>(reader);
                    break;
                case "geometrySortingOrder":
                    instance.geometrySortingOrder = internalReader.DeserializeProperty<TMPro.VertexSortingOrder>(reader);
                    break;
                case "vertexBufferAutoSizeReduction":
                    instance.vertexBufferAutoSizeReduction = reader.ReadProperty<System.Boolean>();
                    break;
                case "firstVisibleCharacter":
                    instance.firstVisibleCharacter = reader.ReadProperty<System.Int32>();
                    break;
                case "maxVisibleCharacters":
                    instance.maxVisibleCharacters = reader.ReadProperty<System.Int32>();
                    break;
                case "maxVisibleWords":
                    instance.maxVisibleWords = reader.ReadProperty<System.Int32>();
                    break;
                case "maxVisibleLines":
                    instance.maxVisibleLines = reader.ReadProperty<System.Int32>();
                    break;
                case "useMaxVisibleDescender":
                    instance.useMaxVisibleDescender = reader.ReadProperty<System.Boolean>();
                    break;
                case "pageToDisplay":
                    instance.pageToDisplay = reader.ReadProperty<System.Int32>();
                    break;
                case "margin":
                    instance.margin = internalReader.DeserializeProperty<UnityEngine.Vector4>(reader);
                    break;
                case "isUsingLegacyAnimationComponent":
                    instance.isUsingLegacyAnimationComponent = reader.ReadProperty<System.Boolean>();
                    break;
                case "isVolumetricText":
                    instance.isVolumetricText = reader.ReadProperty<System.Boolean>();
                    break;
                case "onCullStateChanged":
                    instance.onCullStateChanged = internalReader.DeserializeProperty<UnityEngine.UI.MaskableGraphic.CullStateChangedEvent>(reader);
                    break;
                case "maskable":
                    instance.maskable = reader.ReadProperty<System.Boolean>();
                    break;
                case "isMaskingGraphic":
                    instance.isMaskingGraphic = reader.ReadProperty<System.Boolean>();
                    break;
                case "raycastTarget":
                    instance.raycastTarget = reader.ReadProperty<System.Boolean>();
                    break;
                case "textWrappingMode":
                    instance.textWrappingMode = internalReader.DeserializeProperty<TextWrappingModes>(reader);
                    break;
                case "fontFeatures":
                    instance.fontFeatures = internalReader.DeserializeProperty<List<OTL_FeatureTag>>(reader);
                    break;
                //case "material":
                //    instance.material = internalReader.DeserializeProperty<UnityEngine.Material>(reader);
                //    break;
                case "useGUILayout":
                    instance.useGUILayout = reader.ReadProperty<System.Boolean>();
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return instance;
        }

    }

}