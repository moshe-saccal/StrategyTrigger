using SmartTrigger;
var l = TimeSpan.FromDays(2000);
var years = l.TotalDays / 360;


var pac = new PackageSchedulerNotificator();
var res =pac.GetNotificables().GetAsyncEnumerator();

while(await res.MoveNextAsync())
{
    Console.WriteLine(res.Current.Item1.UniqueId);
}
Console.ReadLine();


l.ToString();

public class PackageSchedulerNotificationStrategy : INotificationStrategy
{
    public bool RetryIfDayIsVoided => true;

    public IEnumerable<DayOfWeek> AvoidedDayOfWeks => new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Saturday };

    public IEnumerable<NotificationStrategyWindow> NotificationStrategyWindows
        => new NotificationStrategyWindow[] { new NotificationStrategyWindow ()
        {
              Start = TimeSpan.FromHours(10),
               End = TimeSpan.FromHours(20)
        } };


    public IEnumerable<NotificationStrategyReminder> NotificationStrategyReminders
        => new NotificationStrategyReminder[]
        {
            new NotificationStrategyReminder(){  Interval= TimeSpan.FromDays(200)},
            new NotificationStrategyReminder(){  Interval= TimeSpan.FromMinutes(10)},
            new NotificationStrategyReminder(){  Interval= TimeSpan.FromMinutes(20)},
        };
    public TimeSpan ExpirationSpanAfterInitialDate => TimeSpan.FromMinutes(100);

    public TimeSpan ExpirationSpanBeforeEndingDate => TimeSpan.FromMinutes(200);
}
public class PackageSchedulerNotificator : SmartTriggerBase
{
    public PackageSchedulerNotificator() : base(new PackageSchedulerNotificationStrategy(),
        new PackageSchedulerHistoricalNotificationProvider(20),
        new CurrentDateProvider())
    {

    }

    public IEnumerable<Guid> AccountsToBeNotified { get; set; }
}



public class PackageSchedulerTask : INotificable
{
    public string UniqueId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; }

}
public class PackageSchedulerHistoricalNotificationProvider : INotificationsProvider
{
    private int _Q;
    private Dictionary<string,DateTime> _hiist= new Dictionary<string,DateTime>();

    public PackageSchedulerHistoricalNotificationProvider(int q)
    {
        _Q = q;
    }
    public DateTime LastNotification(string UniqueId)
    {
        return _hiist[UniqueId];

        //return DateTime.MinValue;
    }

    public IEnumerable<INotificable> Provide()
    {
        return Enumerable.Range(0, _Q).Select(a => new PackageSchedulerTask()
        {
            UniqueId = System.Guid.NewGuid().ToString(),
            Start = DateTime.Now
        }); ;
        
    }

    public void SetNotified(string UniqueId)
    {
        _hiist.Add(UniqueId, DateTime.Now);

        //throw new NotImplementedException();
    }
}
