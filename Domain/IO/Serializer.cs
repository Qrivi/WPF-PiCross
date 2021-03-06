﻿using System;
using System.IO;
using System.Linq;

namespace IO
{
    public interface ISerializer<T>
    {
        void Write(StreamWriter writer, T obj);

        T Read(StreamReader reader);
    }

    public abstract class ReaderBase
    {
        protected readonly StreamReader streamReader;

        protected ReaderBase(StreamReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException("streamReader");
            }
            this.streamReader = streamReader;
        }

        protected int ReadInteger()
        {
            return ReadIntegers(1)[0];
        }

        protected int[] ReadIntegers(int? expectedCount = null)
        {
            var line = streamReader.ReadLine();
            var ns = line.Split(' ').Select(int.Parse).ToArray();

            if (expectedCount.HasValue && ns.Length != expectedCount.Value)
            {
                throw new IOException(string.Format("Expected {0} integers"), expectedCount.Value);
            }
            return ns;
        }

        protected string[] ReadLines(int count)
        {
            return Enumerable.Range(0, count).Select(_ => streamReader.ReadLine()).ToArray();
        }
    }

    public abstract class WriterBase
    {
        protected readonly StreamWriter streamWriter;

        protected WriterBase(StreamWriter streamWriter)
        {
            if (streamWriter == null)
            {
                throw new ArgumentNullException("streamWriter");
            }
            this.streamWriter = streamWriter;
        }

        protected void Write(int[] ns)
        {
            streamWriter.WriteLine(string.Join(" ", ns.Select(n => n.ToString())));
        }
    }
}