using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Evesoft;

namespace NFTGame.Editor
{
    [Serializable]
    public class PixelPivot{
        [InlineEditor(inlineEditorMode:InlineEditorModes.LargePreview)]
        public Sprite sprite;
        public int leftPixel,topPixel;

        #region constructor
        public PixelPivot(){}
        public PixelPivot(Sprite sprite){
            this.sprite = sprite;
        }
        public PixelPivot(Sprite sprite,int leftPixel,int topPixel){
            this.sprite = sprite;
            this.leftPixel = leftPixel;
            this.topPixel = topPixel;
        } 
        #endregion

        #region methods
        public void Set(){
            if(sprite.IsNull())
                return;

            var path = AssetDatabase.GetAssetPath(sprite.texture);
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            var setting = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(setting);

            setting.spriteAlignment = (int)SpriteAlignment.Custom;
            var w = sprite.texture.width;
            var h = sprite.texture.height;

            setting.spritePivot = new Vector2((leftPixel-0.5f)/(float)w,(h-topPixel+0.5f)/(float)h);
            setting.spritePivot.Log();
            setting.spriteGenerateFallbackPhysicsShape = false;
            setting.spritePixelsPerUnit = sprite.pixelsPerUnit;
            setting.filterMode = sprite.texture.filterMode;
            setting.alphaIsTransparency = sprite.texture.alphaIsTransparency;
            setting.textureType = TextureImporterType.Sprite;

            textureImporter.SetTextureSettings(setting);
            textureImporter.SetObjectDirty();
            textureImporter.SaveAndReimport();
        }
        #endregion
    }

    [HideReferenceObjectPicker]
    public class PixelTools
    {
        #region fields
        [TableList(AlwaysExpanded = true)]
        public List<PixelPivot> pixelPivots;
        #endregion

        #region property
        private bool showButton => !pixelPivots.IsNullOrEmpty();
        #endregion

        #region constructor
        public PixelTools(){
            pixelPivots =  new List<PixelPivot>();
        }
        #endregion

        #region methods
        [ShowIf(nameof(showButton)),Button(ButtonSizes.Medium)]
        private void Apply(){
            for (int i = 0; i < pixelPivots.Count; i++)
            {
                if(pixelPivots.IsNull())
                    continue;

                pixelPivots[i].Set();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }   
        #endregion
    }
}


