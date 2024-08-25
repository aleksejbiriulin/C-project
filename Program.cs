// using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static System.Net.Mime.MediaTypeNames;
// using System.Reflection.Metadata;
using System.Collections.Generic;

Console.WriteLine("Hello");
var orderManager = new OrderManager();
// orderManager.RegisterPlugin(new OrderPlugin());
orderManager.RegisterPlugin(new ExcelExportPlugin());
orderManager.RegisterPlugin(new WordReportPlugin());
orderManager.RegisterPlugin(new PdfGraphPlugin());
orderManager.RegisterPlugin(new Email());

// Добавление заказов
byte[] photoBytes = new byte[]
{
    0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48,
    0x00, 0x48, 0x00, 0x00, 0xFF, 0xE1, 0x00, 0x22, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00, 0x4D, 0x4D,
    0x00, 0x2A, 0x00, 0x00, 0x00, 0x08, 0x00, 0x01, 0x01, 0x12, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01,
    0x00, 0x01, 0x00, 0x00, 0x01, 0x1A, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x62,
    0x01, 0x1B, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x6A, 0x01, 0x28, 0x00, 0x03,
    0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x00, 0x01, 0x31, 0x00, 0x02, 0x00, 0x00, 0x00, 0x10,
    0x00, 0x00, 0x00, 0x72, 0x01, 0x32, 0x00, 0x02, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x7C,
    0x87, 0x69, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01,
    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
    0xFF, 0xD9
};

orderManager.AddOrder("John Doe", "john.doe@example.com", new[] { "Product A", "Product B" }, photoBytes);
orderManager.AddOrder("Jane Smith", "jane.smith@example.com", new[] { "Product C" }, photoBytes);

// Получение списка заказов
var orders = orderManager.GetOrders();

public interface IOrderPlugin
{
    void export(List<Order> orders);
    void prov(Order order);

}

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public string[] Products { get; set; }
    public byte[] Photo { get; set; }
}
public class OrderManager
{
    private readonly List<IOrderPlugin> _plugins = new List<IOrderPlugin>();
    private readonly List<Order> _orders = new List<Order>();
    public static Dictionary<string, int> count_product = new Dictionary<string, int>();

    public void RegisterPlugin(IOrderPlugin plugin)
    {
        _plugins.Add(plugin);
    }

    public void AddOrder(string customerName, string email, string[] products, byte[] photo)
    {
        var order = new Order
        {
            Id = _orders.Count + 1,
            CustomerName = customerName,
            Email = email,
            Products = products,
            Photo = photo
        };
        foreach (var plugin in _plugins)
        {
            plugin.prov(order);

            foreach (var i in order.Products)
            {
                int value;
                if (!count_product.TryGetValue(i, out value))
                {
                    count_product.Add(i, 1);
                }
                else
                {
                    count_product[i] = value + 1;
                }

            }
        }
        _orders.Add(order);
        foreach (var plugin in _plugins)
        {
            plugin.export(_orders);
            Console.WriteLine(1);
        }

    }

    public List<Order> GetOrders()
    {
        return _orders;
    }

}

public class ExcelExportPlugin : IOrderPlugin
{
    public void export(List<Order> orders)
    {
        // Реализуйте экспорт данных в Excel с фотографиями
    }
    public void prov(Order order)
    {

    }
}

public class WordReportPlugin : IOrderPlugin
{
    public void export(List<Order> orders)
    {
        // Реализуйте формирование отчета в Word
    }
    public void prov(Order order)
    {

    }

}

public class PdfGraphPlugin : IOrderPlugin
{
    public void export(List<Order> orders)
    {
        // Данные для гистограммы
        var data = new Dictionary<string, int>
        {
            { "Категория 1", 10 },
            { "Категория 2", 15 },
            { "Категория 3", 8 },
            { "Категория 4", 12 },
            { "Категория 5", 20 }
        };

        // Создаем PDF-документ
        var document = new iTextSharp.text.Document();
        var pdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new System.IO.FileStream("histogram.pdf", System.IO.FileMode.Create));
        document.Open();
        PdfContentByte cb = pdfWriter.DirectContent;

        // Создаем гистограмму
        CreateHistogram(cb, data);

        document.Close();


    }



    private static void CreateHistogram(PdfContentByte cb, Dictionary<string, int> data)
    {
        // Вычисляем максимальное значение для масштабирования гистограммы
        int maxValue = data.Values.Max();

        // Настраиваем внешний вид гистограммы
        float barWidth = 50f;
        float barSpacing = 20f;
        float chartHeight = 300f;
        float chartWidth = (barWidth + barSpacing) * data.Count + barSpacing;

        // Создаем графический объект PDF
        // PdfContentByte cb = document.DirectContent;

        // Начинаем рисование гистограммы
        cb.SaveState();
        cb.MoveTo(50, 50);
        cb.LineTo(50 + chartWidth, 50);
        cb.LineTo(50 + chartWidth, 50 + chartHeight);
        cb.LineTo(50, 50 + chartHeight);
        cb.ClosePathStroke();

        int x = 70;
        foreach (var item in data)
        {
            // Рисуем столбец гистограммы
            float barHeight = (float)item.Value / maxValue * chartHeight;
            cb.Rectangle(x, 50, barWidth, barHeight);
            cb.Fill();

            // Добавляем подпись под столбцом
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, item.Key, x + barWidth / 2, 30, 0);
            cb.EndText();

            x += (int)(barWidth + barSpacing);
        }

        cb.RestoreState();
    }
    public void prov(Order order)
    {

    }
}
public class Email : IOrderPlugin
{
    public void export(List<Order> orders)
    {
    }
    public void prov(Order order)
    {
        // Регулярное выражение для проверки формата email
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(order.Email, pattern))
        {
            throw new FormatException("Неправильный формат email.");
        }
    }
}
