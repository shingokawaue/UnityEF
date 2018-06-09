using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common;
public class HintString {
	/// <summary>
	/// Key:GameFlag Value:CorrespondingTips
    /// </summary>
	public static Dictionary<string, string> hintTable= new Dictionary<string, string>()
    {
		{Define.FLAG_GET_CHAIR,"イスを使ってみよう！"},
		{Define.FLAG_OPEN_DIRBOX,"本の矢印の向きをよく見てみよう！"},
		{Define.FLAG_OPEN_STARBOX,"部屋にある星の数を数えてみよう！"},
		{Define.FLAG_GET_PENCIL,"開けた引き出しの中にある鉛筆を取ろう！"},
		{Define.FLAG_GET_REDKEY,"開けた星の中にあるカギを取ろう！"},
		{Define.FLAG_PUT_CHAIR,"高い棚を見るためにイスを置こう！"},
		{Define.FLAG_USE_REDKEY,"赤いカギを使ってみよう！"},
		{Define.FLAG_GET_PENCILSHARPNER,"高い棚を見てみよう！"},
		{Define.FLAG_GET_CUBEDOG,"ピンクの引き出しの中から犬のおもちゃを取ろう！"},
		{Define.FLAG_GET_SHARPEDPENCIL,"鉛筆を削ってみよう！(アイテムにアイテムが使えるよ！"},
		{Define.FLAG_GET_PAPER,"黄色いゴミ箱の裏を見てみよう！"},
		{Define.FLAG_GET_PAPERDRAW,"紙に何かを書いてみよう！"},
		{Define.FLAG_PUT_CUBEDOG ,"おもちゃの犬を置けそうなところに置いてみよう！"},
		{Define.FLAG_PUT_BANANA,"バナナをサルに持たせてみよう！"},
		{Define.FLAG_PUT_BANANA2,"もう一つバナナをサルに持たせてみよう！"},
		{Define.FLAG_OPEN_LRBOX,"サルの動きをよく見てバナナのボタンを押してみよう！"},
		{Define.FLAG_OPEN_NUMBERBOX,"サルの目を見てみよう！"}

    };

}
