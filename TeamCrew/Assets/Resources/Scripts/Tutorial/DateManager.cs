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
	//privates
    private Timer dailyTimer;
    private GameManager gameManager;

	//Unity methods
    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        RefreshUTCDate();
        dailyTimer = GetTimeLeftForToday();
    }
    void Update()
    {
        if (gameManager.gameActive)
            return;

        if (dailyTimer != null)
        {
            dailyTimer.Decrement();
            foreach (TextMesh text in timeLeftTexts)
            {
                text.text = dailyTimer.GetTimeString(true, true, true, false);
            }
        }
    }

	//public methods
    public static void RefreshUTCDate()
    {
        CurrentUTCDate = GetNetworkTime();
        hasValidDate = (CurrentUTCDate != null);
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
    public static int GetSeedFromUTC()
    {
        int seed = 0;

        seed += CurrentUTCDate.Year + CurrentUTCDate.Month + CurrentUTCDate.Day;

        return seed;
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

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

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
}
