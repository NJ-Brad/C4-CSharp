using C4_CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples
{
    public class DeploymentSample : C4DeploymentDocument
    {
        public DeploymentSample()
        {
            Name = "Deploymemnt Example";
            Title = "Internet Banking System - Deployment";
            ShowLegend = true;
            ShowSketch = true;
            TopDown = true;

            this
            .Node("Customer's computer", "Microsoft Windows or Apple </size>\n<size:$TECHN_FONT_SIZE> macOS", contains: e =>
            {
                e.Node("Web Browser", "Google Chrome, Mozilla </size>\n<size:$TECHN_FONT_SIZE>Firefox, Apple Safari or </size>\n<size:$TECHN_FONT_SIZE>Microsoft Edge", contains: e =>
                {
                    e.Container("Single-Page Application", "JavaScript and Angular", "Provides all of the Internet banking functionality to customers via their web browser.");
                });
            })

            .Node("Customer's mobile device", "Apple iOS or Android", contains: e =>
            {
                e.Container("Mobile App", "Xamarin", "Provides a limited subset of the Internet banking functionality to customers via their mobile device.");
            })

            .Node("Big Bank plc", "Big Bank plc data center", contains: e =>
            {
                e.Node("bigbank-web*** (x4)", "Ubuntu 16.04 LTS", contains: e =>
                {
                    e.Node("Apache Tomcat", "Apache Tomcat 8.x", contains: e =>
                    {
                        e.Container("Web Application", "Java and Spring MVC", "Delivers the static content and the Internet banking single page application.");
                    });
                });

                e.Node("bigbank-api*** (x8)", "Ubuntu 16.04 LTS", contains: e =>
                {
                    e.Node("Apache Tomcat 2", "Apache Tomcat 8.x", contains: e =>
                    {
                        e.Container("API_Application", "Java and Spring MVC", "Provides Internet banking functionality via a JSON/HTTPS API.");
                    });
                });

                e.Node("bigbank-db01", "Ubuntu 16.04 LTS", contains: e =>
                {
                    e.Node("Oracle - Primary", "Oracle 12c", contains: e =>
                    {
                        e.Container("Primary Database", "Relational Database Schema", "Stores user registration information, hashed authentication credentials, access logs, etc.", database: true);
                    });
                });

                e.Node("bigbank-db02", "Ubuntu 16.04 LTS", contains: e =>
                {
                    e.Node("Oracle - Secondary", "Oracle 12c", contains: e =>
                    {
                        e.Container("Secondary Database", "Relational Database Schema", "Stores user registration information, hashed authentication credentials, access logs, etc.", database: true);
                    });
                });
            })
;

            Relate("API Application", "Reads from and writes to", "Primary Database", "JDBC");
            Relate("API Application", "Reads from and writes to", "Secondary Database", "JDBC");
            Relate("Mobile App", "Makes API calls to", "API_Application", "JSON/HTTPS");
            Relate("Single Page Application", "Makes API calls to", "API Application", "JSON/HTTPS");
            Relate("Web Application", "Delivers to the customer's web browser", "Single Page Application");
            Relate("Oracle - Primary", "Replicates data to", "Oracle - Secondary");
        }
    }
}
