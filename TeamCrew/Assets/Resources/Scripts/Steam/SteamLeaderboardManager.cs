using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

public struct LeaderboardEntries
{
    public List<LeaderboardEntry> entries;
}
[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public int globalRank;
    public int totalSeconds;
    public bool isClient;

    public LeaderboardEntry()
    {
        name = string.Empty;
        globalRank = 0;
        totalSeconds = 0;
        isClient = false;
    }
    public LeaderboardEntry(LeaderboardEntry_t steamLeaderboardEntry)
    {
        this.name = SteamFriends.GetFriendPersonaName(steamLeaderboardEntry.m_steamIDUser);
        this.globalRank = steamLeaderboardEntry.m_nGlobalRank;
        this.totalSeconds = steamLeaderboardEntry.m_nScore;
        this.isClient = steamLeaderboardEntry.m_steamIDUser == SteamUser.GetSteamID();
    }
}
public class SteamLeaderboardManager : MonoBehaviour
{
	private int m_NumGamesStat;
	private float m_FeetTraveledStat;
	private bool m_AchievedWinOneGame;
	private SteamLeaderboard_t m_SteamLeaderboard;
	private SteamLeaderboardEntries_t m_SteamLeaderboardEntries;
	private Texture2D m_Icon;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	private CallResult<UserStatsReceived_t> UserStatsReceived;
	private CallResult<LeaderboardFindResult_t> LeaderboardFindResult;
	private CallResult<LeaderboardScoresDownloaded_t> LeaderboardScoresDownloaded;
	private CallResult<LeaderboardScoreUploaded_t> LeaderboardScoreUploaded;
	private CallResult<NumberOfCurrentPlayers_t> NumberOfCurrentPlayers;

	public void OnEnable() 
    {
		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);

		UserStatsReceived = CallResult<UserStatsReceived_t>.Create(OnUserStatsReceived);
		LeaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
		LeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
		LeaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
		NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FindLeaderboard("Highscores", 0f));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UploadTimeToLeaderboard(1337, 0f));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(DownloadLeaderboard(0));
        }
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    StartCoroutine(GetLeaderboardEntries(0));
        //}
    }

    //Find leaderboard
    private IEnumerator FindLeaderboard(string leaderboardName, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        Debug.Log("Trying to find a leaderboard called: " + leaderboardName);
        SteamAPICall_t handle = SteamUserStats.FindLeaderboard(leaderboardName);
        LeaderboardFindResult.Set(handle);
    } 
    private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bLeaderboardFound != 0)
        {
            Debug.Log("Success!! Found a leaderboard, leaderboard: " + pCallback.m_hSteamLeaderboard);
            m_SteamLeaderboard = pCallback.m_hSteamLeaderboard;
        }
        else
        {
            Debug.Log("Failure!! Could not find a leaderboard");
        }
    }

    //Download leaderboard entries
    private IEnumerator DownloadLeaderboard(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        if (m_SteamLeaderboard.ToString() != "0")
        {
            Debug.Log("Downloading highscores from leaderboard: " + m_SteamLeaderboard);
            SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(m_SteamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 5);
            LeaderboardScoresDownloaded.Set(handle);
        }
        else
        {
            Debug.Log("Cannot download highscores, there is no leaderboard attached");
        }
    }
    private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (m_SteamLeaderboard.ToString() != "0")
        {
            Debug.Log("Success!! Downloaded leaderboard from steamservers with " + pCallback.m_cEntryCount + " entries.");
            m_SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;
        }
    }

    //Aquire leaderboard entries
    public IEnumerator GetLeaderboardEntries(float timeDelay, LeaderboardEntries entriesRef)
    {
        yield return new WaitForSeconds(timeDelay);

        if (m_SteamLeaderboard.ToString() == "0")
        {
            StartCoroutine(FindLeaderboard("Highscores", 0f));
            StartCoroutine(DownloadLeaderboard(1f));
            yield return new WaitForSeconds(2);
        }

        if (m_SteamLeaderboard.ToString() != "0")
        {
            int entryCount = SteamUserStats.GetLeaderboardEntryCount(m_SteamLeaderboard);
            for (int i = 0; i < entryCount; i++)
            {
                LeaderboardEntry_t leaderboardEntry;
                bool result = SteamUserStats.GetDownloadedLeaderboardEntry(m_SteamLeaderboardEntries, i, out leaderboardEntry, null, 0);
                if (!result)
                {
                    Debug.Log("Entry[" + i.ToString() + "] Could not be retrieved, try downloading leaderboard first\n");
                }
                entriesRef.entries.Add(new LeaderboardEntry(leaderboardEntry));
            }
        }
        else
        {
            Debug.Log("Cannot aquire leaderboard entries, there is no leaderboard attached");
        }
    }

    //Upload time to leaderboard
    public IEnumerator UploadTimeToLeaderboard(int timeInSeconds, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        Debug.Log("Uploading time to leaderboard: " + m_SteamLeaderboard.ToString() + "   With time: " + timeInSeconds.ToString());
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(m_SteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, timeInSeconds, null, 0);
        LeaderboardScoreUploaded.Set(handle);
    }
    private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 0)
        {
            Debug.Log("Success!! Uploaded time to leaderboard: " + pCallback.m_hSteamLeaderboard);
        }
        else
        {
            Debug.Log("Failure!! Could not upload time to leaderboard");
        }
    }

    /// <summary>
    /// Retrieves the number of players playing your game, offline + online
    /// </summary>
    private void GetNumberOfPlayers()
    {
        SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
        NumberOfCurrentPlayers.Set(handle);
    }
    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 0)
        {
            Debug.Log("Number of players in Frog Climbers: " + pCallback.m_cPlayers);
        }
    }












	// Callback version for: SteamUserStats.RequestCurrentStats() (Local Player)
	private void OnUserStatsReceived(UserStatsReceived_t pCallback) 
    {
		Debug.Log("[" + UserStatsReceived_t.k_iCallback + " - UserStatsReceived] - " + pCallback.m_nGameID + " -- " + pCallback.m_eResult + " -- " + pCallback.m_steamIDUser);
	}
	// CallResult version for: SteamUserStats.RequestUserStats() (Other Players)
	private void OnUserStatsReceived(UserStatsReceived_t pCallback, bool bIOFailure) {
		Debug.Log("[" + UserStatsStored_t.k_iCallback + " - UserStatsReceived] - " + pCallback.m_nGameID + " -- " + pCallback.m_eResult + " -- " + pCallback.m_steamIDUser);
	}
	private void OnUserStatsStored(UserStatsStored_t pCallback) {
		Debug.Log("[" + UserStatsStored_t.k_iCallback + " - UserStatsStored] - " + pCallback.m_nGameID + " -- " + pCallback.m_eResult);
	}
	private void OnUserAchievementStored(UserAchievementStored_t pCallback) {
		Debug.Log("[" + UserAchievementStored_t.k_iCallback + " - UserAchievementStored] - " + pCallback.m_nGameID + " -- " + pCallback.m_bGroupAchievement + " -- " + pCallback.m_rgchAchievementName + " -- " + pCallback.m_nCurProgress + " -- " + pCallback.m_nMaxProgress);
	}
}
