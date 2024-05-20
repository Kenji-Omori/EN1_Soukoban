using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    int[,] map;  // �ύX�B�񎟌��z��Ő錾
    GameObject[,] field;
    public GameObject playerPrefab;
    public GameObject boxPrefab;

    public GameObject clearText;

    public GameObject particlePrefab;
    //void PrintArray()
    //{
    //  // �ǉ��B������̐錾�Ə�����
    //  string debugText = "";
    //  for (int i = 0; i < map.Length; i++)
    //  {
    //    // �ύX�B������Ɍ������Ă���
    //    debugText += map[i].ToString() + ", ";
    //  }
    //  // ���������������o��
    //  Debug.Log(debugText);
    //}


    /// <summary>
    /// �v���C���[�̃C���f�b�N�X��擾����
    /// </summary>
    /// <returns>�v���C���[�̃C���f�b�N�X�B������Ȃ������ꍇ��-1</returns>
    Vector2Int GetPlayerIndex()
    {
        // �v�f����map.Length�Ŏ擾
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
    /// num�z��̐�����ړ�������
    /// </summary>
    /// <param name="tag">�������Q�[���I�u�W�F�N�g�̃^�O</param>
    /// <param name="moveFrom">���������̃C���f�b�N�X</param>
    /// <param name="moveTo">��������̃C���f�b�N�X</param>
    /// <returns>����</returns>
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        // �ړ��悪�͈͊O�Ȃ�ړ��s��
        if (moveTo.y < 0 || moveTo.y >= map.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= map.GetLength(1)) { return false; }
        // �ړ����2(��)��������

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber("Box", moveTo, moveTo + velocity);
            if (!success) { return false; }
        }



        // �v���C���[�E���ւ�炸�̈ړ�����
        //field[moveFrom.y, moveFrom.x].transform.position = IndexToPosition(moveTo);
        for (int i = 0; i < 5; i++)
        {
            Instantiate(particlePrefab, IndexToPosition(moveFrom), Quaternion.identity);
        }
        Vector3 moveToPosition = IndexToPosition(moveTo);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);

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
        // Vector2Int�^�̉ϒ��z��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // �i�[�ꏊ���ۂ��𔻒f
                if (map[y, x] == 3)
                {
                    // �i�[�ꏊ�̃C���f�b�N�X��T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        // �v�f����goals.Count�Ŏ擾
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // ��ł�������������������B��
                return false;
            }
        }
        // ������B���łȂ���Ώ���B��
        return true;
    }



    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

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
        // �ύX�B��dfor���œ񎟌��z��̏���o��
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                debugText += map[y, x].ToString() + ", ";
            }
            debugText += "\n";  // ���s
        }
        Debug.Log(debugText);
    }

    float autoMoveTime = 0.6f;
    float leftMoveTime = 0;
    int curPattern = 0;
    Vector2Int[] movePattern = new Vector2Int[] {
        new Vector2Int(0,1),
        new Vector2Int(0,1),
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(1,0),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(0,-1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(-1,0),
        new Vector2Int(-1,0),
    };


    // Update is called once per frame
    void Update()
    {
        leftMoveTime -= Time.deltaTime;
        if (leftMoveTime <= 0)
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber("Player",
              playerIndex,
              playerIndex + movePattern[curPattern]
            );
            leftMoveTime = autoMoveTime;
            curPattern++;
            if (curPattern >= movePattern.Length) { curPattern = 0; }
        }


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
