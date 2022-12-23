using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace NFTGame.Editor
{
    public class PrebuildProcess : IPreprocessBuildWithReport
    {
        #region IPreprocessBuildWithReport
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            
        }
        #endregion
    } 
}