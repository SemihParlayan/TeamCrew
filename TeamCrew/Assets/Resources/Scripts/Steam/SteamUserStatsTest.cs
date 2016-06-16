using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

public class SteamUserStatsTest : MonoBehaviour {
	private int m_NumGamesStat;
	private float m_FeetTraveledStat;
	private bool m_AchievedWinOneGame;
	private SteamLeaderboard_t m_SteamLeaderboard;
	private SteamLeaderboardEntries_t m_SteamLeaderboardEntries;
	private Texture2D m_Icon;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;
	protected Callback<UserStatsUnloaded_t> m_UserStatsUnloaded;
	protected Callback<UserAchievementIconFetched_t> m_UserAchievementIconFetched;

	private CallResult<UserStatsReceived_t> UserStatsReceived;
	private CallResult<LeaderboardFindResult_t> LeaderboardFindResult;
	private CallResult<LeaderboardScoresDownloaded_t> LeaderboardScoresDownloaded;
	private CallResult<LeaderboardScoreUploaded_t> LeaderboardScoreUploaded;
	private CallResult<LeaderboardUGCSet_t> LeaderboardUGCSet;
	private CallResult<NumberOfCurrentPlayers_t> NumberOfCurrentPlayers;
	private CallResult<GlobalAchievementPercentagesReady_t> GlobalAchievementPercentagesReady;
	private CallResult<GlobalStatsReceived_t> GlobalStatsReceived;

	public void OnEnable() {
		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);
		m_UserStatsUnloaded = Callback<UserStatsUnloaded_t>.Create(OnUserStatsUnloaded);
		m_UserAchievementIconFetched = Callback<UserAchievementIconFetched_t>.Create(OnUserAchievementIconFetched);

		UserStatsReceived = CallResult<UserStatsReceived_t>.Create(OnUserStatsReceived);
		LeaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
		LeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
		LeaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
		NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
		GlobalAchievementPercentagesReady = CallResult<GlobalAchievementPercentagesReady_t>.Create(OnGlobalAchievementPercentagesReady);
		LeaderboardUGCSet = CallResult<LeaderboardUGCSet_t>.Create(OnLeaderboardUGCSet);
		GlobalStatsReceived = CallResult<GlobalStatsReceived_t>.Create(OnGlobalStatsReceived);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FindLeaderboard("Highscores");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UploadTimeToLeaderboard(1337);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DownloadLeaderboard();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetLeaderboardEntries();
        }
    }

    //Find leaderboard
    private void FindLeaderboard(string leaderboardName)
    {
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
    private void DownloadLeaderboard()
    {
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
    private List<LeaderboardEntry_t> GetLeaderboardEntries()
    {
        List<LeaderboardEntry_t> entries = new List<LeaderboardEntry_t>();

        if (m_SteamLeaderboard.ToString() != "0")
        {
            int entryCount = SteamUserStats.GetLeaderboardEntryCount(m_SteamLeaderboard);
            for (int i = 0; i < entryCount; i++)
            {
                LeaderboardEntry_t leaderboardEntry;
                bool result = SteamUserStats.GetDownloadedLeaderboardEntry(m_SteamLeaderboardEntries, i, out leaderboardEntry, null, 0);
                if (result)
                {
                    Debug.Log("Entry[" + i.ToString() +"]\n" + "Username: " + SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser) + ", Score: " + leaderboardEntry.m_nScore + ", Rank: " + leaderboardEntry.m_nGlobalRank);
                }
                else
                {
                    Debug.Log("Entry[" + i.ToString() +"] Could not be retrieved, try downloading leaderboard first\n");
                }
                entries.Add(leaderboardEntry);
            }
            if (entries.Count > 0)
                return entries;
            else
                return null;
        }
        else
        {
            Debug.Log("Cannot aquire leaderboard entries, there is no leaderboard attached");
            return null;
        }
    }

    //Upload time to leaderboard
    private void UploadTimeToLeaderboard(int timeInSeconds)
    {
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
        print("GetNumberOfCurrentPlayers() - " + handle);
    }

	// Callback version for: SteamUserStats.RequestCurrentStats() (Local Player)
	private void OnUserStatsReceived(UserStatsReceived_t pCallback) {
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

	

	private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure) {
		Debug.Log("[" + NumberOfCurrentPlayers_t.k_iCallback + " - NumberOfCurrentPlayers] - " + pCallback.m_bSuccess + " -- " + pCallback.m_cPlayers);
	}

	private void OnUserStatsUnloaded(UserStatsUnloaded_t pCallback) {
		Debug.Log("[" + UserStatsUnloaded_t.k_iCallback + " - UserStatsUnloaded] - " + pCallback.m_steamIDUser);
	}

	private void OnUserAchievementIconFetched(UserAchievementIconFetched_t pCallback) {
		Debug.Log("[" + UserAchievementIconFetched_t.k_iCallback + " - UserAchievementIconFetched_t] - " + pCallback.m_nGameID + " -- " + pCallback.m_rgchAchievementName + " -- " + pCallback.m_bAchieved + " -- " + pCallback.m_nIconHandle);
	}

	private void OnGlobalAchievementPercentagesReady(GlobalAchievementPercentagesReady_t pCallback, bool bIOFailure) {
		Debug.Log("[" + GlobalAchievementPercentagesReady_t.k_iCallback + " - GlobalAchievementPercentagesReady] - " + pCallback.m_nGameID + " -- " + pCallback.m_eResult);
	}

	private void OnLeaderboardUGCSet(LeaderboardUGCSet_t pCallback, bool bIOFailure) {
		Debug.Log("[" + LeaderboardUGCSet_t.k_iCallback + " - LeaderboardUGCSet] - " + pCallback.m_eResult + " -- " + pCallback.m_hSteamLeaderboard);
	}
	
	private void OnGlobalStatsReceived(GlobalStatsReceived_t pCallback, bool bIOFailure) {
		Debug.Log("[" + GlobalStatsReceived_t.k_iCallback + " - GlobalStatsReceived] - " + pCallback.m_nGameID + " -- " + pCallback.m_eResult);
	}
}
