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
    public int spawnMaxCnt = 5;

    //������ ���� ���� ��ǥ (x,z)��ġ
    float rndPos = 10f;

    void Spawn()
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

    private void Start()
    {
        //�ݺ������� ���͸� ����� InvokeRepeating
        InvokeRepeating("Spawn", 3f, 5f);
    }
}