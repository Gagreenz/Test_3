using System.Text;
public class Token
{
    public long Id { get;set;}
    public List<string> history { get;set;}
    public Token(long id)
    {
        this.Id = id;
        history = new List<string>()
        {
            $"|Token has been created Time:{System.DateTime.Now}|"
        };
    }
    public string GetHistory()
    {
        StringBuilder historyString = new StringBuilder();
        foreach(var hist in history)
        {
            historyString.Append(hist);
        }
        return historyString.ToString();
    } 
    public void AddHistory(User userFrom,User userTo)
    {
        history.Add($"|Token has been moved from:{userFrom.Name} to {userTo.Name} Time:{System.DateTime.Now}|");
    }
}