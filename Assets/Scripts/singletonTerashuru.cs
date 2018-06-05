
//using UnityEngine;

////   使い方はこのように継承して使う
////public class SingletonTest : SingletonMonoBehaviour<SingletonTest>
////
////{
////
////}


////Unityのシングルトンは特殊で、MonoBehaviourがnew出来ないため、Awakeで初期化する必要がある。らしい。
//public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour

//{

//    private static T instance;

//    public static T Instance {

//        get {

//            if (instance == null) {

//                instance = (T)FindObjectOfType (typeof(T));

 

//                if (instance == null) {

//                    Debug.LogError (typeof(T) + "is nothing");

//                }

//            }

 

//            return instance;

//        }

//    }

    

//    protected void Awake()

//    {

//        CheckInstance();

//    }

    

//    protected bool CheckInstance()

//    {

//        if( this == Instance){ return true;}

//        Destroy(this);

//        return false;

//    }

//}