using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadAssetbundle : MonoBehaviour {

    private Dictionary<string, Material> mMaterialDict;

    protected AsyncOperation req;
    private bool isLoadAsync = false;

    GameObject obj = null;
    GameObject abobj = null;

    AssetBundle manifestBundle = null;
    AssetBundleManifest manifest = null;

    AssetBundle assetBundle = null;
    AssetBundle[] dependsAssetbundle = null;

    private void Start()
    {
        //首先加载Manifest文件;
        manifestBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                              + "/Assetbundle/Assetbundle");
        manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");

        this.mMaterialDict = new Dictionary<string, Material>();
    }

    void OnGUI()
	{        
        if (GUILayout.Button("LoadAssetbundle"))
        {
           
            if (manifestBundle != null)
            {
                //获取依赖文件列表;
                string[] cubedepends = manifest.GetAllDependencies("resources/prefab/piaoyi.unity3d");
                dependsAssetbundle = new AssetBundle[cubedepends.Length];

                for (int index = 0; index < cubedepends.Length; index++)
                {
                    //加载所有的依赖文件;
                    dependsAssetbundle[index] = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                         + "/Assetbundle/"
                                                                         + cubedepends[index]);


                }

                //加载我们需要的文件;"
                assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                                  + "/Assetbundle/resources/prefab/piaoyi.unity3d");
                abobj = assetBundle.LoadAsset("piaoyi") as GameObject;
                if (abobj != null)
                {
                    obj =  Instantiate(abobj);

                    ResetShader(obj, obj.name);
                }
            }
        }
        else if (GUILayout.Button("LoadNormal"))
        {
            LoadRes();

            isLoadAsync = true;
        }
        else if(GUILayout.Button("UnloadRes"))
        {
            GameObject.Destroy(obj);
            obj = null;

            abobj = null;

            for (int i = 0; i < dependsAssetbundle.Length; ++i)
            {
                dependsAssetbundle[i].Unload(false);
                dependsAssetbundle[i] = null;
            }
            dependsAssetbundle = null;
            assetBundle.Unload(false);
            assetBundle = null;
            mMaterialDict.Clear();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        if (isLoadAsync)
        {
            if (req.isDone)
            {
                Debug.Log(" req.isDone====");
                GameObject cube = ((ResourceRequest)this.req).asset as GameObject;
                obj = Instantiate(cube);
                isLoadAsync = false;
            }
        }
	}


    public void LoadRes()
    {
        req = Resources.LoadAsync("Prefab/piaoyi");
    }

    private void OnDestroy()
    {
        if(manifestBundle != null)
            manifestBundle.Unload(true);
        manifest = null;

        if(obj != null)
            GameObject.Destroy(obj);

        abobj = null;
    }


    private void ResetShader(GameObject pGObject, string pParentName)
    {
        if (pGObject == null)
            return;
        //		if(pGObject.name.StartsWith("car") || pGObject.name.StartsWith("Car"))
        //			return;
        if (pGObject.transform.GetComponent<Renderer>() != null)
        {
            Material _item = null;
            Material _material = null;
            Material[] _materials = pGObject.GetComponent<Renderer>().materials;
            for (int i = 0; i < _materials.Length; i++)
            {
                _item = _materials[i];
                string _path = pParentName + "_" + _item.name;
                if (this.mMaterialDict.TryGetValue(_path, out _material) == false)
                {
                    _material = _item;
                    _material.shader = Shader.Find(_material.shader.name);
                    this.mMaterialDict.Add(_path, _material);
                }
                _materials[i] = _material;
            }
            pGObject.GetComponent<Renderer>().sharedMaterials = _materials;
            //pGObject.renderer.materials = _materials;
        }
        int _count = pGObject.transform.childCount;
        for (int i = 0; i < _count; i++)
        {
            this.ResetShader(pGObject.transform.GetChild(i).gameObject, pParentName);
        }
    }
}
