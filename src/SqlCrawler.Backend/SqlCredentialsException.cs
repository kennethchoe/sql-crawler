using System;

namespace SqlCrawler.Backend
{
    public class SqlCredentialsException : Exception
    {
        public SqlCredentialsException(string message): base(message)
        {
        }
    }
}