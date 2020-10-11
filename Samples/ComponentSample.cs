using C4_CSharp;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples
{
    public class ComponentSample : C4ComponentDocument
    {
        public ComponentSample()
        {
            Name = "Component Example";
            Title = "Component diagram for Internet Banking System - API Application";
            ShowLegend = true;
            ShowSketch = true;
            TopDown = true;

            this
                .Container("Single Page Application", "javascript and angular", "Provides all the internet banking functionality to customers via their web browser.", "spa")
                .Container("Mobile App", "Xamarin", "Provides a limited subset ot the internet banking functionality to customers via their mobile mobile device.", "ma")
                .Container("Database", "Relational Database Schema", "Stores user registration information, hashed authentication credentials, access logs, etc.", "db", database: true)
                .System("Mainframe Banking System", "Stores all of the core banking information about customers, accounts, transactions, etc.", "mbs", external: true)
                .ContainerBoundary("API Application", contains: e =>
                {
                    e.Component("Sign In Controller", "MVC Rest Controller", "Allows users to sign in to the internet banking system", "sign")
                    .Component("Accounts Summary Controller", "MVC Rest Controlle", "Provides customers with a summory of their bank accounts", "accounts")
                    .Component("Security Component", "Spring Bean", "Provides functionality related to singing in, changing passwords, etc.", "security")
                    .Component("Mainframe Banking System Facade", "Spring Bean", "A facade onto the mainframe banking system.", "mbsFacade");
                })
                ;

            Relate("sign", "Uses", "security");
            Relate("accounts", "Uses", "mbsFacade");
            Relate("security", "Read & write to", "db", "JDBC");
            Relate("mbsFacade", "Uses", "mbs", "XML/HTTPS");

            Relate("spa", "Uses", "sign", "JSON/HTTPS");
            Relate("spa", "Uses", "accounts", "JSON/HTTPS");

            Relate("ma", "Uses", "sign", "JSON/HTTPS");
            Relate("ma", "Uses", "accounts", "JSON/HTTPS");
        }
    }
}
