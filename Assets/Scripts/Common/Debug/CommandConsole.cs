using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

public class CommandConsole : MonoBehaviour
{
    [SerializeField] TMP_InputField m_Input;

    PlayerController m_Player;

    Dictionary<string, Action<string[]>> m_CommandDictionary;

    void Start()
    {
        m_Player = FindObjectOfType<PlayerController>();

        if (m_Player != null)
        {
            m_Player.Submit += OnSubmit;
        }

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // command name should be all lowercase
        m_CommandDictionary = new Dictionary<string, Action<string[]>>
        {
            { "modifystat", ModifyStat }
        };
    }

    private void OnSubmit()
    {
        if (m_Input.gameObject.activeSelf)
        {
            var input = m_Input.text;
            ProcessCommand(input);
            m_Input.text = string.Empty;
            m_Input.gameObject.SetActive(false);
        }
        else
        {
            m_Input.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(m_Input.gameObject, null); 
            m_Input.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }

    private void ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Utility.LogWarning("No command entered.");
            return;
        }

        string[] parts = input.Split(' ');
        string commandName = parts[0].ToLower();
        string[] parameters = parts.Length > 1 ? parts[1..] : new string[0];

        if (m_CommandDictionary.TryGetValue(commandName, out Action<string[]> command))
        {
            try
            {
                command(parameters);
            }
            catch (Exception ex)
            {
                Utility.LogError($"Error executing command '{commandName}': {ex.Message}");
            }
        }
        else
        {
            Utility.LogWarning($"Command '{commandName}' not recognized.");
        }
    }

    #region Command Definitions
    /// <summary>
    /// params: statType monsterID amount
    /// </summary>
    /// <param name="parameters"></param>
    private void ModifyStat(string[] parameters)
    {
        if (parameters.Length != 3) throw new ArgumentException("Invalid Parameters - ModifyStat requires the parameters: statType monsterID amount", nameof(parameters));

        if (!Enum.TryParse(parameters[0], true, out StatType type)) throw new ArgumentException("Invalid statType");
        if (!int.TryParse(parameters[1], out int monsterId)) throw new ArgumentException("Invalid monsterID");
        if (!int.TryParse(parameters[2], out int amount)) throw new ArgumentException("Invalid amount");

        var monster = FindObjectsOfType<BattleMonster>().FirstOrDefault(x => x.Id == monsterId)
            ?? throw new ArgumentException($"Monster with monsterId {monsterId} not found");

        monster.ModifyStat(type, amount);

        Utility.Log($"Monster with monsterId of id {monsterId} {Enum.GetName(typeof(StatType), type)} has been modified by {amount}");
    }


    #endregion
}
