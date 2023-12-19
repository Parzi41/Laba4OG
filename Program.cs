using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

class Program
{
    static void Main()
    {
        // Зчитуємо датасет з файлу
        List<Point> dataset = ReadDatasetFromFile("DS2.txt");

        // Виконуємо афінне перетворення (обертання)
        List<Point> transformedDataset = PerformAffineTransformation(dataset, 10 * (2 + 1));

        // Встановлюємо розміри вікна (полотна – canvas size) 960x960 пікселів
        int canvasWidth = 960;
        int canvasHeight = 960;

        // Відображаємо датасет після афінного перетворення точками синього кольору
        using (Bitmap bitmap = new Bitmap(canvasWidth, canvasHeight))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White); // Заливаємо білім кольором

            foreach (Point point in transformedDataset)
            {
                g.FillEllipse(Brushes.Blue, point.X - 5, point.Y - 5, 10, 10);
            }

            // Зберігаємо результат у файл графічного формату (PNG у цьому випадку)
            bitmap.Save("output.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        Console.WriteLine("Transformation completed. Results saved to output.png");
    }

    static List<Point> ReadDatasetFromFile(string filePath)
    {
        List<Point> dataset = new List<Point>();

        // Читаємо кожен рядок з файлу та розбиваємо його на координати X та Y
        foreach (string line in File.ReadLines(filePath))
        {
            string[] coordinates = line.Split(' ', '\n', '\r');
            int x = int.Parse(coordinates[0]);
            int y = int.Parse(coordinates[1]);
            dataset.Add(new Point(x, y));
        }

        return dataset;
    }

    static List<Point> PerformAffineTransformation(List<Point> dataset, double angle)
    {
        List<Point> transformedDataset = new List<Point>();

        double radians = angle * Math.PI / 180.0;
        double cosTheta = Math.Cos(radians);
        double sinTheta = Math.Sin(radians);

        // Визначаємо матрицю трансформації
        double[,] transformationMatrix = {
            { cosTheta, -sinTheta, 0 },
            { sinTheta, cosTheta, 0 },
            { (1 - cosTheta) * 480 + sinTheta * 480, -sinTheta * 480 + (1 - cosTheta) * 480, 1 }
        };

        // Застосовуємо матрицю трансформації до кожної точки
        foreach (Point point in dataset)
        {
            double x = point.X * transformationMatrix[0, 0] + point.Y * transformationMatrix[1, 0] + transformationMatrix[2, 0];
            double y = point.X * transformationMatrix[0, 1] + point.Y * transformationMatrix[1, 1] + transformationMatrix[2, 1];
            transformedDataset.Add(new Point((int)x, (int)y));
        }

        return transformedDataset;
    }
}
