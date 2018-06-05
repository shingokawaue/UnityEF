
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/// <summary>
/// Jsonで保存するSaveData
/// </summary>

[Serializable]//indecates that a class can be serialized.This class cannot be inherited.(indecate示す　inherite受け継ぐ
public class SaveData {
	//SingletonImplementation
	private static SaveData _instance = null;
	public static SaveData Instance{
		get{ if (_instance == null)
			{
				Load();
			}
			return _instance;
			}

	}

	//SaveDataをJsonに変換したテキスト(リロード時に何度も読み込まなくていいように保持)
    [SerializeField]
    private static string _jsonText = "";
	//=================================================================================
	//保存されるデータ(public or SerializeFieldを付ける)
	//=================================================================================

	public int[] holes = {2,-1,-1,1};//ReplacementPuzzle4 の　穴
	public int pickedPiece = -1;//ReplacementPuzzle4 の　持ち上げているピース
	public int pickedHoleNumber = -1;//ReplacementPuzzle4 の　持ち上げているピースの元入っていた穴
	public List<string> itemList = new List<string>();
	public List<string> gameFlag = new List<string>();

	//=================================================================================
    //シリアライズ、デシリアライズ
    //=================================================================================
	//引数のオブジェクトをシリアライズして返す
    private static string Serialize<T>(T obj)
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();//compatible with~  ~と互換性がある
        binaryFormatter.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    //引数のテキストを指定されたクラスにデシリアライズして返す
    private static T Deserialize<T>(string str)
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream(Convert.FromBase64String(str));
        return (T)binaryFormatter.Deserialize(memoryStream);
    }

	//=================================================================================
    //取得
    //=================================================================================

    /// <summary>
    /// データを再読み込みする。
    /// </summary>
    public void Reload()
    {
        JsonUtility.FromJsonOverwrite(GetJson(), this);
    }

    //データを読み込む。
    private static void Load()
    {
        _instance = JsonUtility.FromJson<SaveData>(GetJson());
		Debug.Log("Load :" + _jsonText);
    }

    //保存しているJsonを取得する
    private static string GetJson()
    {
        //既にJsonを取得している場合はそれを返す。
        if (!string.IsNullOrEmpty(_jsonText))
        {
            return _jsonText;
        }

        //Jsonを保存している場所のパスを取得。
        string filePath = GetSaveFilePath();

        //Jsonが存在するか調べてから取得し変換する。存在しなければ新たなクラスを作成し、それをJsonに変換する。
        if (File.Exists(filePath))
        {
            _jsonText = File.ReadAllText(filePath);
        }
        else
        {
            _jsonText = JsonUtility.ToJson(new SaveData());
        }

        return _jsonText;
    }

    //=================================================================================
    //保存
    //=================================================================================

    /// <summary>
    /// データをJsonにして保存する。
    /// </summary>
    public void Save()
    {
        _jsonText = JsonUtility.ToJson(this);
        File.WriteAllText(GetSaveFilePath(), _jsonText);
		Debug.Log("Save" + _jsonText);
    }

    //=================================================================================
    //削除
    //=================================================================================

    /// <summary>
    /// データを全て削除し、初期化する。
    /// </summary>
    public void Delete()
    {
        _jsonText = JsonUtility.ToJson(new SaveData());
        Reload();
    }

    //=================================================================================
    //保存先のパス
    //=================================================================================

    //保存する場所のパスを取得。
    private static string GetSaveFilePath()
    {

        string filePath = "SaveData";

        //確認しやすいようにエディタではAssetsと同じ階層に保存し、それ以外ではApplication.persistentDataPath以下に保存するように。
#if UNITY_EDITOR
        filePath += ".json";
		Debug.Log("FilePath = " + filePath);
#else
    filePath = Application.persistentDataPath + "/" + filePath;
		Debug.Log("Saved Data");
#endif

        return filePath;
    }


}
