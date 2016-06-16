using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

[System.Serializable]
public class HighscoreEntry
{
    public ulong ID;
    public int score;

    public HighscoreEntry()
    {

    }
    public HighscoreEntry(ulong ID, int score)
    {
        this.ID = ID;
        this.score = score;
    }
}
public class DatabaseManager : MonoBehaviour 
{
	//publics
    public string getURL = "http://localhost/testdatabase/gethighscore.php";
    public string insertURL = "http://localhost/testdatabase/inserthighscore.php";
    public string removeURL = "http://localhost/testdatabase/removehighscore.php";
    public List<HighscoreEntry> highscoreEntries = new List<HighscoreEntry>();

	//privates

	//Unity methods
	void Start () 
	{
        HighscoreEntry localuserEntry = new HighscoreEntry();
        localuserEntry.ID = SteamUser.GetSteamID().m_SteamID;
        localuserEntry.score = Random.Range(1, 2000);


        StartCoroutine(InsertHighscoreCoroutine(localuserEntry));
        Invoke("AquireHighscores", 0.5f);
	}
	void Update () 
	{
	
	}

	//public methods

	//private methods
    private void FormatDataToEntries(string data)
    {
        string[] entries = data.Split('#');
        int variableCount = 2;

        for(int i = 0; i < entries.Length; i++)
        {
            string[] vars = entries[i].Split('|');

            if (vars.Length >= variableCount)
            {
                //Aquire variables as strings
                string id = vars[0];
                string score = vars[1];

                if (
                        string.IsNullOrEmpty(id) || 
                        string.IsNullOrEmpty(score)
                    )
                {
                    continue;
                }

                //Create a new highscoreEntry
                HighscoreEntry entry = new HighscoreEntry();

                //Parse to int datatype
                entry.ID = ulong.Parse(id);
                entry.score = int.Parse(score);

                //Add to highscorelist
                highscoreEntries.Add(entry);
            }
        }
    }
    private void AddVariableToURL(ref string url, string variableName, string value)
    {
        if (!url.Contains("?"))
        {
            url += "?";
        }
        else
        {
            url += "&";
        }

        url += variableName + "=" + value;
    }



    //Aquire highscores to highscoreEntries variable
    private void AquireHighscores()
    {
        StartCoroutine(AquireURLText());
    }
    private IEnumerator AquireURLText()
    {
        WWW data = new WWW(getURL);
        yield return data;

        string text = data.text;
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("Cannot connect to highscore URL: " + data.error);
            yield return null;
        }

        FormatDataToEntries(text);
    }

    //Insert highscore
    private void InsertHighScore(HighscoreEntry entry)
    {
        StartCoroutine(InsertHighscoreCoroutine(entry));
    }
    private IEnumerator InsertHighscoreCoroutine(HighscoreEntry entry)
    {
        //Remove any previous entries with the same ID
        yield return RemoveHighscoreCoroutine(entry.ID);

        //Insert new highscore entry
        string finalURL = insertURL;

        AddVariableToURL(ref finalURL, "id", entry.ID.ToString());
        AddVariableToURL(ref finalURL, "score", entry.score.ToString());

        WWW www = new WWW(finalURL);
    }

    //Remove highscore
    private void RemoveHighscore(ulong ID)
    {
        StartCoroutine(RemoveHighscoreCoroutine(ID));
    }
    private IEnumerator RemoveHighscoreCoroutine(ulong ID)
    {
        string finalURL = removeURL;

        AddVariableToURL(ref finalURL, "id", ID.ToString());

        WWW www = new WWW(finalURL);
        yield return www;
    }


}
