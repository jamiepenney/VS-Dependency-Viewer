using System;

namespace DependencyViewer.Common.Loaders
{
    public class LoaderException : Exception
    {
        public LoaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public LoaderException(string message) : base(message)
        {
        }
    }
}