using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Canguro.Model
{
    /// <summary>
    /// Clase que representa un catálogo, de materiales o de secciones.
    /// Wrapper de Dictionary para convertir los datos a una estructura de árbol.
    /// </summary>
    /// <typeparam name="Tvalue">Tipo de dato que almacenará el catalogo, en general sólo aplica Section y Material</typeparam>
    public class Catalog<Tvalue> : IEnumerable<Tvalue>
    {
        private Dictionary<string, Tvalue> catalog;
        private bool isReadOnly;
        private string catalogPath;

        /// <summary>
        /// Crea un catálogo vacío con nombre Default
        /// </summary>
        public Catalog() : this(Culture.Get("defaultCatalogName"), false) { }

        /// <summary>
        /// Crea un catálogo nuevo.
        /// </summary>
        /// <param name="name">El nombre del nuevo catálogo</param>
        /// <param name="isReadOnly">Los catálogos del sistema no se pueden alterar, los de usuario, sí</param>
        public Catalog(string name, bool isReadOnly)
        {
            this.name = name;
            this.isReadOnly = isReadOnly;
            catalog = new Dictionary<string, Tvalue>();
        }

        public int Count
        {
            get { return catalog.Count; }
        }

        /// <summary>
        /// Regresa el valor de si es de sólo lectura. Este valor no se puede cambiar.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                isReadOnly = value;
            }
        }
        
        /// <summary>
        /// Indizador del catálogo. Revisa que sea válido hacer cambios.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tvalue this[string key]
        {
            get
            {
                if (catalog.ContainsKey(key))
                    return catalog[key];
                return default(Tvalue);
            }
            set
            {
                if (!isReadOnly)
                {
                    Model.Instance.Undo.Change<Tvalue>(this[key], key, this);

                    if (value == null && catalog.ContainsKey(key))
                    {
                        ListChangedEventArgs<Tvalue> args = new ListChangedEventArgs<Tvalue>(catalog[key]);
                        if (ElementRemoved != null)
                            ElementRemoved(this, args);

                        if (!args.Cancel)
                            catalog.Remove(key);
                    }
                    else
                        catalog[key] = value;
                 
                    if (CatalogChanged != null) CatalogChanged(this, EventArgs.Empty);
                }
                else
                    throw new InvalidCallException(Culture.Get("EM0010"));
            }
        }

        public void MoveValue(string fromKey, string toKey)
        {
            Tvalue val = this[fromKey];
            if (val != null)
            {
                catalog.Remove(fromKey);
                catalog.Add(toKey, val);
            }
        }

        /// <summary>
        /// Función polimórfica que obtiene el nombre de un objeto para usarlo en el árbol.
        /// Está sobrecargada para Section y para Material.
        /// </summary>
        /// <param name="val">El objeto al que se le busca el nombre</param>
        /// <returns>El nombre de despliegue en el árbol</returns>
        private string GetNodeName(object val)
        {
            if (val is Section.Section)
                return ((Section.Section)val).Shape;
            else if (val is Material.Material)
                return ((Material.Material)val).DesignProperties.Name;
            else
                return "";
        }

        private string GetNodeName(Material.Material mat)
        {
            return mat.DesignProperties.Name;
        }

        private string GetNodeName(Section.Section sec)
        {
//            return sec.Name.Substring(0, sec.Name.IndexOf(' '));
            return sec.Shape;
        }
        
        /// <summary>
        /// Propiedad de sólo lectura que genera un árbol para desplegar el catálogo.
        /// </summary>
        public TreeNode Tree
        {
            get
            {
                TreeNode root = new TreeNode(Name);
                Dictionary<string, List<Tvalue>> dict = new Dictionary<string, List<Tvalue>>();
                Tvalue val;
                string nodeName;

                foreach (string key in catalog.Keys)
                {
                    val = catalog[key];
                    if (!dict.ContainsKey(nodeName = GetNodeName(val)))
                    {
                        if (!string.IsNullOrEmpty(nodeName))
                            dict.Add(nodeName, new List<Tvalue>());
                    }
                    dict[nodeName].Add(val);
                }

                foreach (string key in dict.Keys)
                {
                    TreeNode tn = new TreeNode(key);
                    foreach (Tvalue v in dict[key])
                    {
                        TreeNode tn2 = new TreeNode(v.ToString());
                        tn2.Tag = v;
                        if (key.Length > 0)
                            tn.Nodes.Add(tn2);
                        else
                            root.Nodes.Add(tn2);
                    }
                    if (key.Length > 0)
                        root.Nodes.Add(tn);
                }

                return root;
            }
        }

        /// <summary>
        /// Método que lee un archivo con un catálogo serializado.
        /// </summary>
        /// <param name="path">Ruta del archivo</param>
        public void Load(string path)
        {
            catalogPath = path;
            Stream stream = null;
            try
            {
                stream = File.Open(catalogPath, FileMode.Open, FileAccess.Read);
                BinaryFormatter bformatter = new BinaryFormatter();
                Load(stream, bformatter);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(Culture.Get("fileNotFound") + ": '" + catalogPath + "'");
                throw e;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }    
            }
            if (CatalogChanged != null) CatalogChanged(this, EventArgs.Empty);
        }
        /// <summary>
        /// Método que lee un stream con un catálogo serializado.
        /// </summary>
        /// <param name="stream">Ruta del archivo</param>
        /// <param name="bformatter">The deserializer</param>
        public void Load(Stream stream, BinaryFormatter bformatter)
        {
            name = (string)bformatter.Deserialize(stream);
            isReadOnly = (bool)bformatter.Deserialize(stream);
            catalog = (System.Collections.Generic.Dictionary<string, Tvalue>)bformatter.Deserialize(stream);
        }

        /// <summary>
        /// Método que guarda un catálogo con la ruta definida (guarda cambios)
        /// </summary>
        public void Save()
        {
            Save(catalogPath);
        }

        /// <summary>
        /// Método que guarda un catálogo en una ruta especificada (Save As)
        /// </summary>
        /// <param name="path">Ruta del archivo nuevo</param>
        public void Save(string path)
        {
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            Save(stream, bformatter);
            stream.Close();
        }

        /// <summary>
        /// Método que guarda un catálogo en un stream dado.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bformatter"></param>
        public void Save(Stream stream, BinaryFormatter bformatter)
        {
            bformatter.Serialize(stream, name);
            bformatter.Serialize(stream, isReadOnly);
            bformatter.Serialize(stream, catalog);
        }

        /// <summary>
        /// Propiedad con el nombre del catálogo.
        /// Es responsabilidad del Manager asegurar la consistencia de nombres.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Método que crea una lista de solo lectura para acceder a los elementos del catálogo.
        /// No se recomienda su uso extensivo por motivos de eficiencia.
        /// </summary>
        /// <returns></returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<Tvalue> AsReadOnly()
        {
            List<Tvalue> list = new List<Tvalue>(catalog.Count);
            foreach (Tvalue t in catalog.Values)
                list.Add(t);
            return list.AsReadOnly();
        }

        public event EventHandler CatalogChanged;

        private string name;

        #region IEnumerable<Tvalue> Members

        public IEnumerator<Tvalue> GetEnumerator()
        {
            return catalog.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return catalog.Values.GetEnumerator();
        }

        #endregion


        public delegate void ListChangedEventHandler(object sender, ListChangedEventArgs<Tvalue> args);
        public event ListChangedEventHandler ElementRemoved;
    }
}
