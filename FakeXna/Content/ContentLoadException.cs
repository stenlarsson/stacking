using System;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentLoadException : Exception
    {
        public ContentLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

