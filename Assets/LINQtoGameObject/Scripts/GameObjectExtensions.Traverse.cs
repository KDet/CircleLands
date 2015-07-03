using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    // Inspired from LINQ to XML.
    // Reference: http://msdn.microsoft.com/en-us/library/system.xml.linq.xelement.aspx
    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/ObjectsBeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        /// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;
            
            var child = origin.transform.FindChild(name);
            if (child == null) return null;
            return child.gameObject;
        }

        /// <summary>Returns a collection of the child GameObjects.</summary>
        public static IEnumerable<GameObject> Children(this GameObject origin)
        {
            return ChildrenCore(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the child GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Children(this GameObject origin, string name)
        {
            return ChildrenCore(origin, name, false);
        }


        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin)
        {
            return ChildrenCore(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the child GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin, string name)
        {
            return ChildrenCore(origin, name, true);
        }

        static IEnumerable<GameObject> ChildrenCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform child in origin.transform)
            {
                if (nameFilter == null || child.name == nameFilter)
                {
                    yield return child.gameObject;
                }
            }
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin)
        {
            return AncestorsCore(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the ancestor GameObjects of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin, string name)
        {
            return AncestorsCore(origin, null, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin)
        {
            return AncestorsCore(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this element, and the ancestors of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin, string name)
        {
            return AncestorsCore(origin, name, true);
        }

        static IEnumerable<GameObject> AncestorsCore(GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parentTransform = origin.transform.parent;
            while (parentTransform != null)
            {
                if (nameFilter == null || parentTransform.name == nameFilter)
                {
                    yield return parentTransform.gameObject;
                }
                parentTransform = parentTransform.parent;
            }
        }

        /// <summary>Returns a collection of the descendant GameObjects.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin)
        {
            return DescendantsCore(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the descendant GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin, string name)
        {
            return DescendantsCore(origin, name, false);
        }


        /// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin)
        {
            return DescendantsCore(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin, string name)
        {
            return DescendantsCore(origin, name, true);
        }

        static IEnumerable<GameObject> DescendantsCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform item in origin.transform)
            {
                foreach (var child in DescendantsCore(item.gameObject, nameFilter, true))
                {
                    if (nameFilter == null || child.name == nameFilter)
                    {
                        yield return child.gameObject;
                    }
                }
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, name, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, name, true);
        }

        static IEnumerable<GameObject> BeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;

            var parent = origin.transform.parent;
            if (parent == null) goto RETURN_SELF;

            foreach (Transform item in parent.transform)
            {
                var go = item.gameObject;
                if (go == origin)
                {
                    goto RETURN_SELF;
                }

                if (nameFilter == null || item.name == nameFilter)
                {
                    yield return go;
                }
            }

        RETURN_SELF:
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, name, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, name, true);
        }

        static IEnumerable<GameObject> AfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parent = origin.transform.parent;
            if (parent == null) yield break;

            var index = origin.transform.GetSiblingIndex() + 1;
            var parentTransform = parent.transform;
            var count = parentTransform.childCount;

            while (index < count)
            {
                var target = parentTransform.GetChild(index).gameObject;
                if (nameFilter == null || target.name == nameFilter)
                {
                    yield return target;
                }

                index++;
            }
        }
    }
}