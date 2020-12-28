using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class GameObjectPool : MonoBehaviour
{
    public static GameObjectPool Instance;
    private static Dictionary<string, ArrayList> poolDic = new Dictionary<string, ArrayList>();
    void Start()
    {
        Instance = this;
    }
    //入池
    public void InPool(GameObject _obj)
    {
        string tmp_Key = _obj.name;
        //判断是否含有该键
        if (poolDic.ContainsKey(tmp_Key))
        {
            poolDic[tmp_Key].Add(_obj);
        }
        else
        {
            poolDic[tmp_Key] = new ArrayList();
            poolDic[tmp_Key].Add(_obj);
        }
        _obj.SetActive(false);
        //Debug.Log("入池成功！！入池的key名为：" + tmp_Key);
        //Debug.Log("当前对象池里" + tmp_Key + "的数量：" + poolDic[tmp_Key].Count);
    }
    //出池
    public GameObject OutPool(GameObject _obj, Vector3 _objVector3, Quaternion _objRotation)
    {
        string tmp_Key = _obj.name + "(Clone)";
        string tmp_PrefabName = _obj.name;
        //Debug.Log("出池：" + tmp_PrefabName);
        Object tmp_Object;
        if (poolDic.ContainsKey(tmp_Key) && poolDic[tmp_Key].Count >= 1)
        {
            ArrayList tmp_arryList = poolDic[tmp_Key];
            tmp_Object = tmp_arryList[0] as Object;
            tmp_arryList.RemoveAt(0);

            (tmp_Object as GameObject).transform.position = _objVector3;
            (tmp_Object as GameObject).transform.rotation = _objRotation;
            (tmp_Object as GameObject).SetActive(true);
        }
        else
        {
            tmp_Object = Instantiate(Resources.Load("Prefabs/" + tmp_PrefabName), _objVector3, _objRotation);
        }
        return tmp_Object as GameObject;
    }
}
