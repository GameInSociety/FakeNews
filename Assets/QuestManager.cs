using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<Quest> quests = new List<Quest>();

    public int questIndex;
    public Quest currentQuest;

    public bool finishedAllQuests = false;

    private void Awake() {
        Instance = this;
    }

    public void Start() {
        CurrentQuest_Setup();
    }

    public void CurrentQuest_Setup() {
        currentQuest = quests[questIndex];
        Menu.Instance.DisplayHelp("Allez voir la personne en charge");
        Menu.Instance.DisplayQuestName("Aucune photo en cours");
    }

    public void CurrentQuest_Start() {
        Menu.Instance.DisplayHelp(currentQuest.clue);
        Menu.Instance.DisplayQuestName(currentQuest.name);
        currentQuest.currentState = Quest.State.Doing;
    }

    public void CurrentQuest_Finish() {
        currentQuest.currentState = Quest.State.Done;
        ++questIndex;
        if( questIndex == quests.Count ) {
            finishedAllQuests = true;
            return;
        }

        currentQuest = quests[questIndex];
        CurrentQuest_Setup();
    }

    public void AddQuest(Quest quest) {
        quests.Add(quest);
    }
}

