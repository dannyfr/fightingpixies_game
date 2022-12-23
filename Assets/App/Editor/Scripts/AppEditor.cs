using UnityEngine;
using Sirenix.OdinInspector;

namespace NFTGame.Editor
{
    [HideMonoScript]
    [CreateAssetMenu(menuName = Utils.EditorMenu.Editor + nameof(AppEditor),fileName = nameof(AppEditor))]
    public class AppEditor : SerializedScriptableObject
    {
        [TabGroup("Pivot"),HideLabel]
        public PixelTools pixelTool = new PixelTools();

        [TabGroup("Bundle"),HideLabel]
        public UploadBundleTool bundleTool = new UploadBundleTool();

        [TabGroup("Preset"),HideLabel]
        public UploadPresetTool presetTool = new UploadPresetTool();
    }
}


