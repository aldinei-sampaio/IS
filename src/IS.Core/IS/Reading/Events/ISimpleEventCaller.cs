namespace IS.Reading.Events;

public interface ISimpleEventCaller
{
    Task ChangeAsync(string value);
}
