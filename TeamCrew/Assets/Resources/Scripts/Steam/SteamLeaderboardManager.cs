using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

public class LeaderboardEntries
{
    public List<LeaderboardEntry> entries;
}
[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public int globalRank;
    public bool isClient;
    public bool isFriend;
    public Sprite avatar;
    public Timer timer;

    public LeaderboardEntry()
    {
        name = string.Empty;
        globalRank = 0;
        isClient = false;
        timer = new Timer();
        avatar = null;
    }
    public LeaderboardEntry(LeaderboardEntry_t steamLeaderboardEntry)
    {
        //Aquire steamID
        CSteamID steamID = steamLeaderboardEntry.m_steamIDUser;

        if (steamID.m_SteamID == 0)
        {
            this.name = "Unknown";
            this.avatar = null;
            this.globalRank = 0;
            this.isClient = false;
            this.isFriend = false;
            this.timer = new Timer();
        }
        else
        {
            this.name = SteamFriends.GetFriendPersonaName(steamID);
            this.avatar = GetAvatar(steamID);
            this.globalRank = steamLeaderboardEntry.m_nGlobalRank;
            this.isClient = steamID == SteamUser.GetSteamID();
            this.timer = new Timer(steamLeaderboardEntry.m_nScore);

            this.isFriend = SteamFriends.HasFriend(steamID, EFriendFlags.k_EFriendFlagImmediate);
        }
    }
    private Sprite GetAvatar(CSteamID steamid)
    {
        int FriendAvatar = SteamFriends.GetMediumFriendAvatar(steamid);

        uint ImageWidth;
        uint ImageHeight;
        bool ret = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

        if (ret && ImageWidth > 0 && ImageHeight > 0)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            ret = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
            Texture2D m_MediumAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            m_MediumAvatar.LoadRawTextureData(Image);
            m_MediumAvatar.Apply();

            return Sprite.Create(m_MediumAvatar, new Rect(0, 0, ImageWidth, ImageHeight), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
}
public class SteamLeaderboardManager : MonoBehaviour
{
    public delegate void OnEntriesComplete();
    public delegate void OnEntriesFailed();

    public delegate void OnUploadComplete();
    public delegate void OnUploadFailed();

    private OnEntriesFailed onEntriesFailedStack;
    private OnEntriesComplete onEntriesCompleteStack;

    private OnUploadComplete onUploadCompleteStack;
    private OnUploadFailed onUploadFailedStack;

    private bool gettingLeaderboardEntries;

	private SteamLeaderboard_t m_SteamLeaderboard;
	private SteamLeaderboardEntries_t m_SteamLeaderboardEntries;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	private CallResult<UserStatsReceived_t> UserStatsReceived;
	private CallResult<LeaderboardFindResult_t> LeaderboardFindResult;
	private CallResult<LeaderboardScoresDownloaded_t> LeaderboardScoresDownloaded;
	private CallResult<LeaderboardScoreUploaded_t> LeaderboardScoreUploaded;
	private CallResult<NumberOfCurrentPlayers_t> NumberOfCurrentPlayers;


    private LeaderboardEntries entriesRef;

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

    //Find leaderboard
    public void FindLeaderboard(string leaderboardName)
    {
        //Debug.Log("Trying to find a leaderboard called: " + leaderboardName);
        //SteamAPICall_t handle = SteamUserStats.FindLeaderboard(leaderboardName);
        //LeaderboardFindResult.Set(handle);

        Debug.Log("Trying to find a leaderboard called: " + leaderboardName);
        SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(leaderboardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
        LeaderboardFindResult.Set(handle);
    } 
    private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bLeaderboardFound != 0)
        {
            Debug.Log("Success!! Found a leaderboard, leaderboard: " + pCallback.m_hSteamLeaderboard);
            m_SteamLeaderboard = pCallback.m_hSteamLeaderboard;


            //Continue with downloading scores
            DownloadLeaderboard();
        }
        else
        {
            Debug.Log("Failure!! Could not find a leaderboard");
            FailedToGetLeaderboardEntries();
        }
    }

    //Download leaderboard entries
    private void DownloadLeaderboard()
    {
        if (m_SteamLeaderboard.ToString() != "0")
        {
            Debug.Log("Downloading highscores from leaderboard: " + m_SteamLeaderboard);
            SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(m_SteamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, int.MaxValue);
            LeaderboardScoresDownloaded.Set(handle);
        }
        else
        {
            Debug.Log("Failure!! Cannot download highscores, there is no leaderboard attached");
            FailedToGetLeaderboardEntries();
        }
    }
    private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (m_SteamLeaderboard.ToString() != "0")
        {
            Debug.Log("Success!! Downloaded leaderboard from steamservers with " + pCallback.m_cEntryCount + " entries.");
            m_SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;

            GetLeaderboardEntriesLocal();
        }
        else
        {
            Debug.Log("Failure!! Could not download leaderboard scores");
            FailedToGetLeaderboardEntries();
        }
    }

    //Aquire leaderboard entries
    public void GetLeaderboardEntries(string leaderboardName, LeaderboardEntries entriesRef, OnEntriesComplete completeMethod, OnEntriesFailed failedMethod)
    {
        if (!SteamManager.Initialized)
        {
            if (failedMethod != null)
                failedMethod();

            return;
        }

        if (!gettingLeaderboardEntries)
        {
            onEntriesCompleteStack = null;
            onEntriesFailedStack = null;

            onEntriesCompleteStack += completeMethod;
            onEntriesFailedStack += failedMethod;

            this.entriesRef = entriesRef;
            gettingLeaderboardEntries = true;
            FindLeaderboard(leaderboardName);
        }
    }
    private void GetLeaderboardEntriesLocal()
    {
        if (m_SteamLeaderboard.ToString() != "0" && entriesRef != null)
        {
            int entryCount = SteamUserStats.GetLeaderboardEntryCount(m_SteamLeaderboard);
            for (int i = 0; i < entryCount; i++)
            {
                LeaderboardEntry_t leaderboardEntry;
                bool result = SteamUserStats.GetDownloadedLeaderboardEntry(m_SteamLeaderboardEntries, i, out leaderboardEntry, null, 0);
                if (!result)
                {
                    Debug.Log("Entry[" + i.ToString() + "] Could not be retrieved from leaderboard: " + m_SteamLeaderboard.ToString());
                }
                else
                {
                }
                entriesRef.entries.Add(new LeaderboardEntry(leaderboardEntry));
            }

            SuccededToGetLeaderboardEntries();
        }
        else
        {
            Debug.Log("Cannot aquire leaderboard entries, there is no leaderboard attached");
        }
    }
    private void SuccededToGetLeaderboardEntries()
    {
        if (onEntriesCompleteStack != null)
        {
            onEntriesCompleteStack();
        }
        gettingLeaderboardEntries = false;
    }
    public void FailedToGetLeaderboardEntries()
    {
        gettingLeaderboardEntries = false;
        
        if (onEntriesFailedStack != null)
        {
            onEntriesFailedStack();
        }
    }
    public void ResetGettingLeaderboards()
    {
        gettingLeaderboardEntries = false;
    }

    //Upload time to leaderboard
    public void UploadTimeToLeaderboard(int timeInMilliSeconds, OnUploadComplete completeMethod, OnUploadFailed failedMethod)
    {
        if (completeMethod != null)
        {
            onUploadCompleteStack = null;
            onUploadFailedStack = null;

            onUploadCompleteStack += completeMethod;
            onUploadFailedStack += failedMethod;
        }
        Debug.Log("Uploading time to leaderboard: " + m_SteamLeaderboard.ToString() + "   With time: " + timeInMilliSeconds.ToString() + " ms");
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(m_SteamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, timeInMilliSeconds, null, 0);
        LeaderboardScoreUploaded.Set(handle);
    }
    private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess != 0)
        {
            Debug.Log("Success!! Uploaded time to leaderboard: " + pCallback.m_hSteamLeaderboard);

            if (onUploadCompleteStack != null)
            {
                onUploadCompleteStack();
            }
        }
        else
        {
            if (onUploadFailedStack != null)
            {
                onUploadFailedStack();
            }
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
