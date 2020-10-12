using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace C4_CSharp
{
    public class PlannedCommand : Command
    {
        private readonly PlannedCommandOptions options;

        public PlannedCommand(PlannedCommandOptions options)
           : base("planned", "How a project is planned.")
        {
            var assembly = new Option<string>("--assembly")
            {
                Name = "Assembly",
                Description = "The assembly to look in, for C4Documents.",
                IsRequired = false
            };
            this.AddOption(assembly);

            var outputFolder = new Option<string>("--outputfolder")
            {
                Name = "Output Folder",
                Description = "Where to store the png files. (If empty, the assembly folder will be used).",
                IsRequired = false
            };
            this.AddOption(outputFolder);


            this.Handler = CommandHandler.Create(
                (string assembly, string outputFolder) => this.HandleCommand(assembly, outputFolder));
            this.options = options;

            // an alternate execution library
            // https://github.com/Tyrrrz/CliWrap
        }

        private int HandleCommand(string assembly, string outputFolder)
        {
            try
            {
                if (!string.IsNullOrEmpty(assembly))
                {
                    System.Reflection.Assembly loaded = System.Reflection.Assembly.LoadFrom(assembly);
                    if (loaded != null)
                    {
                        var type = typeof(C4DocumentBase);

                        string saveToFolder = Path.GetDirectoryName(assembly);

                        if (!string.IsNullOrEmpty(outputFolder))
                        {
                            saveToFolder = outputFolder;
                        }

                        // https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
                        Type[] types = loaded.GetTypes().Where(p => type.IsAssignableFrom(p) && !p.IsInterface).ToArray();

                        foreach (Type t in types)
                        {
                            C4DocumentBase instance = (C4DocumentBase)Activator.CreateInstance(t);
                            ShowImage(ToPng(instance, saveToFolder));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        public string ToPuml(C4DocumentBase doc)
        {
            return doc.ToString();
        }

        public string ToPng(C4DocumentBase doc, string outputFolder)
        {
            string imageFileName = Path.ChangeExtension(Path.Combine(outputFolder, doc.Name), ".png");

            string encString = Encoder.EncodeUrl(doc.ToString());

            WebRenderer r = new WebRenderer();
            var bytes = r.Render(encString);
            File.WriteAllBytes(imageFileName, bytes);

            return imageFileName;
        }

        private static void ShowImage(string fileName)
        {
            System.Diagnostics.Process fileopener = new System.Diagnostics.Process();
            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + fileName + "\"";
            fileopener.Start();
        }
    }
}



