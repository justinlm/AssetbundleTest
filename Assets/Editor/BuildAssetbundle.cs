using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class BuildAssetbundle : Editor
{
    public static string SourcePath = Application.dataPath + "/ExportRes/Resources";
    public static string TextruePath = SourcePath + "/Textures";
    public static string MaterialsPath = SourcePath + "/Materials";
    public static string AudioPath = SourcePath + "/Audio";
    public static string PrefabPath = SourcePath + "/Prefab";
    public static string ShaderPath = SourcePath + "/Shader";
    public static string RES_FLODER_NAME = "Resources";

    [MenuItem("BuildAssetbundle/BuildAll")]
	static void Build()
	{

        ClearAssetBundleName();

        Pack(ShaderPath);//打包shader，不然会造成材质里面一人保持一份shader，造成冗余
        Pack(TextruePath);
        Pack(MaterialsPath);
        Pack(AudioPath);
        Pack(PrefabPath);


        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath+ "/Assetbundle", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("提示", "导出完成", "确定");
    }

    private static void Pack(string source)
    {
        if (source == string.Empty)
            return;
        if (source.Contains(".unity"))
        {
            //string _path = AssetBundleNameUtility.GetAssetBundleNameByPath(source);
            SetAssetName(source);
            return;
        }

        DirectoryInfo _folder = new DirectoryInfo(source);
        FileSystemInfo[] _files = _folder.GetFileSystemInfos();
        int _length = _files.Length;
        for (int i = 0; i < _length; i++)
        {
            if (_files[i] is DirectoryInfo)
            {
                Pack(_files[i].FullName);
            }
            else
            {
                if (_files[i].Name.EndsWith(".meta") || _files[i].Name.EndsWith(".cs") && _files[i].Name.EndsWith(".FBX"))
                    continue;

                string _path = Replace(_files[i].FullName);
                _path = ReplaceDataPath(_path);

                Object _obj = AssetDatabase.LoadMainAssetAtPath(_path);
                if (_obj == null)
                {
                    Debug.LogError("Asset is null : " + _path);
                    continue;
                }

                SetAssetName(_path);
            }
        }
    }

    private static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }

    private static string ReplaceDataPath(string s)
    {
        return "Assets" + s.Substring(Application.dataPath.Length);
    }

    /// <summary>
    /// 清除上次设置过的AssetbundleName,避免再次打包
    /// </summary>
    public static void ClearAssetBundleName()
    {
        int _length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("LastAssets : " + _length);
        string[] _oldAssetBundleNames = new string[_length];
        for (int i = 0; i < _length; i++)
        {
            _oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }
        for (int j = 0; j < _oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(_oldAssetBundleNames[j], true);
        }
        _length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("NowAssets: " + _length);
    }

    /// <summary>
    /// 设置AssetBundleName
    /// </summary>
    /// <param name="path"></param>
    public static void SetAssetName(string path)
    {
        Debug.Log("AssetPath : " + path);
        AssetImporter _assetImporter = AssetImporter.GetAtPath(path);

        _assetImporter.assetBundleName = GetAssetBundleNameByPath(path);
    }

    public static string GetAssetBundleNameByPath(string path)
    {
        Debug.Log(path);
        path = path.Substring(("Assets/").Length + 1);
        string _assetName = path.Substring(path.IndexOf("/") + 1);
        _assetName = _assetName.Replace(Path.GetExtension(_assetName),
           ".unity3d");
        if (!_assetName.Contains(RES_FLODER_NAME + "/"))
        {
            _assetName = RES_FLODER_NAME + "/" + _assetName;
        }
        return _assetName;
    }

}
