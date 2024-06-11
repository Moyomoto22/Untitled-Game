using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager: MonoBehaviour
{
    public List<GameObject> objects;

    private static OrderManager _instance;
    private List<CharacterSpeed> characters;
    private int maxSpeed;

    private List<int> order;

    private OrderManager()
    {
        //characters = new List<CharacterSpeed>();

        //var allies = PartyMembers.Instance.GetAllies();
        //for (int i = 0; i < allies.Count; i++)
        //{
        //    AddCharacter(i, allies[i].agi2);
        //}

        //var enemies = EnemyManager.Instance.GetAllEnemiesStatus();
        //for (int i = 4; i < enemies.Count + 4; i++)
        //{
        //    AddCharacter(i, enemies[i - 4].agi2);
        //}
    }

    public static OrderManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OrderManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("OrderManager");
                    _instance = go.AddComponent<OrderManager>();
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        //if (_instance != null && _instance != this)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}
        //_instance = this;
        //DontDestroyOnLoad(this.gameObject);

        //Initialize();
    }

    public void Initialize()
    {
        RemoveAllCharacters();
        characters = new List<CharacterSpeed>();

        var allies = PartyMembers.Instance.partyMembers;
        for (int i = 0; i < allies.Count; i++)
        {
            AddCharacter(i, allies[i].agi2);
        }

        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();
        for (int i = 4; i < enemies.Count + 4; i++)
        {
            AddCharacter(i, enemies[i - 4].agi2);
        }
    }

    public void AddCharacter(int id, int speed)
    {
        characters.Add(new CharacterSpeed(id, speed));
        if (speed > maxSpeed)
            maxSpeed = speed;
    }

    private void RemoveAllCharacters()
    {
        if (characters != null)
        {
            characters.Clear();
        }
    }

    public List<int> GetActionOrder(int numberOfTurns)
    {
        order = new List<int>();
        
        if (characters.Count == 0)
        {
            Debug.LogError("No characters have been added.");
            return null;
        }


        // Initialize effective speeds for the first turn
        foreach (var character in characters)
        {
            character.SimulateActionsTaken = character.ActionsTaken;
            character.EffectiveSpeed = character.CalculateEffectiveSpeed(maxSpeed, character.ActionsTaken);
        }

        for (int i = 1 + numberOfTurns; i <= numberOfTurns + 10; i++)
        {
            var sortedCharacters = characters.OrderBy(c => c.EffectiveSpeed).ToList();
            var currentChar = sortedCharacters.First();
            Debug.Log($"Turn {i}: ID {currentChar.Id} acts");

            var status = GetStatus(currentChar.Id);

            // 戦闘不能時は行動順に加えず、ループ回数をデクリメントする
            if (!status.knockedOut)
            {
                order.Add(currentChar.Id);
            }
            else
            {
                i -= 1;
            }

            // Update actions taken and recalculate effective speeds
            currentChar.SimulateActionsTaken++;
            foreach (var character in characters)
            {
            character.EffectiveSpeed = character.CalculateEffectiveSpeed(maxSpeed, character.SimulateActionsTaken);
            }
        }

        SetTurnCharacter();

        return order;
    }

    private void SetTurnCharacter()
    {
        for(int i = 0; i < order.Count; i ++)
        {
            CharacterStatus currentCharacter = GetStatus(order[i]);

            if (i == 0)
            {
                TurnCharacter.Instance.SetTurnCharacter(currentCharacter, order[i]);
            }

            Sprite sprite = null;

            if (currentCharacter is AllyStatus)
            {
                var allyStatus = currentCharacter as AllyStatus;
                sprite = allyStatus.Class.imagesD[allyStatus.CharacterID - 1];
            }
            else if (currentCharacter is EnemyStatus)
            {
                var enemyStatus = currentCharacter as EnemyStatus;
                sprite = enemyStatus.eyesSprite;
            }

            objects[i].GetComponentInChildren<Image>().sprite = sprite;

            if (i > 9)
            {
                break;
            }
        }
    }

    private CharacterStatus GetStatus(int id)
    {
        CharacterStatus character = null;
        if (id < 4)
        {
            // プレイヤー
            character = PartyMembers.Instance.GetAllyByIndex(id);
        }
        else
        {
            // エネミー
            var index = id - 4;
            character = EnemyManager.Instance.GetEnemyStatusByIndex(index);
        }
        return character;
    }

    public void IncrementActionsTaken(int ID)
    {
        var character = characters.First(x => x.Id == ID);
        character.ActionsTaken ++;
    }

    private class CharacterSpeed
    {
        public int Id { get; }
        public int Speed { get; }
        public double EffectiveSpeed { get; set; }
        public int ActionsTaken { get; set; }
        public int SimulateActionsTaken { get; set; }

        public CharacterSpeed(int id, int speed)
        {
            Id = id;
            Speed = speed;
        }

        public double CalculateEffectiveSpeed(int maxSpeed, int actionsTaken)
        {
            return ((maxSpeed - Speed) / 100.0 + 1) * (actionsTaken + 1);
        }
    }
}