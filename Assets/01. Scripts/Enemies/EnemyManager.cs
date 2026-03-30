using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject enemyPrefab;            // 적 프리팹
    [SerializeField] private GameObject boxPrefab;              // 상자 프리팹

    [Header("플레이어")]
    [SerializeField] private Transform player;                  // 플레이어

    [Header("총알 공장")]
    [SerializeField] private BulletFactory bulletFactory;       // 총알 공장

    [Header("정보")]
    [SerializeField] private Transform spawnPoints;             // 생성 위치들
    [SerializeField] private int maxEnemyCount;                 // 적 최대 마리
    [SerializeField] private Transform enemyContainer;          // 적 컨테이너

    private IObjectPool<EnemyBT> enemyPool;                     // 적 풀

    private void Awake()
    {
        // 초기화
        enemyPool = new ObjectPool<EnemyBT>(() => CreateEnemy(),
            GetEnemy, ReturnEnemy, DestoryEnemy, maxSize: maxEnemyCount);
    }

    private void Start()
    {
        // 적 생성
        SpawnEnemys();
    }

    private EnemyBT CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.transform.parent = enemyContainer;
        return enemy.GetComponentInChildren<EnemyBT>();
    }

    private void GetEnemy(EnemyBT enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.Init(player, bulletFactory, enemyPool);
    }

    private void ReturnEnemy(EnemyBT enemy)
    {
        enemy.gameObject.SetActive(false);
        Vector3 pos = enemy.transform.position;
        pos.y += enemy.transform.localScale.y * 0.5f;
        Subject<IEnemyDeadHandler>.Publish(h => h.OnEnemyDead(boxPrefab, pos));
    }

    private void DestoryEnemy(EnemyBT enemy) => Destroy(enemy.gameObject);

    private void SpawnEnemys()
    {
        for (int i = 0; i < spawnPoints.childCount; i++)
        {
            var enemy = enemyPool.Get();
            var agent = enemy.GetComponentInChildren<NavMeshAgent>();

            if (agent != null)
            {
                agent.Warp(spawnPoints.GetChild(i).position);
                enemy.SetPointPosition(spawnPoints.GetChild(i).position);
            }
        }
    }
}