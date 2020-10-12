using C4_CSharp.AssemblyDiagram;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;

namespace C4_CSharp
{
    public class AsBuilt : Command
    {
        private readonly AsBuiltCommandOptions options;

        public AsBuilt(AsBuiltCommandOptions options)
           : base("asbuilt", "Shows how a system was actually built (class diagrams).")
        {
            var assembly = new Option<string>("--assembly")
            {
                Name = "Assembly",
                Description = "The assembly to analyze.",
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

                //string outputFileName = @"c:\autoany\assemblyclasses.puml";

                //using (var filestream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                //using (var writer = new StreamWriter(filestream))
                //{
                //    var gen = new ClassDiagramGenerator(writer, "    ", Accessibilities.None, true);
                //    gen.Generate(Assembly.GetExecutingAssembly());
                //}

                //return 0;



                if (!string.IsNullOrEmpty(assembly))
                {
                    System.Reflection.Assembly loaded = System.Reflection.Assembly.LoadFrom(assembly);
                    if (loaded != null)
                    {
                        string folderName = Path.GetDirectoryName(assembly);

                        using (var stream = new MemoryStream())
                        using (var writer = new StreamWriter(stream))
                        {
                            var gen = new ClassDiagramGenerator(writer, "    ", Accessibilities.None, true);
                            gen.Generate(loaded);
                            writer.Flush();
                            string puml = Encoding.ASCII.GetString(stream.ToArray());

                            File.WriteAllText(Path.ChangeExtension(Path.Combine(folderName, Path.GetFileName(assembly)), ".puml"), puml);

                            string imageFileName = Path.ChangeExtension(Path.Combine(folderName, Path.GetFileName(assembly)), ".png");

                            ToPng(puml, imageFileName);

                            ShowImage(imageFileName);
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

        public string ToPng(string puml, string pngFileName)
        {
            string encString = Encoder.EncodeUrl(puml);

            WebRenderer r = new WebRenderer();
            var bytes = r.Render(encString);
            File.WriteAllBytes(pngFileName, bytes);

            return pngFileName;
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
