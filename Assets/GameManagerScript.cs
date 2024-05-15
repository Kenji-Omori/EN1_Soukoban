using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
  int[,] map;  // 変更。二次元配列で宣言
  GameObject[,] field;
  public GameObject playerPrefab;
  public GameObject boxPrefab;

  public GameObject clearText;
  //void PrintArray()
  //{
  //  // 追加。文字列の宣言と初期化
  //  string debugText = "";
  //  for (int i = 0; i < map.Length; i++)
  //  {
  //    // 変更。文字列に結合していく
  //    debugText += map[i].ToString() + ", ";
  //  }
  //  // 結合した文字列を出力
  //  Debug.Log(debugText);
  //}


  /// <summary>
  /// プレイヤーのインデックスを取得する
  /// </summary>
  /// <returns>プレイヤーのインデックス。見つからなかった場合は-1</returns>
  Vector2Int GetPlayerIndex()
  {
    // 要素数はmap.Lengthで取得
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        if (field[y, x] == null) { continue; }
        if (field[y, x].tag == "Player")
        {
          return new Vector2Int(x, y);
        }
      }
    }
    return new Vector2Int(-1, -1);
  }

  /// <summary>
  /// num配列の数字を移動させる
  /// </summary>
  /// <param name="tag">動かすゲームオブジェクトのタグ</param>
  /// <param name="moveFrom">動かす元のインデックス</param>
  /// <param name="moveTo">動かす先のインデックス</param>
  /// <returns>成否</returns>
  bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
  {
    // 移動先が範囲外なら移動不可
    if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }
    if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
    // 移動先に2(箱)が居たら

    if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
    {
      Vector2Int velocity = moveTo - moveFrom;
      bool success = MoveNumber("Box", moveTo, moveTo + velocity);
      if (!success) { return false; }
    }



    // プレイヤー・箱関わらずの移動処理
    field[moveFrom.y, moveFrom.x].transform.position = IndexToPosition(moveTo);
    field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
    field[moveFrom.y, moveFrom.x] = null;
    return true;
  }

  Vector3 IndexToPosition(Vector2Int index)
  {
    return new Vector3(
      index.x - map.GetLength(1) / 2 + 0.5f,
      index.y - map.GetLength(0) / 2,
      0);
  }


  bool IsCleard()
  {
    // Vector2Int型の可変長配列の作成
    List<Vector2Int> goals = new List<Vector2Int>();
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        // 格納場所か否かを判断
        if (map[y, x] == 3)
        {
          // 格納場所のインデックスを控えておく
          goals.Add(new Vector2Int(x, y));
        }
      }
    }
    // 要素数はgoals.Countで取得
    for (int i = 0; i < goals.Count; i++)
    {
      GameObject f = field[goals[i].y, goals[i].x];
      if (f == null || f.tag != "Box")
      {
        // 一つでも箱が無かったら条件未達成
        return false;
      }
    }
    // 条件未達成でなければ条件達成
    return true;
  }



  // Start is called before the first frame update
  void Start()
  {


    map = new int[,] {
      { 1, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 2, 3, 0, 0 },
      { 0, 0, 0, 0, 2, 3, 0, 0 },
      { 0, 0, 0, 0, 2, 3, 0, 0 },
      { 0, 0, 0, 0, 2, 3, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0 },
      { 0, 0, 0, 0, 0, 0, 0, 0 },
    };
    field = new GameObject[
        map.GetLength(0),
        map.GetLength(1)
      ];


    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        if (map[y, x] == 1)
        {
          field[y, x] = Instantiate(
            playerPrefab,
            IndexToPosition(new Vector2Int(x, y)),
            Quaternion.identity
          );
        }
        if (map[y, x] == 2)
        {
          field[y, x] = Instantiate(
            boxPrefab,
            IndexToPosition(new Vector2Int(x, y)),
            Quaternion.identity
          );
        }
      }
    }



    string debugText = "";
    // 変更。二重for文で二次元配列の情報を出力
    for (int y = 0; y < map.GetLength(0); y++)
    {
      for (int x = 0; x < map.GetLength(1); x++)
      {
        debugText += map[y, x].ToString() + ", ";
      }
      debugText += "\n";  // 改行
    }
    Debug.Log(debugText);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      Vector2Int playerIndex = GetPlayerIndex();
      MoveNumber("Player",
        playerIndex,
        playerIndex + new Vector2Int(1, 0)
      );
      if (IsCleard())
      {
        clearText.SetActive(true);
      }

    }

    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      Vector2Int playerIndex = GetPlayerIndex();
      MoveNumber("Player",
        playerIndex,
        playerIndex + new Vector2Int(-1, 0)
      );
      if (IsCleard())
      {
        clearText.SetActive(true);
      }
    }

    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      Vector2Int playerIndex = GetPlayerIndex();
      MoveNumber("Player",
        playerIndex,
        playerIndex + new Vector2Int(0, 1)
      ); 
      if (IsCleard())
      {
        clearText.SetActive(true);
      }

    }

    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      Vector2Int playerIndex = GetPlayerIndex();
      MoveNumber("Player",
        playerIndex,
        playerIndex + new Vector2Int(0, -1)
      ); 
      if (IsCleard())
      {
        clearText.SetActive(true);
      }

    }

  }
}
