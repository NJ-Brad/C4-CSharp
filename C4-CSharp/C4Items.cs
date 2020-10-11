using System;
using System.Collections.Generic;
using System.Text;

namespace C4_CSharp
{

    public class C4Item
    {
        public List<C4Item> items = new List<C4Item>();

        public string Name { get; set; }
        public string Alias { get => alias;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    //alias = Guid.NewGuid().ToString().Replace('-', '_');
                    alias = Name.Replace(' ', '_').Replace('-', '_');
                }
                else
                {
                    alias = value;
                }
            }
        }

        string alias;

        public void Add(C4Item item)
        {
            items.Add(item);
        }
    }

    public class C4Person : C4Item
    {
        public string Description { get; set; }
        public bool External { get; set; } = false;

        public C4Person(string name, string description = "", string alias = "", bool external = false)
        {
            Name = name;
            Description = description;
            Alias = alias;
            External = external;
        }

        public override string ToString()
        {
            string prefix = External ? "Person_Ext" : "Person";

            string output = $"{prefix}({Alias}, \"{Name}\"";
            output = output + (string.IsNullOrEmpty(Description) ? ")" : $", \"{Description}\")");

            return output;
        }
    }

    public class C4Component : C4Item
    {
        public string Technology { get; set; }
        public bool External { get; set; } = false;
        public bool Database { get; set; } = false;
        public string Description { get; set; }


        public C4Component(string name, string description, string technology = "System", string alias = "", bool external = false, bool database = false)
        {
            Name = name;
            Description = description;
            Technology = technology;
            Alias = alias;
            External = external;
            Database = database;
        }

        public override string ToString()
        {
            string prefix = Database ? "ComponentDb" : "Component";

            if (External)
                prefix = prefix + "_Ext";

            string output = $"{prefix}({Alias}, \"{Name}\", \"{Technology}\"";
            output = output + (string.IsNullOrEmpty(Description) ? ")" : $", \"{Description}\")");

            return output;
        }
    }


    public class C4System : C4Item
    {
        public string Description { get; set; }
        public bool External { get; set; } = false;
        public bool Database { get; set; } = false;

        public C4System(string name, string description = "System", string alias = "", bool external = false, bool database = false)
        {
            Name = name;
            Description = description;
            Alias = alias;
            External = external;
            Database = database;
        }

        public override string ToString()
        {
            string prefix = Database ? "SystemDb" : "System";

            if (External)
                prefix = prefix + "_Ext";

            string output = $"{prefix}({Alias}, \"{Name}\"";
            output = output + (string.IsNullOrEmpty(Description) ? ")" : $", \"{Description}\")");

            return output;
        }
    }
    public class C4SystemBoundary : C4Item
    {
        public C4SystemBoundary(string name, string alias = "")
        {
            Name = name;
            Alias = alias;
        }

        public override string ToString()
        {
            string prefix = "System_Boundary";

            StringBuilder sb = new StringBuilder($"{prefix}({Alias}, \"{Name}\")");
            sb.AppendLine(" {");

            foreach (C4Item item in items)
            {
                sb.AppendLine("  " + item.ToString());
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class C4ContainerBoundary : C4Item
    {
        public C4ContainerBoundary(string name, string alias = "")
        {
            Name = name;
            Alias = alias;
        }

        public override string ToString()
        {
            string prefix = "Container_Boundary";

            StringBuilder sb = new StringBuilder($"{prefix}({Alias}, \"{Name}\")");
            sb.AppendLine(" {");

            foreach (C4Item item in items)
            {
                sb.AppendLine("  " + item.ToString());
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class C4Node : C4Item
    {
        public string Technology { get; set; }
        public C4Node(string name, string technology, string alias = "")
        {
            Name = name;
            Alias = alias;
            Technology = technology.Replace("\n", "\\n");
        }

        public override string ToString()
        {
            string prefix = "Node";

            StringBuilder sb = new StringBuilder($"{prefix}({Alias}, \"{Name}\", \"{Technology}\")");
            sb.AppendLine(" {");

            foreach (C4Item item in items)
            {
                sb.AppendLine("  " + item.ToString().TrimEnd());
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class C4Experiment : C4Item
    {
        public C4Experiment(string name, string alias = "")
        {
            Name = name;
            Alias = alias;
        }

        public override string ToString()
        {
            string prefix = "Enterprise_Boundary";

            StringBuilder sb = new StringBuilder($"{prefix}({Alias}, \"{Name}\")");
            sb.AppendLine(" {");

            foreach (C4Item item in items)
            {
                sb.AppendLine("  " + item.ToString());
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }


    public class C4Enterprise : C4Item
    {
        public C4Enterprise(string name, string alias = "")
        {
            Name = name;
            Alias = alias;
        }

        public override string ToString()
        {
            string prefix = "Enterprise_Boundary";

            StringBuilder sb = new StringBuilder($"{prefix}({Alias}, \"{Name}\")");
            sb.AppendLine(" {");

            foreach (C4Item item in items)
            {
                sb.AppendLine("  " + item.ToString());
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class C4Container : C4Item
    {
        public string Technology { get; set; }
        public string Description { get; set; }
        public bool External { get; set; } = false;
        public bool Database { get; set; } = false;

        public C4Container(string name, string technology, string description = "", string alias = "", bool external = false, bool database = false)
        {
            Name = name;
            Technology = technology;
            Description = description;
            Alias = alias;
            External = external;
            Database = database;
        }

        public override string ToString()
        {
            string prefix = Database ? "ContainerDb" : "Container";

            if (External)
                prefix = prefix + "_Ext";

            string output = $"{prefix}({Alias}, \"{Name}\", \"{Technology}\"";

            output = output + (string.IsNullOrEmpty(Description) ? ")" : $", \"{Description}\")");

            return output;
        }
    }

    public class C4Relationship
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Label { get; set; }
        public string Technology { get; set; }

        //Rel(CustomerInformationSystem__AuditService__1e6fa8e, CustomerInformationSystem__AuditStore__11ecf05, "Stores events in")
        public override string ToString()
        {
            string prefix = "Rel";

            string output = $"{prefix}({FixAlias(From)}, {FixAlias(To)}, \"{Label}\"";

            output = output + (string.IsNullOrEmpty(Technology) ? ")" : $", \"{Technology}\")");

            return output;
        }

        private string FixAlias(string input)
        {
            return input.Replace(' ', '_').Replace('-', '_');
        }

    }

    public class C4DynamicRelationship : C4Relationship
    {
        public string SequenceNumber { get; set; }

        //Rel(CustomerInformationSystem__AuditService__1e6fa8e, CustomerInformationSystem__AuditStore__11ecf05, "Stores events in")
        public override string ToString()
        {
            string prefix = "Interact2";

            string output = $"{prefix}(\"{SequenceNumber}\", {FixAlias(From)}, {FixAlias(To)}, {Label}";

            output = output + (string.IsNullOrEmpty(Technology) ? ")" : $", \"{Technology}\")");

            return output;
        }

        private string FixAlias(string input)
        {
            return input.Replace(' ', '_').Replace('-', '_');
        }

    }

}
