using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnner : MonoBehaviour {

    public static PlayerSpawnner instance;
    [SerializeField] private int _spawnCount;
    [SerializeField] private MinMax _spawnInterval;
    [SerializeField] private float _spawnIntervalDecay;
    public GameObject player;
    private int _spawnedCount;
    public List<GameObject> players;
    private bool _gameStarted;
    int saveCount;
    public void Setup()
    {
        _spawnedCount = 0;
    }

    private void Awake()
    {
        if (PlayerSpawnner.instance == null)
            PlayerSpawnner.instance = this;
        players = new List<GameObject>();
        Setup();
    }

    public void StartGame()
    {
        if (_gameStarted) return;

        _gameStarted = true;
        
        SoundManager.Instance.PlayBgm();
        SoundManager.Instance.PlaySfx(SFX.Ui_Start);
        
        Create_Player();
    }

    public void ResetGame()
    {
        _gameStarted = false;
        if (_ieStartGame != null)
            StopCoroutine(_ieStartGame);
        _ieStartGame = null;
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlaySfx(SFX.Ui_Start);
    }

    public void FastenGame()
    {
        if (_ieStartGame != null)
            return;
        
        SoundManager.Instance.PlaySfx(SFX.Ui_Start);
        SoundManager.Instance.PlayBgm();
        
        _ieStartGame = CoStartGame();
        StartCoroutine(_ieStartGame);
    }
    
    private IEnumerator _ieStartGame;
    private IEnumerator CoStartGame()
    {
        float spawnInterval = _spawnInterval.Max;
        for (int i = 0; i < _spawnCount; i++)
        {
            yield return new WaitForSeconds(spawnInterval * Random.Range(0.5f, 1f));
            spawnInterval = Mathf.Max(spawnInterval * _spawnIntervalDecay, _spawnInterval.Min);
            
            Create_Player();
        }
        
        yield break;
    }

    
    public void Create_Player()
    {
        GameObject newPlayer = Instantiate(player);
        newPlayer.transform.position = transform.position + Vector3.up * 300;
        
        players.Add(newPlayer);
        _spawnedCount++;
    }

    public void Reset_Player()
    {
        Debug.Log("진입");
        if (players.Count > 0)
            for (int i = 0; i < players.Count; i++)
            {
                GameObject obj = players[0];
                Destroy(obj);
                
            }
        players.Clear();
        
        Setup();
    }

    public void Check_Player(GameObject obj)
    {
        for(int i = 0; i < players.Count; i ++)
        {
            if(players[i] == obj)
            {
                players.Remove(obj);
                break;
            }
        }
        Destroy(obj);
        SoundManager.Instance.PlaySfx(SFX.DeadByFall);

        if (players.Count == 0 && _spawnedCount == _spawnCount)
        {
            Debug.Log("Game Over");
            SoundManager.Instance.StopBgm();
            Popup_Script.instance.Show("Game Over", "망했어요", "Reset", Reset_Player);
        }
    }
}
