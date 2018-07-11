using sqlBuilder.Data.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlBuilder
{
    public static class FileManager
    {
        public static void CreateFile(List<string> text)
        {
            try
            {
                if (File.Exists(AppSettings.SCRIPT_PATH))
                {
                    File.Delete(AppSettings.SCRIPT_PATH);
                }

                using (StreamWriter fs = File.CreateText(AppSettings.SCRIPT_PATH))
                {
                    foreach (string line in text)
                    {
                        fs.WriteLine(line);
                        fs.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {

                // throw;

            }
        }

        public static void WriteText(string text, string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.CreateText(path);
                }
                using (StreamWriter fs = File.AppendText(path))
                {
                    fs.Write(text);
                    fs.WriteLine("");
                }
            }
            //TODO Tratar essa exceção
            catch (Exception ex)
            {

                // throw;
            }
        }
    }
}
