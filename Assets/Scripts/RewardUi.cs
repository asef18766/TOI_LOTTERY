using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUi : MonoBehaviour
{
    [SerializeField] private List<string> rewardType;
    [SerializeField] private List<string> reward;

    [SerializeField] private Text usernameTxt;
    [SerializeField] private Text rewardTypeTxt;
    [SerializeField] private Text rewardTxt;

    private int _rewardCount;
    private static readonly int JumpOutTrigger = Animator.StringToHash("JumpOutTrigger");

    private void OnEnable()
    {
        GetComponent<Animator>().SetTrigger(JumpOutTrigger);
    }
    public void ClaimReward(string name)
    {
        usernameTxt.text = name + " 同學";
        rewardTypeTxt.text = "獲得 " + rewardType[_rewardCount];
        rewardTxt.text = reward[_rewardCount];
        _rewardCount++;
    }

    public void Cancel()
    {
        _rewardCount--;
    }
}