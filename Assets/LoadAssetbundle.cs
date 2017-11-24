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

    AssetbundleLoader abLoader = null;
    public static LoadAssetbundle Instance = null;
    private void Start()
    {
        Instance = this;

        //首先加载Manifest文件;
        manifestBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath
                                                              + "/Assetbundle/Assetbundle");
        manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");

        this.mMaterialDict = new Dictionary<string, Material>();
    }

    public string[] GetABDepends(string abName)
    {
        return manifest.GetAllDependencies(abName);
    }

    void OnGUI()
	{        
        if (GUILayout.Button("LoadAssetbundle"))
        {
           
            if (manifestBundle != null)
            {
                if (abLoader != null)
                    abLoader = null;
                abLoader = new AssetbundleLoader("resources/prefab/piaoyi.unity3d");

                abobj = abLoader.GetAsset() as GameObject;

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

            mMaterialDict.Clear();

            abLoader.Unload();

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
        }
        int _count = pGObject.transform.childCount;
        for (int i = 0; i < _count; i++)
        {
            this.ResetShader(pGObject.transform.GetChild(i).gameObject, pParentName);
        }
    }
}
