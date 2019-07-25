using CSharpTranslator.src.Generators;

namespace CSharpTranslator.src.Core
{
    public class GenericNodeTree
    {
        public static void AddChild(ref GenericNode parent, ref GenericNode child)
        {
            if (parent == null || child == null) return;

            if (parent.Child == null)
                parent.Child = child;
            else
                AddSibling(ref parent, ref child);
        }

        public static void PrependChild(ref GenericNode parent, ref GenericNode prependedChild)
        {
            if (parent == null || prependedChild == null) return;

            if (parent.Child == null)
            {
                parent.Child = prependedChild;
                return;
            }

            GenericNode previousFirstChild = parent.Child;
            parent.Child = prependedChild;
            parent.Child.Next = previousFirstChild;
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
