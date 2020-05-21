using System.Xml.Serialization;
using System.IO;

namespace SchemaLib
{

    public class Design
    {
        /// <summary>
        /// Массив строк, представляющих макеты условных изображений в LaTeX.
        /// </summary>
        static public string[] icons= new string[] 
        {
            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol1\n"+
            "\\draw[fill = black] (N*0.5+0.25,N*0.5+0.25) circle(0.5*0.35cm);\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol2\n" +
            "\\draw[fill = black] (N*0.5+0.25,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.25) -- " +
            "(N*0.5+0.25,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.25) -- cycle;\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol3\n" +
            "\\draw[fill = black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.25,N*0.5+0.5) -- cycle;\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol4\n" +
            "\\draw[fill = black] (N*0.5+0.5,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.25) -- " +
            "(N*0.5+0,N*0.5+0.25) -- cycle;\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol5\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0.5) -- (N*0.5+0.25,N*0.5+0);\n" +
            "\\draw[ultra thick, black] (N*0.5+0.25,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.5);\n" +
            "\\draw[ultra thick, black] (N*0.5+0.25,N*0.5+0) -- (N*0.5+0.25,N*0.5+0.5);\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol6\n" +
            "\\draw[ultra thick] (N*0.5+0.25,N*0.5+0.25) circle(0.5*0.35cm);\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol7\n" +
            "\\draw[ultra thick, black] (N*0.5+0.25,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.25) -- " +
            "(N*0.5+0.25,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.25) -- cycle;\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.5);\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0.5) -- (N*0.5+0.5,N*0.5+0);\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol8\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0.5);\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0.5) -- (N*0.5+0.5,N*0.5+0);\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol9\n" +
            "\\draw[fill = black] (N*0.5+0,N*0.5+0.5) -- (N*0.5+0.25,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- cycle;\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol10\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0.5) -- (N*0.5+0.25,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- cycle;\n",

            "\\draw[thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.5,N*0.5+0.5) -- (N*0.5+0,N*0.5+0.5) -- cycle; %symbol11\n" +
            "\\draw[ultra thick, black] (N*0.5+0,N*0.5+0) -- (N*0.5+0.5,N*0.5+0) -- " +
            "(N*0.5+0.25,N*0.5+0.5) -- cycle;\n"
        };

        /// <summary>
        /// Конструктор класса Design.
        /// </summary>
        public Design() { }

        /// <summary>
        /// Метод для сериализации макетов изображений в XML.
        /// </summary>
        /// <param name="name"> Название создаваемого файла. </param>
        /// <param name="str"> Массив объектов для сериализации. </param>
        static public void XmlSerializeIcons(string name, Design design)
        {
            XmlSerializer formatter = new XmlSerializer(design.GetType());
            using (FileStream fs = new FileStream(name + ".xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, design);
            }
        }

        /// <summary>
        /// Метод для десериализации макетов изображений из XML.
        /// </summary>
        /// <param name="name"> Название считываемого файла. </param>
        /// <returns> Возвращает новый объект - массив строк, представляющих макеты изображений в LaTeX. </returns>
        static public Design XmlDeSerializeIcons(string name)
        {
            Design res = new Design();
            XmlSerializer formatter = new XmlSerializer(res.GetType());
            using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
            {
                res = formatter.Deserialize(fs) as Design;
            }
            return res;
        }
    }
}