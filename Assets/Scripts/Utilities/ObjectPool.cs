using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {
    Stack<GameObject> inactive;
    GameObject prefab;
    private Transform parent;
	
    public ObjectPool(GameObject prefab, Transform parent, int initialQty) {
        this.prefab = prefab;
        this.parent = parent;
        inactive = new Stack<GameObject>(initialQty);
    }
	
    public GameObject Spawn(Vector3 pos) {
        GameObject obj = null;
        if(inactive.Count==0) {
            obj = (GameObject)GameObject.Instantiate(prefab, parent, false);
        }
        else {
            while (obj == null && inactive.Count > 0) obj = inactive.Pop();
            if(obj == null) return Spawn(pos);
        }

        obj.transform.position = pos;
        obj.SetActive(true);
        return obj;

    }
    public void Despawn(GameObject obj) {
        obj.SetActive(false);
        inactive.Push(obj);
    }
}
