namespace Sample.SP0.Client.Core
{
    public class TokenStore
    {
        public string TokenType;
        public DateTime ExpireDateTime;
        public string Token;

        public TokenStore()
        {
            Token = string.Empty;
            TokenType = string.Empty;
        }
    }
}
