using System.Collections.Generic;
using UnityEngine;

public class EChapter : MonoBehaviour
{
    public int id;
    public string title;
    public string bGImagePath;
    public int rewardId;
    public Dictionary<int,int> questDict = new Dictionary<int, int>();
}
