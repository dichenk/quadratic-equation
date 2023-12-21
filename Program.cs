using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;


class DataItem
{
    public int Id { get; set; }
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }
    public double Discriminant { get; set; }
    public bool IsDiscriminantPositive { get; set; }
    public double SquareRootDiscriminant { get; set; }
    public string? X1 { get; set; } = "не определено";
    public string? X2 { get; set; } = "не определено";
}

class Program
{
    static Dictionary<int, DataItem> objectsById = new Dictionary<int, DataItem>();
    static Queue<int>[] queues = new Queue<int>[6];
    static double[] delays = { 3.0, 1.5, 3.5, 3.1, 3.8, 3.2 };
    static int lastId = 0; // Инициализация lastId
    static DateTime startTime; // Переменная для хранения времени начала работы

    static AutoResetEvent[] autoResetEvents = new AutoResetEvent[6];

    static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(60000); // 60 секунд


static void Main(string[] args)
{
    var token = cancellationTokenSource.Token;

    for (int i = 0; i < autoResetEvents.Length; i++)
    {
        autoResetEvents[i] = new AutoResetEvent(false);
    }

    // Инициализация массива queues
    for (int i = 0; i < queues.Length; i++)
    {
        queues[i] = new Queue<int>();
    }

    startTime = DateTime.Now;
    
    // Запуск потоков
    var thread1 = new Thread(() => ProcessStage1(token));
    var thread2 = new Thread(() => ProcessStage2(token));
    var thread3 = new Thread(() => ProcessStage3(token));
    var thread4 = new Thread(() => ProcessStage4(token));
    var thread5 = new Thread(() => ProcessStage5(token));
    var thread6 = new Thread(() => ProcessStage6(token));

    thread1.Start();
    thread2.Start();
    thread3.Start();
    thread4.Start();
    thread5.Start();
    thread6.Start();

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    // while (stopwatch.Elapsed < TimeSpan.FromSeconds(15))
    // {
    //     Thread.Sleep(10000); // Проверка каждую секунду

    //     if (queues.All(q => q.Count == 0))
    //     {
    //         cancellationTokenSource.Cancel(); // Отмена, если все очереди пусты
    //         break;
    //     }
    // }

    thread1.Join();
    thread2.Join();
    thread3.Join();
    thread4.Join();
    thread5.Join();
    thread6.Join();
}

    static void ProcessStage1(CancellationToken token)
    {
        Console.WriteLine($"{DateTime.Now - startTime} — Запущен поток 1");

        string filePath = "input.txt"; // Путь к файлу с данными

            try
            {
            // Проверка наличия файла
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл '{filePath}' не найден");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now - startTime} — Файл существует");
                }                
                string[] lines;

                lines = File.ReadAllLines(filePath); // Чтение всех строк файла

                Console.WriteLine($"{DateTime.Now - startTime} — Прочитали файл");
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    var dataItem = new DataItem
                    {
                        Id = Interlocked.Increment(ref lastId),
                        A = double.Parse(parts[0].Trim()),
                        B = double.Parse(parts[1].Trim()),
                        C = double.Parse(parts[2].Trim())
                    };
                    Thread.Sleep(TimeSpan.FromSeconds(delays[0]));
                    Console.WriteLine($"{DateTime.Now - startTime} — Распаковали очередную строку");

                    objectsById[dataItem.Id] = dataItem;
                    Console.WriteLine($"{DateTime.Now - startTime} — Поток 1 добавляет ID {dataItem.Id} в очередь");
                    queues[1].Enqueue(dataItem.Id); // Добавление ID в очередь следующего потока
                    autoResetEvents[1].Set(); // Уведомление второго потока

                }
                Console.WriteLine($"{DateTime.Now - startTime} — Обработка файла завершена, ожидаем сигнала отмены");
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(1000); // Ожидание в течение короткого времени перед повторной проверкой
                }

    Console.WriteLine($"{DateTime.Now - startTime} — Поток 1 получил сигнал отмены");
            }
            catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
    }
}

static void ProcessStage2(CancellationToken token)
{
    while (true)
    {
        // Проверка, был ли сделан запрос на отмену
        if (token.IsCancellationRequested) {
            Console.WriteLine($"{DateTime.Now - startTime} — Поток 2 прерван");
            return; // Завершение работы потока
        }
        int dataItemId = -1;

            if (queues[1].Count > 0)
            {
                dataItemId = queues[1].Dequeue();
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 2 получил ID {dataItemId}");

            }
            else
            {
                autoResetEvents[1].WaitOne(TimeSpan.FromSeconds(10));
                continue;
            }

        DataItem dataItem;

        if (!objectsById.TryGetValue(dataItemId, out dataItem) || dataItem == null)
            continue; // Пропустить итерацию, если объект не найден или равен null

        dataItem.Discriminant = dataItem.B * dataItem.B - 4 * dataItem.A * dataItem.C;

        // Передача id в очередь следующего потока

            queues[2].Enqueue(dataItemId);
            autoResetEvents[2].Set();
    }
}

static void ProcessStage3(CancellationToken token)
{
    while (true)
    {
        // Проверка, был ли сделан запрос на отмену
        if (token.IsCancellationRequested) {
            Console.WriteLine($"{DateTime.Now - startTime} — Поток 3 прерван");
            return; // Завершение работы потока
        }
        int dataItemId = -1;

            if (queues[2].Count > 0)
            {
                dataItemId = queues[2].Dequeue();
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 3 получил ID {dataItemId}");

            }
            else
            {
                // Если очередь пуста, поток переходит в режим ожидания
                autoResetEvents[2].WaitOne(TimeSpan.FromSeconds(10));
                continue;
            }

        // Задержка потока
        Thread.Sleep(TimeSpan.FromSeconds(delays[2]));

        // Проверка значения дискриминанта и установка флага is_D_positive
        DataItem dataItem;
            if (!objectsById.TryGetValue(dataItemId, out dataItem))
                continue; // Если объект не найден, пропустить итерацию

            dataItem.IsDiscriminantPositive = dataItem.Discriminant >= 0;

        // Передача id в очередь следующего потока
            queues[3].Enqueue(dataItemId);
            autoResetEvents[3].Set(); // Уведомление следующего потока
    }
}

static void ProcessStage4(CancellationToken token)
{
    while (true)
    {
        // Проверка, был ли сделан запрос на отмену
        if (token.IsCancellationRequested) {
            Console.WriteLine($"{DateTime.Now - startTime} — Поток 4 прерван");
            return; // Завершение работы потока
        }
        int dataItemId = -1;

            if (queues[3].Count > 0)
            {
                dataItemId = queues[3].Dequeue();
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 4 получил ID {dataItemId}");

            }
            else
            {
                autoResetEvents[3].WaitOne(TimeSpan.FromSeconds(10)); // Переход в режим ожидания
                continue;
            }

        // Задержка потока
        Thread.Sleep(TimeSpan.FromSeconds(delays[3]));

        DataItem dataItem;
            if (!objectsById.TryGetValue(dataItemId, out dataItem))
                continue; // Если объект не найден, пропустить итерацию

            if (!dataItem.IsDiscriminantPositive)
            {
                dataItem.X1 = dataItem.X2 = "нет корней";
            }
            else
            {
                dataItem.SquareRootDiscriminant = Math.Sqrt(dataItem.Discriminant);
            }

        // Передача id в очередь следующего потока
            queues[4].Enqueue(dataItemId);
            autoResetEvents[4].Set(); // Уведомление следующего потока
    }
}

static void ProcessStage5(CancellationToken token)
{
    while (true)
    {
        // Проверка, был ли сделан запрос на отмену
        if (token.IsCancellationRequested) {
            Console.WriteLine($"{DateTime.Now - startTime} — Поток 5 прерван");
            return; // Завершение работы потока
        }
        int dataItemId = -1;
            if (queues[4].Count > 0)
            {
                dataItemId = queues[4].Dequeue();
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 5 получил ID {dataItemId}");

            }
            else
            {
                autoResetEvents[4].WaitOne(TimeSpan.FromSeconds(10)); // Переход в режим ожидания
                continue;
            }

        // Задержка потока
        Thread.Sleep(TimeSpan.FromSeconds(delays[4]));

        DataItem dataItem;
            if (!objectsById.TryGetValue(dataItemId, out dataItem))
                continue; // Если объект не найден, пропустить итерацию

            if (dataItem.IsDiscriminantPositive)
            {
                double a = dataItem.A, b = dataItem.B, d = dataItem.SquareRootDiscriminant;
                dataItem.X1 = ((-b + d) / (2 * a)).ToString();
                dataItem.X2 = ((-b - d) / (2 * a)).ToString();
            }

        // Передача id в очередь следующего потока
            queues[5].Enqueue(dataItemId);
            autoResetEvents[5].Set(); // Уведомление следующего потока
    }
}

static void ProcessStage6(CancellationToken token)
{
    while (true)
    {
        // Проверка, был ли сделан запрос на отмену
        if (token.IsCancellationRequested) {
            Console.WriteLine($"{DateTime.Now - startTime} — Поток 6 прерван");
            return; // Завершение работы потока
        }
        int dataItemId = -1;

            if (queues[5].Count > 0)
            {
                dataItemId = queues[5].Dequeue();
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 6 получил ID {dataItemId}");

            }
            else
            {
                autoResetEvents[5].WaitOne(TimeSpan.FromSeconds(10)); // Переход в режим ожидания
                continue;
            }

        // Задержка потока
        Thread.Sleep(TimeSpan.FromSeconds(delays[5]));

        DataItem dataItem;
            if (objectsById.TryGetValue(dataItemId, out dataItem))
            {
                // Запись данных в файл
                WriteResultsToFile(dataItem);
                Console.WriteLine($"{DateTime.Now - startTime} — Поток 6 записал результат в словарь ID {dataItemId}");
                // Удаление объекта из словаря
                objectsById.Remove(dataItemId);
            }
    }
}

static void WriteResultsToFile(DataItem dataItem)
{
    string resultLine = $"ID: {dataItem.Id}, A: {dataItem.A}, B: {dataItem.B}, C: {dataItem.C}, D: {dataItem.Discriminant}, X1: {dataItem.X1}, X2: {dataItem.X2}";
        File.AppendAllText("result.txt", resultLine + Environment.NewLine);
}
}