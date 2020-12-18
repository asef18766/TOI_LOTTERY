using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class User
{
    [JsonProperty] public string Name;
    [JsonProperty] public int Score;

    public int GetProb()
    {
        return Score / 100;
    }
}