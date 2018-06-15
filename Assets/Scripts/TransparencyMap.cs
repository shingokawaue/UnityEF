using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transparency map.
/// PNGから作られた透明か透明でないかを格納するマップを扱うクラス (コンポーネントじゃない
/// エディターでPNGファイルからTransparencyMap(.bytesファイル)を作っておく
/// //コンストラクタは非公開でLoadからアクセスさせる。Cache方式
/// </summary>

public class TransparencyMap
{
	byte[] bytes;
	uint width, height;

	public static Dictionary<string, TransparencyMap> cache;

	TransparencyMap(Sprite sprite)
	{//コンストラクタは非公開でLoadからアクセスさせる。Cache方式
		bytes = Resources.Load<TextAsset>("TransparencyMap/" + sprite.name).bytes;
		width = (uint)sprite.texture.width;
		height = (uint)sprite.texture.height;
	}

	static public TransparencyMap Load(Sprite sprite)
	{
		cache = cache ?? new Dictionary<string, TransparencyMap>();

		if (cache.ContainsKey(sprite.name))
			return cache[sprite.name];

		var map = new TransparencyMap(sprite);
		cache.Add(sprite.name, map);
		return map;
	}


	public bool IsTransparency(int x, int y)
	{//左下を起点とする1,1（暫定 　TranceparencyMapは左下が0,0となる

		var index = (int)((x - 1) + ((y - 1) * width)) / 8;
		var flag = 1 << (int)((x - 1) + ((y - 1) * width)) % 8;
		Debug.Log(bytes[index].ToString());
		return bytes[index] > (bytes[index] ^ flag);//^ XORを使ったフラグ判定

	}

}
