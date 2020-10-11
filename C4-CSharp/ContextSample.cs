using System;
using System.Collections.Generic;
using System.Text;

namespace C4_CSharp
{
    class ContextSample : C4ContextDocument
    {
        public ContextSample()
        {
            Name = "Enterprise";
            Title = "";
            ShowLegend = true;
            ShowSketch = true;
            TopDown = true;

            this
            .Person(name: "Customer",
                    description: "A customer of Widgets Limited.",
                    alias: "customer")
            .Enterprise(name: "Widgets Limited",
                    alias: "c0",
                    contains: e =>
                    {
                        e.Person(name: "Customer Service Agent",
                            description: "Deals with customer enquiries.",
                            alias: "csa")
                        .System(name: "E-commerce System",
                            description: "Allows customers to buy widgts online via the widgets.com website.",
                            alias: "ecommerce")
                        .System("Fulfillment System", "Responsible for processing and shipping of customer orders.", "fulfillment");
                    })
            .System(name: "Braintree Payments",
                description: "Processes credit card payments on behalf of Widgets Limited.",
                alias: "braintree")
            .System(name: "Taxamo",
                description: "Calculates local tax (for EU B2B customers) and acts as a front-end for Braintree Payments.",
                alias: "taxamo")
            .System(name: "Jersey Post",
                description: "Calculates worldwide shipping costs for packages.",
                alias: "post")
            ;

            Relate("customer", "Asks questions to", "csa", "Telephone");
            Relate("customer", "Places orders for widgets using", "ecommerce");
            Relate("csa", "Looks up order information using", "ecommerce");
            Relate("ecommerce", "Sends order information to", "fulfillment");
            Relate("fulfillment", "Gets shipping charges from", "post");
            Relate("ecommerce", "Delegates credit card processing to", "taxamo");
            Relate("taxamo", "Uses for credit card processing", "braintree");
        }


    }
}
