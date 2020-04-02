using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeDirector : MonoBehaviour
{
   public static NarrativeDirector Instance;
   public GameObject storyBook;
   public GameObject eChapterPrefab;
   public GameObject eTaskPrefab;
   public GameObject eFragmentPrefab;
   public GameObject eDialoguePrefab;
   public GameObject nextButton;
   public GameObject backButton;
   public GameObject char1DialogueText;
   public GameObject char2DialogueText;
   public GameObject char1NameText;
   public GameObject char2NameText;
   public GameObject playHUD;
   public GameObject storyboard;
   public GameObject playButton;
   public GameObject mainMenuPanel;
   public GameObject chapterSelectBox;

   public EChapter currentChapter;
   public ETask currentTask;
   public EFragment currentFragment;
   public EDialogue currentDialogue;
   
   private List<Chapter> _chapters = new List<Chapter>();
   private List<Task> _tasks = new List<Task>();
   private List<Fragment> _fragments = new List<Fragment>();
   private List<Dialogue> _dialogues = new List<Dialogue>();
   public Dictionary<int, EChapter> chapterList = new Dictionary<int, EChapter>();
   public Dictionary<int, EFragment> fragmentList = new Dictionary<int, EFragment>();
   public Dictionary<int, EDialogue> dialogueList = new Dictionary<int, EDialogue>();
   public Dictionary<int,ETask> taskList = new Dictionary<int, ETask>();

   private void Awake()
   {
      Init();
   }
   
   

   private void Init()
   {
      ReadAllData();
      CreateAllGameObjects();
      OrganiseObjectHierarchy();

      var selection = chapterSelectBox.GetComponent<Dropdown>();
      var options = new List<string>();
      
      selection.options.Clear();
      
      foreach (var key in chapterList.Keys)
      {
         options.Add(key.ToString());
      }
      
      selection.AddOptions(options);
      
   }

   private void OrganiseObjectHierarchy()
   {
      foreach (var frag in fragmentList.Values)
      {
         for (int i = 0; i < frag.dialogues.Length; i++)
         {
            dialogueList[frag.dialogues[i].id].transform.SetParent(frag.transform);
         }
      }

      foreach (var chapter in chapterList.Values)
      {
         foreach (var pair in chapter.questDict)
         {
            taskList[pair.Key].transform.SetParent(chapter.transform);
            fragmentList[pair.Value].transform.SetParent(chapter.transform);
         }
         
         chapter.transform.SetParent(storyBook.transform);
      }
   }

   private void CreateAllGameObjects()
   {

      foreach (var dialogueData in _dialogues)
      {
         CreateEDialogue(dialogueData);
      }

      foreach (var taskData in _tasks)
      {
         CreateETask(taskData);
      }

      foreach (var fragData in _fragments)
      {
         CreateEFragment(fragData);
      }

      foreach (var chapterData in _chapters)
      {
         CreateEChapter(chapterData);
      }
      
   }

   private void ReadAllData()
   {
      ReadChapterData();
      ReadTaskData();
      ReadFragmentData();
      ReadDialogueData();
   }

   private void ReadFragmentData()
   {
      var files = Directory.GetFiles("Assets/JSON/Fragments", "*.json", SearchOption.AllDirectories);
      _fragments.Clear();

      foreach (var jsonFile in files)
      {
         var json = File.ReadAllText(jsonFile);
         var nd = JsonUtility.FromJson<Fragment>(json);
         _fragments.Add(nd);
      }
   }

   private void ReadTaskData()
   {
      var files = Directory.GetFiles("Assets/JSON/Tasks", "*.json", SearchOption.AllDirectories);
      _tasks.Clear();

      foreach (var jsonFile in files)
      {
         var json = File.ReadAllText(jsonFile);
         var nd = JsonUtility.FromJson<Task>(json);
         _tasks.Add(nd);
      }
   }

   private void ReadDialogueData()
   {
      var files = Directory.GetFiles("Assets/JSON/Dialogues", "*.json", SearchOption.AllDirectories);
      _dialogues.Clear();

      foreach (var jsonFile in files)
      {
         var json = File.ReadAllText(jsonFile);
         var nd = JsonUtility.FromJson<Dialogue>(json);
         _dialogues.Add(nd);
      }

   }

   private void ReadChapterData()
   {
      var files = Directory.GetFiles("Assets/JSON/Chapters", "*.json", SearchOption.AllDirectories);
      _chapters.Clear();
      
      foreach (var jsonFile in files)
      {
         var json = File.ReadAllText(jsonFile);
         var nd = JsonUtility.FromJson<Chapter>(json);
         _chapters.Add(nd);
      }
   }

   private void CreateEChapter(Chapter chapterData)
   {
      var newObj = Instantiate(eChapterPrefab, storyBook.transform.position, Quaternion.identity);

         var newChap = newObj.GetComponent<EChapter>();
         
         newChap.id = chapterData.id;
         newChap.title = chapterData.title;
         newChap.bGImagePath = chapterData.bGImagePath;
         newChap.rewardId = chapterData.rewardId;
         newChap.transform.name = $"{newChap.id}. {newChap.title}";

         foreach (var quest in chapterData.quests)
         {
            newChap.questDict.Add(quest.taskId,quest.storyFragmentId);
         }
         
         chapterList.Add(newChap.id, newChap);
   }

   private void CreateETask(Task taskData)
   {
      var newETask = Instantiate(eTaskPrefab).GetComponent<ETask>();

         newETask.id = taskData.id;
         newETask.title = taskData.title;
         newETask.objectiveIds = taskData.objectiveIds;
         newETask.transform.name = newETask.title;
         
         taskList.Add(newETask.id, newETask);
   }

   private void CreateEDialogue(Dialogue diaData)
   {
      var newEDialogue = Instantiate(eDialoguePrefab).GetComponent<EDialogue>();

         newEDialogue.id = diaData.id;
         newEDialogue.charId = diaData.charId;
         newEDialogue.poseId = diaData.poseId;
         newEDialogue.text = diaData.text;
         newEDialogue.transform.name = newEDialogue.id.ToString();
         
         dialogueList.Add(newEDialogue.id,newEDialogue);
   }

   private void CreateEFragment(Fragment fragData)
   {
      var newEFrag = Instantiate(eFragmentPrefab).GetComponent<EFragment>();

      newEFrag.id = fragData.id;
      newEFrag.imagePath = fragData.imagePath;
      newEFrag.dialogues = GetDialogues(fragData.dialogueIds);
      newEFrag.transform.name = fragData.id.ToString();

      fragmentList.Add(newEFrag.id, newEFrag);
      
   }

   private EDialogue[] GetDialogues(int[] ids)
   {
      var dialogueArray = new EDialogue[ids.Length];
      
      for (var i = 0; i< ids.Length; i++)
      {
         dialogueArray[i] = dialogueList[ids[i]];
      }
      return dialogueArray;
   }

   public void PlayButtonAction()
   {
      var dropBox = chapterSelectBox.GetComponent<Dropdown>();
      var chapterId = Convert.ToInt32(dropBox.options[dropBox.value].text);
      PlayChapter(chapterList[chapterId]);
   }
   
   public void PlayChapter(EChapter chapter)
   {
      currentChapter = chapter;
      currentTask = taskList[chapter.questDict.Keys.First()];
      currentFragment = fragmentList[chapter.questDict[chapter.questDict.Keys.First()]];
      currentDialogue = currentFragment.dialogues[0];
      
      mainMenuPanel.SetActive(false);
      playHUD.SetActive(true);
      storyboard.SetActive(true);
   }
}
