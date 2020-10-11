using C4_CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Samples
{
    public class ContainerSample : C4ContainerDocument
    {
        C4Person customer = new C4Person("Customer", "A customer", external: true);
        C4SystemBoundary system = new C4SystemBoundary("Customer Information System");
        C4Container custApp = new C4Container("Customer Application", "Angular");
        C4Container custSvc = new C4Container("Customer Service", "Java and Spring Boot");

        public ContainerSample()
        {
            Name = "Container Example";
            Title = "Customer Information System - Containers";
            ShowLegend = true;
            ShowSketch = true;
            TopDown = true;

            this
                .Person("Customer", "A customer", external: true)
                .SystemBoundary("Customer Information System", contains: e =>
                {
                    e.Container("Customer Application", "Angular")
                    .Container("Customer Service", "Java and Spring Boot");
                });

            Relate("Customer", "Uses", "Customer Application");
        }
    }
}
