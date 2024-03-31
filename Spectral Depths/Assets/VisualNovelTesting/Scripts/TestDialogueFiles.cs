using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.TopDown;
using SpectralDepths.Tools;


public class TestDialogueFiles : MonoBehaviour, PLEventListener<VNEvent>, PLEventListener<TopDownEngineEvent>
{
    [SerializeField] private TextAsset fileToRead = null;
    public Canvas VNCanvas;
    private bool _runVN = true;
    // Start is called before the first frame update
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(fileToRead);

        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrWhiteSpace(line))
        //        continue;

        //    DIALOGUE_LINE dl = DialogueParser.Parse(line);

        //    for (int i = 0; i < dl.commandData.commands.Count; i++)
        //    {
        //        DL_COMMAND_DATA.Command command = dl.commandData.commands[i];
        //        Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
        //    }

        //}
        if(_runVN) DialogueSystem.instance.Say(lines);
    }

    public virtual void OnMMEvent(VNEvent engineEvent)
    {
        switch (engineEvent.EventType)
        {
            case VNEventTypes.ChangeVNScene:
                ChangeVNScene(engineEvent.FileToRead);
                break;
            case VNEventTypes.DisableVNScene:
                DisableVNScene();
                break;
        }
    }

    public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
    {
        switch (engineEvent.EventType)
        {
            case TopDownEngineEventTypes.SwitchToGameMode:
                DisableVNScene();
                break;
        }
    }

    void DisableVNScene()
    {
        _runVN = false;
        VNCanvas.gameObject.SetActive(false);
    }

    void ChangeVNScene(TextAsset fileToRead)
    {
        _runVN = true;
        this.fileToRead = fileToRead;
        VNCanvas.gameObject.SetActive(true);
        StartConversation();
    }

    protected virtual void OnEnable()
    {
        this.PLEventStartListening<VNEvent> ();
        this.PLEventStartListening<TopDownEngineEvent> ();

    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.PLEventStopListening<VNEvent> ();
        this.PLEventStopListening<TopDownEngineEvent> ();

    }
}
