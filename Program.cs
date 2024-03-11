using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUOpakovani
{
    public class LogItem : IComparable<LogItem>
    {
        public DateTime Timestamp { get; }
        public string Content { get; }

        public LogItem(DateTime timestamp, string content)
        {
            Timestamp = timestamp;
            Content = content;
        }

        public int CompareTo(LogItem other)
        {
            // Porovnání podle časového razítka sestupně (od nejnovějšího k nejstaršímu)
            return other.Timestamp.CompareTo(Timestamp);
        }
    }

    public class CircularLogBuffer
    {
        private List<LogItem> buffer;
        private int bufferSize;

        public event EventHandler<string> NewLogAdded;

        public CircularLogBuffer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new List<LogItem>(bufferSize);
        }

        public void ResizeBuffer(int newSize)
        {
            // Zvětší/zmenší velikost bufferu
            bufferSize = newSize;

            // Odstraní přebytečné položky z bufferu, pokud je nová velikost menší než aktuální počet položek
            while (buffer.Count > newSize)
            {
                buffer.RemoveAt(0);
            }
        }

        public void ClearBuffer()
        {
            // Vyčistí obsah bufferu
            buffer.Clear();
        }

        public void AddLog(DateTime timestamp, string content)
        {
            // Přidá nový záznam do logu
            buffer.Add(new LogItem(timestamp, content));

            // Zkontroluje, zda byl překročen maximální počet záznamů v bufferu
            if (buffer.Count > bufferSize)
            {
                // Pokud ano, odstraní nejstarší záznam
                buffer.RemoveAt(0);
            }

            // Vyvolá událost s obsahem nového záznamu
            NewLogAdded?.Invoke(this, content);
        }

        public List<LogItem> GetLogs()
        {
            // Vrátí seznam logů seřazených sestupně podle časového razítka
            buffer.Sort();
            return buffer;
        }
    }

    public class LogConsumer
    {
        public void OnNewLogAdded(object sender, string content)
        {
            // Reakce na přidání nového záznamu do logu
            Console.WriteLine($"Nový záznam v logu: {content}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Vytvoření instance třídy CircularLogBuffer s velikostí bufferu 10
            CircularLogBuffer logBuffer = new CircularLogBuffer(10);

            // Vytvoření instance třídy LogConsumer, která bude odběratelem události o novém záznamu
            LogConsumer logConsumer = new LogConsumer();
            logBuffer.NewLogAdded += logConsumer.OnNewLogAdded;

            // Přidání několika logů
            logBuffer.AddLog(DateTime.Now, "První záznam");
            logBuffer.AddLog(DateTime.Now, "Druhý záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");
            logBuffer.AddLog(DateTime.Now, "Třetí záznam");

            // Získání seznamu logů a jejich výpis
            List<LogItem> logs = logBuffer.GetLogs();
            foreach (var log in logs)
            {
                Console.WriteLine($"Čas: {log.Timestamp}, Obsah: {log.Content}");
            }
            Console.Read();
        }
    }
}
