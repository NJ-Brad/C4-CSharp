using C4_CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Samples
{
    //    public class DocumentOne : C4ContainerDocumentBase
    public class DynamicSample : C4DynamicDocument
    {
        public DynamicSample()
        {
            Name = "Dynamic Example";
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

            RelateDynamically("1", "Customer", "Uses", "Customer Application");
        }
    }
}
