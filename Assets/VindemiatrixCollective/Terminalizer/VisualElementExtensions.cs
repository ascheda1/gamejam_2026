// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

#region

using System.Linq;
using UnityEngine.UIElements;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public static class VisualElementExtensions
    {
        public static void SwapClass(this VisualElement element, string prefix, string newClass)
        {
            string previousClass = element.GetClasses().FirstOrDefault(cls => cls.StartsWith(prefix));
            element.RemoveFromClassList(previousClass);
            element.AddToClassList(newClass);
        }
    }
}