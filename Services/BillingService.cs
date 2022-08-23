using Grpc.Core;

namespace Server.Services;

public class BillingService : Billing.BillingBase
{
    public static List<User> Users = new List<User>()
    {
            new User("boris",5000),
            new User("maria",1000),
            new User("oleg",800)
    };
    public override async Task ListUsers(None request, IServerStreamWriter<UserProfile> responseStream, ServerCallContext context)
    {
        foreach(var user in Users)
        {
            await responseStream.WriteAsync(new UserProfile
            {
                Name = user.Name,
                Amount = user.Tokens.Count
            });
        }
        
    }
    public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
    {
        long avaibleCoins = request.Amount;

        if(avaibleCoins < Users.Count) 
            return Task.FromResult(
                new Response{
                    Status = Response.Types.Status.Failed,
                    Comment = "Coins amount must be greater than users"
                }
            );
        int totalRating = 0;
        foreach(var user in Users)
        {
            totalRating += user.Rating;

            var newCoin = new Token(System.DateTime.Now.Ticks);
            user.Tokens.Add(newCoin);
            avaibleCoins--;
        }
        float avgCoinCost = totalRating/avaibleCoins;
        
        foreach(var user in Users)
        {
            if(avaibleCoins == 0) 
                return Task.FromResult(
                    new Response{
                        Status = Response.Types.Status.Ok,
                        Comment = "Coin emission completed!"
                    }
                );
            var count = MathF.Floor(user.Rating / avgCoinCost);
            if(count == 0) count = 1;
            for(int i =0;i<count;i++)
            {
                var newCoin = new Token(System.DateTime.Now.Ticks);
                user.Tokens.Add(newCoin);
            }
        }

        return Task.FromResult(
                new Response{
                    Status = Response.Types.Status.Ok,
                    Comment = "Coin emission completed!"
                }
            );
    }
    public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
    {
        var fromUser = Users.FirstOrDefault(u => u.Name == request.SrcUser);
        var toUser = Users.FirstOrDefault(u => u.Name == request.DstUser);
        int amount = Convert.ToInt32(request.Amount);

        if(fromUser == null && toUser == null)
            return Task.FromResult(
                new Response{
                    Status = Response.Types.Status.Failed,
                    Comment = "Users not found"
                }
            );

        if(fromUser.Tokens.Count <= request.Amount)
            return Task.FromResult(
                new Response{
                    Status = Response.Types.Status.Failed,
                    Comment = "Not enough money"
                }
            );

        toUser.GetCoinsFromUser(fromUser,amount);

        return Task.FromResult(
                new Response{
                    Status = Response.Types.Status.Ok,
                    Comment = ""
                }
            );
    }
    public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
    {
        Token LongestHistoryCoin = new Token(-1);
        foreach(var user in Users)
        {   
            for(int i = 0;i<user.Tokens.Count;i++)
            {
                if(user.Tokens[i].history.Count > LongestHistoryCoin.history.Count)
                    LongestHistoryCoin = user.Tokens[i];
            }
            
            //LongestHistoryCoin.Add(user.Tokens.Where(t => t.history.Count == t.history.Max(c => c.history.Count));
        }
        return Task.FromResult(
                new Coin{
                    Id = LongestHistoryCoin.Id,
                    History = LongestHistoryCoin.GetHistory()
                }
            );
    }
}
