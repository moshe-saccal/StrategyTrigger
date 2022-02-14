using SmartTrigger;
using SmartTrigger.Interfaces;
using SmartTrigger.Models;

var l = TimeSpan.FromDays(2000);
var years = l.TotalDays / 360;


var pac = new PackageSchedulerNotificator();
 
Console.ReadLine();


l.ToString();
 



public class PackageSchedulerTask : INotificable
{
    public string NotificationIdentifier { get; set; }
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
            NotificationIdentifier = System.Guid.NewGuid().ToString(),
            Start = DateTime.Now
        }); ;
        
    }

    public void SetNotified(string UniqueId)
    {
        _hiist.Add(UniqueId, DateTime.Now);

        //throw new NotImplementedException();
    }
}
