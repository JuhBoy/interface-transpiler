using CSharpTranslator.src.Generators;

namespace CSharpTranslator.src.Core
{
    public static class GenericNodeTreeHelper
    {
        public static void AddChild(ref GenericNode parent, ref GenericNode child)
        {
            if (parent == null || child == null) return;

            if (parent.Child == null)
                parent.Child = child;
            else
                AddSibling(ref parent, ref child);
        }

        public static void AddSibling(ref GenericNode parent, ref GenericNode newChild)
        {
            GenericNode currentSibling = parent.Child;
            while (currentSibling.Next != null)
                currentSibling = currentSibling.Next;
            currentSibling.Next = newChild;
        }
    }
}
