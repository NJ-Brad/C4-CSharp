using System;
using System.Collections.Generic;
using System.Text;

namespace C4_CSharp
{
    public static class C4Extensions
    {
        //public static C4Item Experiment(this C4Item parent, string name, string alias = "", C4Item[] children = null)
        //{
        //    parent.Add(new C4Experiment(name, alias, children));
        //    return parent;
        //}

        public static C4Item Experiment(this C4Item parent, string name, string alias = "", Action<C4Item> contains = null)
        {
            C4Item newItem = new C4Experiment(name, alias);
            parent.Add(newItem);
            if (contains != null)
            {
                contains(newItem);
            }
            return parent;
        }

        public static C4Item Person(this C4Item parent, string name, string description = "", string alias = "", bool external = false)
        {
            C4Item newItem = new C4Person(name, description, alias, external);
            parent.Add(newItem);
            return parent;
        }

        public static C4Item Container(this C4Item parent, string name, string technology, string description = "", string alias = "", bool external = false, bool database = false)
        {
            C4Item newItem = new C4Container(name, technology, description, alias, external, database);
            parent.Add(newItem);
            return parent;
        }


        public static C4Item Node(this C4Item parent, string name, string technology, string alias = "", Action<C4Item> contains = null)
        {
            C4Item newItem = new C4Node(name, technology, alias);
            parent.Add(newItem);
            if (contains != null)
            {
                contains(newItem);
            }
            return parent;
        }

        public static C4Item System(this C4Item parent, string name, string description, string alias = "", bool external = false, bool database = false)
        {
            C4Item newItem = new C4System(name, description, alias, external, database);
            parent.Add(newItem);
            return parent;
        }
        public static C4Item Component(this C4Item parent, string name, string description, string technology, string alias = "", bool external = false, bool database = false)
        {
            C4Item newItem = new C4Component(name, description, technology, alias, external, database);
            parent.Add(newItem);
            return parent;
        }

        public static C4Item ContainerBoundary(this C4Item parent, string name, string alias = "", Action<C4Item> contains = null)
        {
            C4Item newItem = new C4ContainerBoundary(name, alias);
            parent.Add(newItem);
            if (contains != null)
            {
                contains(newItem);
            }
            return parent;
        }

        public static C4Item SystemBoundary(this C4Item parent, string name, string alias = "", Action<C4Item> contains = null)
        {
            C4Item newItem = new C4SystemBoundary(name, alias);
            parent.Add(newItem);
            if (contains != null)
            {
                contains(newItem);
            }
            return parent;
        }

        public static C4Item Enterprise(this C4Item parent, string name, string alias = "", Action<C4Item> contains = null)
        {
            C4Item newItem = new C4Enterprise(name, alias);
            parent.Add(newItem);
            if (contains != null)
            {
                contains(newItem);
            }
            return parent;
        }
    }
}
