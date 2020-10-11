using System;
using System.Collections.Generic;
using System.Text;

namespace C4_CSharp
{
    public abstract class C4DocumentBase : C4Item
    {
        public abstract string GetTemplateFile();

        public string Title { get; set; }
        public bool ShowLegend { get; set; }
        public bool ShowSketch { get; set; }
        public bool TopDown { get; set; }

        public List<C4Relationship> relationships = new List<C4Relationship>();

        public void Relate(C4Item from, string label, C4Item to, string technology = "")
        {
            Relate(from.Alias, label, to.Alias, technology);
        }

        public void Relate(string from, string label, string to, string technology = "")
        {
            relationships.Add(new C4Relationship
            {
                From = from,
                To = to,
                Label = label,
                Technology = technology
            });
        }

        public void RelateDynamically(string sequenceNumber, C4Item from, string label, C4Item to, string technology = "")
        {
            RelateDynamically(sequenceNumber, from.Alias, label, to.Alias, technology);
        }
        public void RelateDynamically(string sequenceNumber, string from, string label, string to, string technology = "")
        {
            relationships.Add(new C4DynamicRelationship
            {
                SequenceNumber = sequenceNumber,
                From = from,
                To = to,
                Label = label,
                Technology = technology
            }); ;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"@startuml {Name}");
            sb.AppendLine($"!include {GetTemplateFile()}");
            if (!string.IsNullOrEmpty(Title))
            {
                sb.AppendLine($"title {Title}");
            }

            if (ShowSketch)
            {
                sb.AppendLine("LAYOUT_AS_SKETCH()");
            }

            if (ShowLegend)
            {
                sb.AppendLine("LAYOUT_WITH_LEGEND()");
            }

            if (TopDown)
            {
                sb.AppendLine("LAYOUT_TOP_DOWN()");
            }
            else
            {
                sb.AppendLine("LAYOUT_LEFT_RIGHT()");
            }

            foreach (C4Item item in items)
            {
                sb.AppendLine(item.ToString().TrimEnd());
            }

            foreach (C4Relationship relationship in relationships)
            {
                sb.AppendLine(relationship.ToString().TrimEnd());
            }

            sb.AppendLine("@enduml");

            return sb.ToString();
        }
    }

    public abstract class C4ContainerDocument : C4DocumentBase
    {
        public override string GetTemplateFile()
        {
            return "https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_Container.puml";
        }
    }

    public abstract class C4DynamicDocument : C4DocumentBase
    {
        public override string GetTemplateFile()
        {
            return "https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_Dynamic.puml";
        }
    }

    public abstract class C4ContextDocument : C4DocumentBase
    {
        public override string GetTemplateFile()
        {
            return "https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_Context.puml";
        }
    }

    public abstract class C4ComponentDocument : C4DocumentBase
    {
        public override string GetTemplateFile()
        {
            return "https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_Component.puml";
        }
    }


    public abstract class C4DeploymentDocument : C4DocumentBase
    {
        public override string GetTemplateFile()
        {
            return "https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_Deployment.puml";
        }
    }
}
