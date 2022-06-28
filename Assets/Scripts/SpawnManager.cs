using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //������ ���� ������Ʈ
    public GameObject monsterSpawner = null;

    //����Ȱ ���͵� ��Ƴ���
    public List<GameObject> monsters = new List<GameObject>();

    //������ ���� �ִ��
    public int spawnMaxCnt = 10;

    //������ ���� ���� ��ǥ (x,z)��ġ
    float rndPos = 10f;

    //������ ��������
    public GameObject bossMonster = null;

    public bool foxSpawnCheck = false;

    void Spawn()
    {
        if (PlayerMove.Instance.killEnemy <= 10)
        {
            //���� ���� ������ ���� �ִ�� ���� ũ�� ���ư�~
            if (monsters.Count >= spawnMaxCnt)
            {
                return;
            }

            //������ ��ġ�� �����Ѵ�. �ʱ� ���̸� 1000 ������ .x,z�� ���� 
            Vector3 vecSpawn = new Vector3(Random.Range(-rndPos, rndPos), 100f, Random.Range(-rndPos, rndPos));

            //������ �ӽ� ���̿��� �Ʒ��������� Raycast�� ���� �������� ���� ���ϱ�
            Ray ray = new Ray(vecSpawn, Vector3.down);

            //Raycast ���� ��������
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            {
                //Raycast ���̸� y������ �缳��
                vecSpawn.y = raycastHit.point.y;
            }

            //������ ���ο� ���͸� Instantiate�� clone�� �����.
            GameObject newMonster = Instantiate(monsterSpawner, vecSpawn, Quaternion.identity);

            //���� ��Ͽ� ���ο� ���͸� �߰�
            monsters.Add(newMonster);
        }
        else if (foxSpawnCheck == false)
        {
            CancelSpawn();
            foxSpawnCheck = true;
            GameManager.Instance.boosHp.gameObject.SetActive(true);
        }
        else
        {
            return;
        }
    }

    private void Start()
    {
        //�ݺ������� ���͸� ����� InvokeRepeating
        InvokeRepeating("Spawn", 0.5f, 2f);
        foxSpawnCheck = false;
    }

    private void CancelSpawn()
    {
        CancelInvoke("Spawn");
        Vector3 vecSpawn = new Vector3(Random.Range(-rndPos, rndPos), 100f, Random.Range(-rndPos, rndPos));
        Ray ray = new Ray(vecSpawn, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            vecSpawn.y = raycastHit.point.y;
        GameObject newMonster = Instantiate(bossMonster, vecSpawn, Quaternion.identity);
    }

    private static SpawnManager instance;
    public static SpawnManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<SpawnManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("Singleton Class").AddComponent<SpawnManager>();
                    instance = newSingleton;
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
}