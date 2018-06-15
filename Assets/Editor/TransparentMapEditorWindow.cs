using UnityEngine;
using UnityEditor;
using System.IO;
public class TransparentMapEditorWindow : EditorWindow
{

	[MenuItem("Window/TransparentMapEditor")]
	static void Open()
	{
		GetWindow<TransparentMapEditorWindow>();
	}

	string text, text2;
	Texture2D texture2;


	private void OnGUI()
	{

		EditorGUI.BeginChangeCheck();

		var options = new[] { GUILayout.Width(256), GUILayout.Height(256) };
		texture2 = (Texture2D)EditorGUILayout.ObjectField(texture2, typeof(Texture2D), true, options);


		if (EditorGUI.EndChangeCheck())
		{
			text = "format:" + texture2.format.ToString();
			text += " height:" + texture2.height;
			text += " width:" + texture2.width;
			text2 = texture2.name;
		}
		EditorGUILayout.TextArea(text);
		EditorGUILayout.TextArea(text2);

		if (texture2 != null && texture2.format == TextureFormat.RGBA32)
		{
			EditorGUI.BeginChangeCheck();
			DisplayGenerateButton();
			if (EditorGUI.EndChangeCheck())
			{
				CreateTrancparencyMap(texture2);
			}
		}
		else
		{
			EditorGUI.BeginDisabledGroup(true);
			DisplayGenerateButton();
			EditorGUI.EndDisabledGroup();
		}
	}

	void DisplayGenerateButton()
	{
		GUILayout.Button("GenerateTransparentMap", GUILayout.Width(160));
	}

	public void CreateTrancparencyMap(Texture2D texture)
	{

		var map = texture.GetRawTextureData();

		using (Stream stream = File.OpenWrite("Assets/Resources/TransparencyMap/" + texture.name + ".bytes"))
		{
			using (var writer = new BinaryWriter(stream))
			{
				//writer.Write((uint)texture.width);
				//writer.Write((uint)texture.height);
				for (int i = 0; i < map.Length; i += 4 * 8)
				{
					int f = i + 3, j = i + 7, k = i + 11, l = i + 15, m = i + 19, n = i + 23, o = i + 27, p = i + 31;
					byte b = (byte)(
					(1 << 7) * Bit(map, f) +
					(1 << 6) * Bit(map, j) +
					(1 << 5) * Bit(map, k) +
					(1 << 4) * Bit(map, l) +
					(1 << 3) * Bit(map, m) +
					(1 << 2) * Bit(map, n) +
					(1 << 1) * Bit(map, o) +
					1 * Bit(map, p)
					);
					writer.Write(b);
				}
			}
		}
	}

	private static int Bit(byte[] map, int x)
	{

		if (map.Length <= x)
			return 0;
		//Debug.Log(map[x]);
		return map[x] > 1 ? 1 : 0;
	}
}
