using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Canguro.Model.Load
{
    [Serializable]
    public class ResponseSpectrum : ICloneable
    {
        private float[,] function;
        private string name;

        public static IList<ResponseSpectrum> ReadDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + @"\RuntimeData\rspectrum");
            
            if (!di.Exists)
                di = new DirectoryInfo(@"rspectrum");

            // Create an array representing the files in the current directory.
            FileInfo[] fi = di.GetFiles();

            IList<ResponseSpectrum> list = new List<ResponseSpectrum>();
            foreach (FileInfo fiTemp in fi)
            {
                try
                {
                    if (fiTemp.Extension.ToLower().Equals(".rsp"))
                        list.Add(new ResponseSpectrum(fiTemp.FullName));
                }
                catch (Exception) { }
            }

            return new ManagedList<ResponseSpectrum>(list);
        }

        public ResponseSpectrum(string fileName)
        {
            Load(fileName);
        }

        private void Load(string file)
        {
            try
            {
                name = Path.GetFileNameWithoutExtension(file);
                StreamReader reader = File.OpenText(file);
                string line = reader.ReadLine();
                int len = Convert.ToInt32(line);
                function = new float[len, 2];
                char[] separators = "\t ".ToCharArray();
                for (int i = 0; i < len; i++)
                {
                    line = reader.ReadLine();
                    string[] values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    function[i, 0] = Convert.ToSingle(values[0]);
                    function[i, 1] = Convert.ToSingle(values[1]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid rsp File", ex);
            }
        }

        public float[,] Function
        {
            get { return function; }
        }

        public override string ToString()
        {
            return name;
        }

        private ResponseSpectrum(ResponseSpectrum copy)
        {
            name = copy.name;
            function = (float[,])copy.function.Clone();
        }

        /// <summary>
        /// Método heredado de IClonable
        /// </summary>
        /// <returns>Regresa una copia superficial (completa en este caso) de sí mismo</returns>
        public object Clone()
        {
            return new ResponseSpectrum(this);
        }
    }
}
