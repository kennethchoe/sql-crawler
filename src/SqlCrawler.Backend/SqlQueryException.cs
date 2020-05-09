using System;

namespace SqlCrawler.Backend
{
    public class SqlQueryException : Exception
    {
        public SqlQueryException(string message) : base(message)
        {
        }
    }
}