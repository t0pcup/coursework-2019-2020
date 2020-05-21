using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SchemaLib
{
    public class KMeans
    {
        /// <summary>
        /// Поле для получения случайных чисел, чтобы инициализировать начальные значения центров кластеров.
        /// </summary>
        static readonly Random rand = new Random();

        /// <summary>
        /// Публичное поле, изображение, на основе его данных производится кластеризация.
        /// </summary>
        public ImageWrapper wr;

        /// <summary>
        /// Публичное поле, изображение результата кластеризации.
        /// </summary>
        public Bitmap resImage;

        /// <summary>
        /// Публичное поле, словарь, возвращающий индекс кластера, к которому относится цвет ключа (точки).
        /// </summary>
        public Dictionary<Point, int> colors = new Dictionary<Point, int>();

        /// <summary>
        /// Приватное поле, список цветов - значения каждого центра кластеров.
        /// </summary>
        public List<Color> centres;

        /// <summary>
        /// Приватное поле, массив цветов, необходимый для запоминания значений центров кластеров на предыдущей итерации.
        /// </summary>
        Color[] oldCentres;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="img"> Изображение, на основе которого будет производиться кластеризация. </param>
        /// <param name="clusters"> Количество кластеров, между которыми должны распределиться цвета изображения. </param>
        public KMeans(Bitmap img, int clusters)
        {
            using (ImageWrapper imagewrapper = new ImageWrapper(img))
            {
                wr = imagewrapper;
            }
            AlphaFix();
            resImage = new Bitmap(wr.Width, wr.Height);
            oldCentres = new Color[clusters];
            centres = new List<Color>();
            for (int i = 0; i < clusters; ++i)
            {
                int wi = rand.Next(wr.Width), he = rand.Next(wr.Height);
                centres.Add(wr[wi, he]);
            }
            for (int w = 0; w < wr.Width; ++w)
            {
                for (int h = 0; h < wr.Height; ++h)
                {
                    List<double> distances = new List<double>();
                    foreach (Color c in centres)
                    {
                        distances.Add(Euclid(c, wr[w, h]));
                    }
                    colors.Add(new Point(w, h), distances.IndexOf(distances.Min()));
                }
            }
        }

        /// <summary>
        /// Адаптирует альфа канал изображений.
        /// </summary>
        void AlphaFix()
        {
            for (int w = 0; w < wr.Width; ++w) 
            {
                for (int h = 0; h < wr.Height; ++h) 
                {
                    if (wr[w, h].A <= 60)
                    {
                        wr.SetPixel(new Point(w, h), 255, 255, 255);
                    }
                }
            }
        }

        /// <summary>
        /// Метод проверяющий, равны ли все старые центры кластеров новым центрам.
        /// </summary>
        /// <returns> 
        /// Возвращает логическое значение истинности высказывания об оставлении новых центров на местах старых. 
        /// </returns>
        public bool Static()
        {
            uint res = 0;
            for (int i = 0; i < centres.Count(); ++i)
            {
                if (centres[i] == oldCentres[i])
                {
                    ++res;
                }
            }
            return res == centres.Count();
        }

        /// <summary>
        /// Метод для расчета евклидова расстояния между двумя цветами.
        /// </summary>
        /// <param name="color1"> Первый цвет. </param>
        /// <param name="color2"> Второй цвет. </param>
        /// <returns> Возвращает вещественное значение - расстояние между двумя цветами. </returns>
        static public double Euclid(Color color1, Color color2)
        {
            byte[] a = { color1.R, color1.G, color1.B };
            byte[] b = { color2.R, color2.G, color2.B };
            double sum = 0;
            for (int i = 0; i < a.Count(); ++i)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Метод, относящий каждый элемент изображения к одному из кластеров.
        /// </summary>
        public void Group()
        {
            for (int w = 0; w < wr.Width; ++w)
            {
                for (int h = 0; h < wr.Height; ++h)
                {
                    List<double> distances = new List<double>();
                    foreach (Color c in centres)
                    {
                        distances.Add(Euclid(c, wr[w, h]));
                    }
                    colors[new Point(w, h)] = distances.IndexOf(distances.Min());
                }
            }
        }

        /// <summary>
        /// Метод для создания нового цвета центра кластера из средних значений красного, зеленого, синего спектров его группы.
        /// </summary>
        /// <param name="group"> Список цветов, относящихся к выбранному кластеру. </param>
        /// <returns> 
        /// Возвращает цвет, составленный их средних арифметических значений красного, зеленого, синего спектров группы. 
        /// </returns>
        private Color Average(List<Color> group)
        {
            int amount = group.Count();
            int r = 0, g = 0, b = 0;
            for (int i = 0; i < amount; ++i)
            {
                r += group[i].R;
                g += group[i].G;
                b += group[i].B;
            }
            return Color.FromArgb(r / amount, g / amount, b / amount);
        }

        /// <summary>
        /// Метод, осуществляющий шаг в процессе стабилизации кластеров.
        /// </summary>
        public void Stabilize()
        {
            centres.CopyTo(oldCentres);
            Group();
            for (int i = 0; i < centres.Count(); ++i)
            {
                List<Color> group = new List<Color>();
                foreach (var item in colors.Where(x => centres[x.Value] == centres[i]))
                {
                    group.Add(wr[item.Key.X, item.Key.Y]);
                }
                if (group.Count() != 0)
                {
                    centres[i] = Average(group);
                }
            }
        }

        /// <summary>
        /// Метод для удаления кластеров, у которых есть эквивалентные замены среди оставшихся кластеров.
        /// </summary>
        public void Regroup()
        {
            List<int> unbanned = new List<int>();
            List<Color> new_centres = new List<Color>();
            for (int i = centres.Count() - 1; i >= 0; --i)
            {
                for (int j = centres.Count() - 1; j >= 0; --j)
                {
                    if (i != j && centres[i] == centres[j] && !unbanned.Contains(centres.LastIndexOf(centres[i])))
                    {
                        unbanned.Add(i);
                        break;
                    }
                }
            }
            foreach (var item in unbanned)
            {
                new_centres.Add(centres[item]);
            }
            if (new_centres.Count > 1)
            {
                centres.Clear();
                centres = new_centres;
            }
            Group();
        }

        /// <summary>
        /// Метод для обновления изображения полученного в итоге кластеризации исходного изображения.
        /// </summary>
        public void RefreshImage()
        {
            foreach (var item in colors)
            {
                resImage.SetPixel(item.Key.X, item.Key.Y, centres[item.Value]);
            }
        }

        /// <summary>
        /// Переопределенный метод, предоставляет сведения о количестве кластеров, примененных к изображению.
        /// </summary>
        /// <returns> Возвращает строковое представление о количестве кластеров экземпляра класса. </returns>
        public override string ToString() => $"Сохранено {centres.Count} цветов картинки";
    }
}