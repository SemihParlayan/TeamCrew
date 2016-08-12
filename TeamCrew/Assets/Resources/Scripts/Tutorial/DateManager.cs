using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.UI;

public class DateManager : MonoBehaviour 
{
	//publics
    public static System.DateTime CurrentUTCDate;
    public static bool hasValidDate;

    public TextMesh[] timeLeftTexts;
    public GameObject noInternetConnectionParent;

	//privates
    private static GameObject noInternetConnectionParentStatic;
    private GameManager gameManager;
    private static SteamLeaderboardManager leaderboardManager;
    private static Timer dailyTimer;
    private static bool calledFirstLeaderboardFind;
    private static bool calledReset;
    public static bool searchingForInternet;
    private float searchInternetTimer;

	//Unity methods
    void Awake()
    {
        noInternetConnectionParentStatic = noInternetConnectionParent;

        gameManager = GameObject.FindObjectOfType<GameManager>();
        leaderboardManager = GameObject.FindObjectOfType<SteamLeaderboardManager>();

        Initialize();
    }
    void Start()
    {
        if (SteamManager.Initialized)
        {
            ResetLeaderboards();
            calledFirstLeaderboardFind = true;
        }
    }
    void Update()
    {
        if (gameManager.gameActive)
            return;

        if (dailyTimer != null)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                dailyTimer.AddHours(-1);
            }
            dailyTimer.Decrement();
            foreach (TextMesh text in timeLeftTexts)
            {
                text.text = dailyTimer.GetTimeString(true, true, true, false);
            }

            //Check if timer reaches zero
            if (dailyTimer.IsAtZero())
            {
                if (!calledReset)
                {
                    calledReset = true;
                    Invoke("ResetLeaderboards", 1f);
                }
            }
        }



        //Search for internet
        if (searchingForInternet)
        {
            searchInternetTimer += Time.deltaTime;
            if (searchInternetTimer >= 5f)
            {
                searchInternetTimer -= 5f;

                Initialize();
            }
        }
    }

	//public methods
    public static void RefreshUTCDate()
    {
        CurrentUTCDate = GetNetworkTime();
        hasValidDate = (CurrentUTCDate.Year != 1);
        Debug.Log("Refresing date... Valid date = " + hasValidDate);
    }
    public static string GetDateString()
    {
        string text = string.Empty;

        text += CurrentUTCDate.DayOfWeek + " - ";
        text += CurrentUTCDate.Day + "/";
        text += CurrentUTCDate.Month + "/";
        text += CurrentUTCDate.Year;

        return text;
    }
    public static string GetDateString(int offsetInDays)
    {
        DateTime newTime = CurrentUTCDate.AddDays(offsetInDays);
        string text = string.Empty;

        text += newTime.DayOfWeek + " - ";
        text += newTime.Day + "/";
        text += newTime.Month + "/";
        text += newTime.Year;

        return text;
    }
    public static int GetSeedFromUTC()
    {
        string textSeed = CurrentUTCDate.Year.ToString() + CurrentUTCDate.Month.ToString() + CurrentUTCDate.Day.ToString();
        return int.Parse(textSeed);
    }
    public static int GetSeedFromUTC(int offsetInDays)
    {
        DateTime newTime = CurrentUTCDate.AddDays(offsetInDays);
        string textSeed = newTime.Year.ToString() + newTime.Month.ToString() + newTime.Day.ToString();
        return int.Parse(textSeed);
    }
    public static Timer GetDailyTimer()
    {
        return dailyTimer;
    }
	//private methods
    //http://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c/12150289#12150289
    private static System.DateTime GetNetworkTime()
    {
        //default Windows time server
        const string ntpServer = "time.windows.com";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        IPAddress[] addresses = null;
        try
        {
            addresses = Dns.GetHostEntry(ntpServer).AddressList;
        }
        catch
        {

        }

        if (addresses == null)
        {
            NoInternetConnection();
        }
        else
        {
            searchingForInternet = false;
            noInternetConnectionParentStatic.gameObject.SetActive(false);

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);


            return networkDateTime;
        }

        return new System.DateTime();
    }
    private static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
    private static Timer GetTimeLeftForToday()
    {
        if (!hasValidDate)
            return null;

        int hoursLeft = 23 - CurrentUTCDate.Hour;
        int minutesLeft = 59 - CurrentUTCDate.Minute;
        int secondsleft = 59 - CurrentUTCDate.Second; 
        int milliSecondsLeft = 1000 - CurrentUTCDate.Millisecond;

        Timer timer = new Timer();
        timer.AddHours(hoursLeft);
        timer.AddMinutes(minutesLeft);
        timer.AddSeconds(secondsleft);
        timer.AddMilliSeconds(milliSecondsLeft / 10);
        return timer;
    }

    private static void Initialize()
    {
        RefreshUTCDate();
        dailyTimer = GetTimeLeftForToday();
    }
    private static void NoInternetConnection()
    {
        noInternetConnectionParentStatic.gameObject.SetActive(true);
        searchingForInternet = true;
    }
    private void ResetLeaderboards()
    {
        Debug.Log("Trying to reset leaderboards");
        int prevSeed = GetSeedFromUTC();
        if (!calledFirstLeaderboardFind)
        {
            prevSeed = 0;
        }

        RefreshUTCDate();
        if (hasValidDate)
        {
            int newSeed = GetSeedFromUTC();
            dailyTimer = GetTimeLeftForToday();
            if (newSeed != prevSeed)
            {
                if (leaderboardManager != null)
                    leaderboardManager.FindLeaderboard(newSeed.ToString());
                GameObject.FindObjectOfType<DailyMountainScreen>().RefreshScreen();
                CancelInvoke();
            }
        }

        calledReset = false;
    }
}
