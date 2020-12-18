using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Participant : MonoBehaviour
{
    [SerializeField] private List<RectTransform> alignment;
    [SerializeField] private GameObject fontPrefab;
    [SerializeField] private float spinSpeed;
    [SerializeField] private string filename;
    [SerializeField] private RewardUi rewardWindow;
    private float _yOri;
    private float _xOri;
    private List<string> _participant = new List<string>();
    private List<User> _users;

    private bool reqLock = true;
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        StartCoroutine(LoadUsers());
        while(reqLock)
            yield return new WaitForSeconds(0.1f);
        _yOri = GetComponent<RectTransform>().position.y;
        _xOri = GetComponent<RectTransform>().position.x;
        for (var i = 0 ; i != _participant.Count ; ++i)
        {
            GameObject o;
            if (i == 0)
            {
                o = alignment[0].gameObject;
            }
            else
            {
                o = Instantiate(fontPrefab, transform);
                var preRect = alignment[i - 1].GetComponent<RectTransform>().position;
                var height = alignment[i - 1].rect.height;
                print($"x:{preRect.x} y:{preRect.y}");
                o.GetComponent<RectTransform>().position = new Vector3(preRect.x, preRect.y + height, 0);
                alignment.Add(o.GetComponent<RectTransform>());
            }
            o.GetComponent<Text>().text = _participant[i];
        }
        yield break;
    }

    private IEnumerator LoadUsers()
    {
        var url = $"https://ujoj.cc/{filename}";
        var request = UnityWebRequest.Get(url);
        var download = new DownloadHandlerBuffer();
        request.downloadHandler = download;
        yield return request.SendWebRequest();
        var res = download.text;
        _users = JsonConvert.DeserializeObject<List<User>>(res);
        
        foreach (var ele in _users)
            _participant.Add(ele.Name);
        reqLock = false;
    }
    
    private int GetWinner()
    {
        var totalProb = _users.Sum(u => u.GetProb());
        var pickRes = Random.Range(0, totalProb);
        var idx = 0;
        
        print($"pickRes:{pickRes}");
        
        while (pickRes >= 0)
        {
            pickRes -= _users[idx].GetProb();
            idx++;
        }
        
        print($"idx:{idx}");
        
        // if needed to discard ticket
        _users[idx - 1].Score -= 100;
        
        return idx-1;
    }

    private IEnumerator Spinning(int winner)
    {
        print("start spin!!");
        var yDelta = alignment[0].rect.height * _participant.Count;
        var round = Random.Range(3, 10);
        var total = _participant.Count;

        float procedure;
        
        for (var r = 0; r != round; ++r)
        {
            procedure = 0;
            while (procedure < 1)
            {
                procedure += spinSpeed;
                GetComponent<RectTransform>().position += Vector3.down * (spinSpeed * yDelta);
                yield return new WaitForFixedUpdate();
            }
            GetComponent<RectTransform>().position = new Vector3(_xOri, _yOri);
        }
        
        procedure = 0;
        while (procedure < 1.0f / total)
        {
            procedure += spinSpeed;
            GetComponent<RectTransform>().position += Vector3.down * (spinSpeed * yDelta);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<RectTransform>().position = new Vector3(_xOri, _yOri - (winner * alignment[0].rect.height));
        print("end spin!!");
        rewardWindow.ClaimReward(_participant[winner]);
        rewardWindow.gameObject.SetActive(true);
    }

    private void ResetSpin()
    {
        GetComponent<RectTransform>().position = new Vector3(_xOri, _yOri);
    }
    public void Spin()
    {
        ResetSpin();
        var winner = GetWinner();
        StartCoroutine(Spinning(winner));
    }
}
