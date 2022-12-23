using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    static string AppBranch = Environment.GetEnvironmentVariable("BRANCH");
    static string BuildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

    private static string[] GetScene()
    {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }

    static void WebGL()
    {
        string BuildLocation = "./nftgame_" + AppBranch + "_" + BuildNumber + "/";
        System.Console.WriteLine(BuildLocation);

        PlayerSettings.SetIncrementalIl2CppBuild(BuildTargetGroup.WebGL, true);
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        BuildPipeline.BuildPlayer(GetScene(), BuildLocation, BuildTarget.WebGL, BuildOptions.None);
    }
}