using System;
using System.Collections.Generic;

namespace DuplicateFileSearcher
{
    //Сделать ограничение для поиска среди всех логических томов
    //Сделать проверку веса файла и его наличие перед удалением
    class Common
    {
        public static String searchingDir { get; set; }

        static public String getFormattedSize(Int64 size)
        {
            String fSize = "";
            String[] measure = { "Б", "КБ", "МБ", "ГБ", "ТБ", "ПБ", "ЭБ" };

            for (Int32 i = 0; i < measure.Length; i++)
            {
                if (size < Math.Pow(1024, i + 1) * 10)
                {
                    fSize += String.Format("{0:0.##}", size / Math.Pow(1024, i)) + " " + measure[i];
                    break;
                }
            }

            return fSize;
        }
    }
}
