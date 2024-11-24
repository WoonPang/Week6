using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // [SerializeField]
    // private GameObject enemyPrefab; // 적 프리펩
    [SerializeField]
    private GameObject enemyHPSliderPrefab; // 적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private Transform canvasTransform;  // UI를 표현하는 Canvas 오브젝트의 Transform
    // [SerializeField]
    // private float spawnTime;    // 적 생성 주기
    [SerializeField]
    private Transform[] wayPoints;  // 현재 스테이지 이동 경로
    [SerializeField]
    private PlayerHP playerHP;  // 플레이어의 체력 컴포넌트
    [SerializeField]
    private PlayerGold playerGold;    // 플레이어의 골드 컴포넌트
    private Wave currentWave;   // 현재 웨이브 정보
    private int currentEnemyCount;  // 현재 웨이브에 남아있는 적 숫자 (웨이브 시작 시 max로 설정, 적 사망 시 -1)
    private List<Enemy> enemyList;  // 현재 맵에 존재하는 모든 적의 정보

    // 적의 생성과 삭제는 EnemySpawner에서 하기 때문에 Set은 필요 없다.
    public List<Enemy> EnemyList => enemyList;
    // 현재 웨이브에 남아있는 적, 최대 적 숫자
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        // 적 리스트 메모리 할당
        enemyList = new List<Enemy>();
        // 적 생성 코루틴 함수 호출
        // StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        // 매개변수로 받아온 웨이브 정보 저장
        currentWave = wave;
        // 현재 웨이브의 최대 적 숫자를 저장
        currentEnemyCount = currentWave.maxEnemyCount;
        // 현재 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        // 현재 웨이브에서 생성한 적 숫자
        int spawnEnemyCount = 0;

        // while (true)
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // GameObject clone = Instantiate(enemyPrefab);    // 적 오브젝트 생성
            // 웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하도록 설정하고, 적 오브젝트 생성
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();  // 방금 생성된 적의 Enemy 컴포넌트

            // this는 나 자신 (자신의 EnemySpawner 정보)
            enemy.Setup(this, wayPoints); // wayPoint 정보를 매개변수로 Setup() 호출
            enemyList.Add(enemy);   // 리스트에 방금 생성된 적 정보 저장

            SpawnEnemyHPSlider(clone);

            // 현재 웨이브에서 생성한 적의 숫자 +1
            spawnEnemyCount++;

            // yield return new WaitForSeconds(spawnTime); // spawnTime 동안 대기
            // 각 웨이브마다 spawnTime이 다를 수 있기 때문에 현재 웨이브(currentWave)의 spawnTIme 사용
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // 적이 목표지점까지 도착했을 때
        if (type == EnemyDestroyType.Arrive)
        {
            // 플레이어의 체력 -1
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.kill)
        {
            // 적의 종류에 따라 사망 시 골드 휙득
            playerGold.CurrentGold += gold;
        }

        // 적이 사망할 때마다 현재 웨이브의 생존 적 숫자 감소 (UI 표시용)
        currentEnemyCount--;
        // 리스트에서 사망하는 적 정보 삭제
        enemyList.Remove(enemy);
        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        // 적 체력을 나타내는 Slider UI 생성
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        // Slider UI 오브젝트를 parent("Canvas" 오브젝트)의 자식으로 설정
        // Tip. UI는 캔버스의 자식오브젝트로 설정되어 있어야 화면에 보인다.
        sliderClone.transform.SetParent(canvasTransform);
        // 계층 설정으로 바뀐 크기를 다시 (1, 1, 1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI가 쫓아다닐 대상을 본인으로 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}