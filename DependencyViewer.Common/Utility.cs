using System;
using System.Collections;
using System.Xml;

namespace DependencyViewer.Common
{
    public class EmptyXmlNodeList : XmlNodeList
    {
        public override XmlNode Item(int index)
        {
            throw new InvalidOperationException();
        }

        public override IEnumerator GetEnumerator()
        {
            return new EmptyEnumerator();
        }

        public override int Count
        {
            get { return 0; }
        }
    }

    public class EmptyEnumerator : IEnumerator
    {
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }
    }
}