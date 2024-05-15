using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDelete : MonoBehaviour
{
    void Start()
    {
        // 在 2 秒后调用销毁方法
        Invoke("DestroyObject", 2.0f);
    }

    void DestroyObject()
    {
        // 销毁当前游戏对象
        Destroy(gameObject);
    }
}

