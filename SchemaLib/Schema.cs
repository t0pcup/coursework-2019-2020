using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SchemaLib
{
    public class Schema
    {
        /// <summary>
        /// Экземпляр класса KMeans, на основе которого составляется схема изображения.
        /// </summary>
        public KMeans kMeans;

        /// <summary>
        /// Словарь, который предоставляет кодировки ближайшего цвета у производителей по ключу - индексу цвета в списке кластеров.
        /// </summary>
        public Dictionary<int, Threads> recolored = new Dictionary<int, Threads>();

        /// <summary>
        /// Список строк, представляющих условные изображения с соответствующими фонами в LaTeX.
        /// </summary>
        readonly List<string> legend = new List<string>();

        /// <summary>
        /// Список строк, представляющих цвета, оставленные в изображении.
        /// </summary>
        readonly List<string> colorsDefine = new List<string>();

        /// <summary>
        /// Список строк, представляющих цвета нитей, наиболее близких по цвету к оставленным в изображении.
        /// </summary>
        readonly List<string> threadColorsDefine = new List<string>();

        /// <summary>
        /// Список строк, представляющих цвета для фонов условных изображений.
        /// </summary>
        readonly List<string> colorsNames = new List<string>();

        /// <summary>
        /// Массив строк, представляющих макеты условных изображений в LaTeX.
        /// </summary>
        public string[] icons = Design.icons;

        /// <summary>
        /// Строка, представляющая исходный LaTeX файл.
        /// </summary>
        public string file;

        /// <summary>
        /// Конструктор класса Schema.
        /// </summary>
        /// <param name="k"> Передаваемый объект класса KMeans. </param>
        public Schema(KMeans k) => kMeans = k;

        /// <summary>
        /// Метод конвертации символа в число.
        /// </summary>
        /// <param name="symbol"> Цифра в шестнадцатеричной системе счисления. </param>
        /// <returns> Возвращает эквивалентное число в десятичной системе счисления. </returns>
        static byte Convert(char symbol)
        {
            try
            {
                return byte.Parse(symbol.ToString());
            }
            catch
            {
                switch (char.ToUpper(symbol))
                {
                    case 'A':
                        return 10;
                    case 'B':
                        return 11;
                    case 'C':
                        return 12;
                    case 'D':
                        return 13;
                    case 'E':
                        return 14;
                    case 'F':
                        return 15;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Метод для перевода цвета из HEX в RGB представление.
        /// </summary>
        /// <param name="colorCode"> Строка, содержащая в себе код цвета в шестрадцатиричной СС. </param>
        /// <returns> Возвращает новый объект - цвет, равный переданному в строке. </returns>
        static Color HexToRGB(string colorCode)
        {
            int r, g, b;
            r = Convert(colorCode[0]) * 16 + Convert(colorCode[1]);
            g = Convert(colorCode[2]) * 16 + Convert(colorCode[3]);
            b = Convert(colorCode[4]) * 16 + Convert(colorCode[5]);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Метод для сериализации полученной базы данных в XML.
        /// </summary>
        /// <param name="name"> Название создаваемого файла. </param>
        /// <param name="list"> Список объектов для сериализации. </param>
        public static void XmlSerializeThreads(string name, List<Threads> list)
        {
            XmlSerializer formatter = new XmlSerializer(list.GetType());
            using (FileStream fs = new FileStream(name + ".xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, list);
            }
        }

        /// <summary>
        /// Метод для прочтения информации о нитях из HTML, записи информации о них в XML-файл.
        /// </summary>
        public static void ReadThreads()
        {
            List<Threads> threads = new List<Threads>();
            using (StreamReader sr = new StreamReader("Threads.html"))
            {
                string stringLine;
                string[] dataLine = new string[5];
                while ((stringLine = sr.ReadLine()) != null && stringLine.Length > 0)
                {
                    stringLine = stringLine.Replace(@"<tr>", string.Empty).Replace(@"</tr>", string.Empty);
                    int begining, ending;
                    for (int i = 0; i < 4; ++i)
                    {
                        begining = stringLine.IndexOf(@"<td>") + @"<td>".Length;
                        ending = stringLine.IndexOf(@"</td>");
                        dataLine[i] = stringLine.Substring(begining, ending - begining);
                        stringLine = stringLine.Replace(dataLine[i], string.Empty).Replace(@"<td></td>", string.Empty);
                    }
                    begining = stringLine.IndexOf("#") + 1;
                    dataLine[4] = stringLine.Remove(0, begining).Substring(0, 6);
                    Color c = HexToRGB(dataLine[4]);
                    threads.Add(new Threads(dataLine[0], dataLine[1], dataLine[2], dataLine[3], c));
                }
            }
            XmlSerializeThreads("Threads", threads);
        }

        /// <summary>
        /// Метод для десериализации файла с данными о кодировках нитей разных цветов в компаниях из XML.
        /// </summary>
        /// <param name="name"> Путь к файлу для десериализации. </param>
        /// <returns> Возращает новый объект - список информации о каждом из цветов нитей. </returns>
        public static List<Threads> XmlDeSerializeThreads(string name)
        {
            List<Threads> res = new List<Threads>();
            XmlSerializer formatter = new XmlSerializer(res.GetType());
            using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
            {
                res = formatter.Deserialize(fs) as List<Threads>;
            }
            return res;
        }

        /// <summary>
        /// Метод для представления строки в качестве цвета.
        /// </summary>
        /// <param name="s"> Строка со значениями красного, зеленого, синего спектров. </param>
        /// <returns> Возвращает новый объект - цвет со значениями из данной строки. </returns>
        Color ToColor(string[] s) => Color.FromArgb(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));

        /// <summary>
        /// Метод для заполнения списка с информацией о нитях из XML-файла.
        /// </summary>
        public void GetDataBase()
        {
            List<Threads> threads = XmlDeSerializeThreads("Threads.xml");
            for (int i = 0; i < kMeans.centres.Count; ++i)
            {
                List<double> distances = new List<double>();
                List<Threads> t = new List<Threads>();
                foreach (var item in threads)
                {
                    distances.Add(KMeans.Euclid(kMeans.centres[i], ToColor(item.color.Split())));
                    t.Add(item);
                }
                recolored.Add(i, t[distances.IndexOf(distances.Min())]);
            }
        }

        /// <summary>
        /// Метод для форматирования цвета из структуры Color в цвет LaTeX.
        /// </summary>
        /// <param name="c"> Цвет для форматирования. </param>
        /// <returns> Возвращает новый объект - строку, представляющую цвет на языке LaTeX. </returns>
        string Str(Color c) => $"}}{{RGB}}{{{c.R}, {c.G}, {c.B}}}";

        /// <summary>
        /// Метод для отображения элемента с переданным индексом в легенде.
        /// </summary>
        /// <param name="i"> Индекс элемента для отображения. </param>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string AddItem(int i)
        {
            string s = "\\draw[thick, black";
            string new_icon = icons[i % icons.Length].Replace(",N*0.5+", $",{i}+");
            new_icon = new_icon.Replace("N*0.5+", "2.5+");
            string res = new_icon.Substring(0, s.Length);
            res += $", fill = {colorsNames[i]}{new_icon.Substring(s.Length)}";
            return res;
        }

        /// <summary>
        /// Метод для заполнения списков информацией, необходимой для создания схемы.
        /// </summary>
        public void Fill()
        {
            for (int i = 0; i < kMeans.centres.Count; ++i)
            {
                colorsDefine.Add($"\\definecolor{{mycolor{i}" + Str(kMeans.centres[i]));
                threadColorsDefine.Add($"\\definecolor{{threadcolor{i}" + Str(ToColor(recolored[i].color.Split())));
                colorsNames.Add($"mycolor{i}");
                legend.Add(string.Empty);
            }
            foreach (var item in kMeans.colors)
            {
                legend[kMeans.colors[item.Key]] = AddItem(kMeans.colors[item.Key]);
            }
        }

        /// <summary>
        /// Метод для замещения запятых на точки при переводе в строку вещественного значения. 
        /// </summary>
        /// <param name="x"> Вещественное число. </param>
        /// <returns> Возвращает новый объект - строку, содержащую отформатированное вещественное число. </returns>
        string PlaceDots(double x) => x.ToString().Replace(",", ".");

        /// <summary>
        /// Метод для определения использумемых цветов в преамбуле файла LaTeX.
        /// </summary>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string DefineColors()
        {
            string s = string.Empty;
            foreach (string i in colorsDefine)
            {
                s += i + "\n";
            }
            foreach (string i in threadColorsDefine)
            {
                s += i + "\n";
            }
            return s;
        }

        /// <summary>
        /// Метод для отображения элемента с переданным индексом на переданных координатах.
        /// </summary>
        /// <param name="i"> Индекс элемента для отображения. </param>
        /// <param name="p"> Точка, в которой необходимо его отобразить на схеме. </param>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string DrawIcon(int i, Point p)
        {
            double he = kMeans.wr.Height;
            string s = "\\draw[thick, black";
            string new_icon = icons[i % icons.Length].Replace(",N", $",{(he - p.Y).ToString().Replace(",", ".")}");
            new_icon = p.X == 0 ? new_icon.Replace("N*0.5+", string.Empty) : new_icon.Replace("N", $"{p.X}");
            string res = new_icon.Substring(0, s.Length);
            res += ", fill = " + colorsNames[i] + new_icon.Substring(s.Length);
            return res;
        }

        /// <summary>
        /// Метод для создания легенды и карты цветов для схемы.
        /// </summary>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string Info()
        {
            string s = "\\begin{tikzpicture}\n";
            for (int i = 0; i < kMeans.centres.Count; ++i)
            {
                s += $"\\draw [thick, black, fill = mycolor{i}] (0,{i}+0) -- (0.5,{i}+0) node [above right] " +
                    $"{{$~is~symbol~$}} -- (0.5,{i}+0.5) -- (0,{i}+0.5) -- cycle;\n" + legend[i] +
                    $"\\draw (3.5,{i}+0) node [above right] {{$~we~recommend~{recolored[i]}$}};\n" +
                    $"\\draw [thick, black, fill = threadcolor{i}] (16,{i}+0) -- (16.5,{i}+0) -- " +
                    $"(16.5,{i}+0.5) -- (16,{i}+0.5) -- cycle;\n";
            }
            s += "\\end{tikzpicture}\n";
            return s;
        }

        /// <summary>
        /// Метод для генерации схемы изображения в виде векторной графики.
        /// </summary>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string VectorSchema()
        {
            string schema = string.Empty;
            foreach (var item in kMeans.colors)
            {
                int i = kMeans.colors[item.Key];
                schema += DrawIcon(kMeans.colors[item.Key], new Point(item.Key.X, item.Key.Y + 1));
            }
            return schema;
        }

        /// <summary>
        /// Метод для составления строк с отметками координат.
        /// </summary>
        /// <param name="wi"> Ширина переданного изображения. </param>
        /// <param name="he"> Высота переданного изображения. </param>
        /// <returns> Возвращает новый объект - строку, отформатированную как код LaTeX. </returns>
        string Cordinates(double wi, double he)
        {
            string cord = string.Empty;
            for (int i = 0; i <= wi; i += 10)
            {
                cord += $"\\draw ({i / 2},{PlaceDots(he / 2)}) node [above]{{${i}$}};\n";
            }
            for (int i = 0; i <= he; i += 10)
            {
                cord += $"\\draw (0,{PlaceDots(i / 2)}) node [left]{{${i}$}};\n";
            }
            return cord;
        }

        /// <summary>
        /// Метод для генерации полного кода LaTeX с использованием информации об изображении.
        /// </summary>
        /// <param name="name"> Название исходного изображения. </param>
        public void Generate(string name)
        {
            double wi = kMeans.wr.Width, he = kMeans.wr.Height;
            file = "\\documentclass{article}\n" +
                "\\usepackage[utf8]{inputenc}\n" +
                "\\usepackage[english]{babel}\n" +
                "\\usepackage{tikz}\n" +
                "\\usepackage[left=25mm, top=20mm, right=10mm, bottom=10mm, nofoot]{geometry}\n" +
                $"\\geometry{{papersize={{{PlaceDots(wi / 2.0 + 10)}cm, {PlaceDots(he / 2.0 + recolored.Count + 10)}cm}}}}\n" +
                "\\usepackage{xcolor}\n" +
                DefineColors() +
                "\\begin{document}\n" +
                "\\begin{center}\n" +
                $"\\section*{{{name}-image schema}}\\\\[15pt]\n" +
                "\\subsubsection*{This is automaticly generated~\\LaTeX~file}\n" +
                "\\end{center}\\\\[10pt]\n" +
                "\\begin{center}\n" +
                Info() +
                "\\end{center}\\\\[20pt]\n" +
                "\\begin{center}\n" +
                "\\begin{tikzpicture}\n" +
                VectorSchema() +
                $"\\draw[step=0.5, white, thin] (0,0) grid +({PlaceDots(wi / 2.0)},{PlaceDots(he / 2.0)});\n" +
                $"\\draw[step=5, black, ultra thick] (0,0) grid +({PlaceDots(wi / 2.0)},{PlaceDots(he / 2.0)});\n" +
                Cordinates(wi, he) +
                "\\end{tikzpicture}\n" +
                "\\end{center}\n" +
                "\\end{document}";
        }
    }
}