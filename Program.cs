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
        private LogItem[] buffer;
        private int bufferSize;
        private int currentIndex;

        public event EventHandler<string> NewLogAdded;

        public CircularLogBuffer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new LogItem[bufferSize];
            currentIndex = 0;
        }

        public void ResizeBuffer(int newSize)
        {
            // Vytvoří se nové pole s novou velikostí
            LogItem[] newBuffer = new LogItem[newSize];

            // Přesune se obsah stávajícího pole do nového pole
            int minSize = Math.Min(newSize, bufferSize);
            Array.Copy(buffer, newBuffer, minSize);

            // Nastaví se nová velikost bufferu a buffer se přepíše na nové pole
            bufferSize = newSize;
            buffer = newBuffer;
        }

        public void ClearBuffer()
        {
            // Vyčistí se obsah bufferu
            Array.Clear(buffer, 0, bufferSize);
        }

        public void AddLog(DateTime timestamp, string content)
        {
            // Přidá se nový záznam do bufferu
            buffer[currentIndex] = new LogItem(timestamp, content);

            // Inkrementuje se index a zkontroluje se, zda index nepřekročil velikost bufferu
            currentIndex = (currentIndex + 1) % bufferSize;

            // Vyvolá se událost s obsahem nového záznamu
            NewLogAdded?.Invoke(this, content);
        }

        public LogItem[] GetLogs()
        {
            // Vytvoří se kopie pole
            LogItem[] copy = new LogItem[bufferSize];
            Array.Copy(buffer, copy, bufferSize);

            // Seřadí se záznamy podle časového razítka
            Array.Sort(copy);

            // Vrátí se seřazené pole
            return copy;
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

            while (true)
            {
                // Přidání logů
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                else
                {
                    logBuffer.AddLog(DateTime.Now, input);
                }
            }

            // Získání seznamu logů a jejich výpis
            LogItem[] logs = logBuffer.GetLogs();
            foreach (var log in logs)
            {
                Console.WriteLine($"Čas: {log.Timestamp}, Obsah: {log.Content}");
            }
            Console.Read();
        }
    }
}