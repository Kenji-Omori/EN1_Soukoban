using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
  int[] map;

  void PrintArray()
  {
    // 追加。文字列の宣言と初期化
    string debugText = "";
    for (int i = 0; i < map.Length; i++)
    {
      // 変更。文字列に結合していく
      debugText += map[i].ToString() + ", ";
    }
    // 結合した文字列を出力
    Debug.Log(debugText);
  }


  /// <summary>
  /// プレイヤーのインデックスを取得する
  /// </summary>
  /// <returns>プレイヤーのインデックス。見つからなかった場合は-1</returns>
  int GetPlayerIndex()
  {
    // 要素数はmap.Lengthで取得
    for (int i = 0; i < map.Length; i++)
    {
      if (map[i] == 1)
      {
        return i;
      }
    }
    return -1;
  }

  /// <summary>
  /// num配列の数字を移動させる
  /// </summary>
  /// <param name="number">動かす数字</param>
  /// <param name="moveFrom">動かす元のインデックス</param>
  /// <param name="moveTo">動かす先のインデックス</param>
  /// <returns>成否</returns>
  bool MoveNumber(int number, int moveFrom, int moveTo)
  {
    if (moveTo < 0 || moveTo >= map.Length)
    {
      // 動けない条件を先に書き、リターンする。早期リターン
      return false;
    }
    map[moveTo] = number;
    map[moveFrom] = 0;
    return true;
  }

  // Start is called before the first frame update
  void Start()
  {

    // 配列の実態の作成と初期化
    map = new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0 };
    PrintArray();
  }
  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      int playerIndex = GetPlayerIndex();
      MoveNumber(1, playerIndex, playerIndex + 1);
      PrintArray();
    }

    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      int playerIndex = GetPlayerIndex();
      MoveNumber(1, playerIndex, playerIndex - 1);
      PrintArray();
    }
  }
}
