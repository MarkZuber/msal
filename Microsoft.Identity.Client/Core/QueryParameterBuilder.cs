using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Identity.Client.Core
{
    internal sealed class QueryParameterBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public QueryParameterBuilder()
        {
        }

        public QueryParameterBuilder(string currentQuery)
        {
            _sb.Append(currentQuery);
        }

        public QueryParameterBuilder(
            string initialKey,
            string initialValue)
        {

        }

        public void AddQueryPair(
            string key,
            string value)
        {

        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
