using UnityEngine;
using System.Collections;

public class LoadAssetbundle : MonoBehaviour {

    protected AsyncOperation req;
    private bool isLoadAsync = false;
    void OnGUI()
	{
        if (GUILayout.Button("LoadAssetbundle"))
        {
            //首先加载Manifest文件;
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                  + "/Assetbundle/Assetbundle");
            if (manifestBundle != null)
            {
                AssetBundleManifest manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");

                //获取依赖文件列表;
                string[] cubedepends = manifest.GetAllDependencies("resources/prefab/cube0.unity3d");
                AssetBundle[] dependsAssetbundle = new AssetBundle[cubedepends.Length];

                for (int index = 0; index < cubedepends.Length; index++)
                {
                    //加载所有的依赖文件;
                    dependsAssetbundle[index] = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                         + "/Assetbundle/"
                                                                         + cubedepends[index]);


                }

                //加载我们需要的文件;"
                AssetBundle cubeBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                  + "/Assetbundle/resources/prefab/cube0.unity3d");
                GameObject cube = cubeBundle.LoadAsset("Cube0") as GameObject;
                if (cube != null)
                {
                    GameObject obj =  Instantiate(cube);
                }
            }
        }
        else if (GUILayout.Button("LoadNormal"))
        {
            LoadRes();

            isLoadAsync = true;
        }
        if (isLoadAsync)
        {
            if (req.isDone)
            {
                Debug.Log(" req.isDone====");
                GameObject cube = ((ResourceRequest)this.req).asset as GameObject;
                GameObject obj = Instantiate(cube);
                isLoadAsync = false;
            }
        }
	}


    public void LoadRes()
    {
        req = Resources.LoadAsync("Prefab/cube0");
    }
}
