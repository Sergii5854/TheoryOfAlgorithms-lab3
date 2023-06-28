using System;
using System.Collections.Generic;

public class Key
{
    public string Stock { get; }
    public int DayOfYear { get; }

    public Key(string stock, int dayOfYear)
    {
        Stock = stock;
        DayOfYear = dayOfYear;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Stock.GetHashCode();
            hash = hash * 23 + DayOfYear.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Key other = (Key)obj;
        return Stock == other.Stock && DayOfYear == other.DayOfYear;
    }
}

public class HashTable
{
    private const int DefaultInitialCapacity = 16;
    private const double DefaultLoadFactor = 0.75;

    private int capacity;
    private int size;
    private double loadFactor;
    private Dictionary<Key, double>[] buckets;

    public HashTable(int initialCapacity, double loadFactor)
    {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException("initialCapacity", "Initial capacity must be greater than zero.");

        if (loadFactor <= 0 || loadFactor >= 1)
            throw new ArgumentOutOfRangeException("loadFactor", "Load factor must be between 0 and 1.");

        capacity = initialCapacity;
        this.loadFactor = loadFactor;
        size = 0;
        buckets = new Dictionary<Key, double>[capacity];
    }

    public HashTable(int initialCapacity) : this(initialCapacity, DefaultLoadFactor)
    {
    }

    public HashTable() : this(DefaultInitialCapacity, DefaultLoadFactor)
    {
    }

    public void Put(Key key, double value)
    {
        if (size >= capacity * loadFactor)
            Resize();

        int bucketIndex = GetBucketIndex(key);
        if (buckets[bucketIndex] == null)
            buckets[bucketIndex] = new Dictionary<Key, double>();

        buckets[bucketIndex][key] = value;
        size++;
    }

    public double Get(Key key)
    {
        int bucketIndex = GetBucketIndex(key);
        if (buckets[bucketIndex] != null && buckets[bucketIndex].ContainsKey(key))
        {
            return buckets[bucketIndex][key];
        }

        // Повертати захищений значок, якщо ключ не знайдений
        return 0.0;
    }

    public bool ContainsKey(Key key)
    {
        int bucketIndex = GetBucketIndex(key);
        return buckets[bucketIndex] != null && buckets[bucketIndex].ContainsKey(key);
    }

    public double Remove(Key key)
    {
        int bucketIndex = GetBucketIndex(key);
        if (buckets[bucketIndex] != null && buckets[bucketIndex].ContainsKey(key))
        {
            double value = buckets[bucketIndex][key];
            buckets[bucketIndex].Remove(key);
            size--;
            return value;
        }

        // Повертати захищений значок, якщо ключ не знайдений
        return 0.0;
    }

    public int Size()
    {
        return size;
    }

    private int GetBucketIndex(Key key)
    {
        int hashCode = key.GetHashCode();
        return Math.Abs(hashCode % capacity);
    }

    private void Resize()
    {
        int newCapacity = capacity * 2;
        Dictionary<Key, double>[] newBuckets = new Dictionary<Key, double>[newCapacity];

        foreach (var bucket in buckets)
        {
            if (bucket != null)
            {
                foreach (var kvp in bucket)
                {
                    int newIndex = GetBucketIndex(kvp.Key);
                    if (newBuckets[newIndex] == null)
                        newBuckets[newIndex] = new Dictionary<Key, double>();

                    newBuckets[newIndex][kvp.Key] = kvp.Value;
                }
            }
        }

        capacity = newCapacity;
        buckets = newBuckets;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Test1();
        Console.WriteLine("\n---------------------------\n");
        Test2();
    }
    static void Test1()
    {
        HashTable ht = new HashTable(16, 0.75);
        ht.Put(new Key("APPL", 223), 180.0);
        ht.Put(new Key("META", 300), 160.34);

        double metaPrice = ht.Get(new Key("META", 300));
        Console.WriteLine(metaPrice); // Output: 160.34
    }
    static void Test2()
    {
        HashTable ht = new HashTable(4, 0.75);

        // Додавання елементів до хеш-таблиці
        ht.Put(new Key("APPL", 223), 180.0);
        ht.Put(new Key("META", 300), 160.34);
        ht.Put(new Key("GOOG", 123), 250.0);
        ht.Put(new Key("AMZN", 365), 350.5);

        // Перевірка розміру хеш-таблиці
        Console.WriteLine("Size: " + ht.Size()); // Output: Size: 4

        // Додавання елемента, що спричинить розширення хеш-таблиці
        ht.Put(new Key("MSFT", 200), 300.75);

        // Перевірка розміру хеш-таблиці після розширення
        Console.WriteLine("Size: " + ht.Size()); // Output: Size: 5

        // Отримання значення за ключем
        double metaPrice = ht.Get(new Key("META", 300));
        Console.WriteLine("META price: " + metaPrice); // Output: META price: 160.34

        // Видалення елемента з хеш-таблиці
        double removedPrice = ht.Remove(new Key("GOOG", 123));
        Console.WriteLine("Removed price: " + removedPrice); // Output: Removed price: 250.0

        // Перевірка наявності ключа в хеш-таблиці
        bool containsKey = ht.ContainsKey(new Key("AMZN", 365));
        Console.WriteLine("Contains key: " + containsKey); // Output: Contains key: true
    }
}
