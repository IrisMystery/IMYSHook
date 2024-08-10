using System;
using System.Globalization;
using System.Timers;

namespace IMYSHook;

public class Tasker
{
    private static Timer _apTimer = new();
    private static Timer _bpTimer = new();
    private static Timer _expeditionTimer = new();

    private static Timer GetTimer(int eventType)
    {
        switch (eventType)
        {
            case 1:
                return _apTimer;
            case 2:
                return _bpTimer;
            case 3:
                return _expeditionTimer;
        }
        return null;
    }

    public static void Set(int eventType, string scheduleAt)
    {
        var timer = GetTimer(eventType);

        if (timer == null) return;

        if (timer.Enabled) timer.Stop();

        var interval = GetInterval(scheduleAt);

        if (interval <= 0) return;

        timer = new Timer(interval * 1000);
        timer.Elapsed += (sender, e) => EventNotification(sender, e, eventType);
        timer.AutoReset = false;
        timer.Enabled = true;
    }

    private static int GetInterval(string scheduleTime)
    {
        if (scheduleTime == "") return 0;

        var result = DateTime.ParseExact(scheduleTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        result = result.AddHours(-1);

        var now = DateTime.Now;

        var interval = (int)(result - now).TotalSeconds;

        return interval;
    }

    private static void EventNotification(object source, ElapsedEventArgs e, int eventType)
    {
        switch (eventType)
        {
            case 1:
                Notification.Popup("回復提示", "AP已回復完成！");
                break;
            case 2:
                Notification.Popup("回復提示", "BP已回復完成！");
                break;
            case 3:
                Notification.Popup("遠征提示", "遠征完成！");
                break;
        }
    }
}