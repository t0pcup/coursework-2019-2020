using System;
using System.Drawing;

namespace SchemaLib
{
    [Serializable]
    public class Threads
    {
        /// <summary>
        /// Кодировка цвета нити у Anchor.
        /// </summary>
        public string anchor;

        /// <summary>
        /// Кодировка цвета нити у Gamma.
        /// </summary>
        public string gamma;

        /// <summary>
        /// Кодировка цвета нити у DMC.
        /// </summary>
        public string dmc;

        /// <summary>
        /// Кодировка цвета нити у Madeira.
        /// </summary>
        public string madeira;
        
        /// <summary>
        /// Цвет нити.
        /// </summary>
        public string color;

        public Threads() { }

        public Threads(string a, string g, string d, string m, Color c)
        {
            anchor = a;
            gamma = g;
            dmc = d;
            madeira = m;
            color = $"{c.R} {c.G} {c.B}";
        }

        /// <summary>
        /// Переопределенный метод, предоставляет сведения о кодировках нити у разных производителей.
        /// </summary>
        /// <returns> Возвращает строковое представление об экземпляре класса нити. </returns>
        public override string ToString()
            => $"Anchor: { anchor.Trim('a')}, Gamma: {gamma}, DMC: {dmc}, Madeira: {madeira}";
    }
}
