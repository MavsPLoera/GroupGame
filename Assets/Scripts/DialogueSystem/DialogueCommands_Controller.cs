using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Can maybe consider turning this into a scriptable object
public class DialogueCommands_Controller : MonoBehaviour
{
    /*
     * Need to store this list twice to take advantage of O(1) access of dictionary.
     * 
     * Unity does not serialize dictionaries in the editor, so in order to add commands to the dictionary, we need to first store it in a list then copy every value to the dictionary.
     */
    public Command[] listCommands;
    public Dictionary<string, UnityEvent> commands = new Dictionary<string, UnityEvent>();
    public static DialogueCommands_Controller instance;

    void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Command command in listCommands)
        {
            commands.Add(command.key, command.events);
        }
    }

    [ContextMenu("Test Call Command")]
    public void testCallCommand()
    {
        CallCommand("test");
    }

    public void CallCommand(string key)
    {
        if (commands.ContainsKey(key))
        {
            UnityEvent temp = commands[key];
            temp.Invoke();
        }
        else
        {
            Debug.LogError($"Invalid key: \"{key}\"");
        }
    }
}

[System.Serializable]
public class Command
{
    public string key;
    public UnityEvent events;
}
