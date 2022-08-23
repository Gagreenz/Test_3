public class User{
    public string Name { get;set;}
    public int Rating { get;set;}
    public List<Token> Tokens { get;set;}
    public User(string name,int rating)
    {
        this.Name = name;
        this.Rating = rating;
        Tokens = new List<Token>();
    }
    private List<Token> EraseCoin(int count)
    {
        if(count > Tokens.Count) return null;

        List<Token> temp = new List<Token>();
        for(int i = 0; i < count; i++)
        {
            temp.Add(Tokens[i]);
        }
        Tokens.RemoveRange(0,count);
        return temp;
    }
    public void GetCoinsFromUser(User user,int count)
    {
        var coins = user.EraseCoin(count);
        foreach(var coin in coins)
        {
            coin.AddHistory(user,this);
        }
        Tokens.AddRange(coins);
    }
}