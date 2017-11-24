using UnityEngine;
using System.Collections;
using System.IO;

public class AssetbundleLoader {

    string m_AbName;

    Object asset;

    AssetBundle assetBundle = null;
    AssetBundle[] dependsAssetbundle = null;

    public AssetbundleLoader(string abName)
    {
        m_AbName = abName;

       string [] abDepensName = LoadAssetbundle.Instance.GetABDepends(abName);

        dependsAssetbundle = new AssetBundle[abDepensName.Length];

        //加载所有的依赖文件;
        for (int index = 0; index < abDepensName.Length; index++)
        {
            dependsAssetbundle[index] = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                 + "/Assetbundle/" + abDepensName[index]);
        }

        //加载我们需要的文件;"
        assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                          + "/Assetbundle/" + abName);

        asset = assetBundle.LoadAsset(GetAssetName(abName));
    }

    public static string GetAssetName(string pAssetPath)
    {
        // Find last available character.
        // ... This is either last index or last index before last period.
        int lastIndex = pAssetPath.Length - 1;
        for (int i = lastIndex; i >= 1; i--)
        {
            if (pAssetPath[i] == '.')
            {
                lastIndex = i - 1;
                break;
            }
        }
        // Find first available character.
        // ... Is either first character or first character after closest /
        // ... or \ character after last index.
        int firstIndex = 0;
        for (int i = lastIndex - 1; i >= 0; i--)
        {
            switch (pAssetPath[i])
            {
                case '/':
                case '\\':
                    {
                        firstIndex = i + 1;
                        goto End;
                    }
            }
        }
        End:
        // Return substring.
        return pAssetPath.Substring(firstIndex, (lastIndex - firstIndex + 1));
    }

    public Object GetAsset()
    {
        return asset;
    }

    ~AssetbundleLoader()
    {
        Debug.Log(" ~AssetbundleLoader");
    }

    public void Unload()
    {
        Debug.Log("Unload");
        for (int i = 0; i < dependsAssetbundle.Length; ++i)
        {
            dependsAssetbundle[i].Unload(false);
            dependsAssetbundle[i] = null;
        }
        dependsAssetbundle = null;
        assetBundle.Unload(false);
        assetBundle = null;

        asset = null;
    }
}
