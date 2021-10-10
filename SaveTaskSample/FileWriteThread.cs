using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace SaveTaskSample
{
    public class FileWriteThread
    {
        private Queue<string> dataQueue = new Queue<string>();
        private CancellationTokenSource cancelTokenSource;
        private StreamWriter streamWriter;
        public string SavePath { get; set; }
        public bool IsSaving { get; private set; }

        public FileWriteThread()
        {
            SavePath = "temp.txt";
        }
        public FileWriteThread(string path)
        {
            SavePath = path;
        }

        public void Start()
        {
            cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            streamWriter = new StreamWriter(SavePath);
            dataQueue.Clear();

            Task.Run(() => SaveTask(token));

        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
        }

        public void SetWriteData(string data)
        {
            lock (dataQueue)
            {
                dataQueue.Enqueue(data);
            }
        }

        private void SaveTask(CancellationToken ct)
        {
            try
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    if (dataQueue.Count > 0)
                    {
                        string data;
                        lock (dataQueue)
                        {
                            data = dataQueue.Dequeue();
                        }
                        streamWriter.WriteLine(data);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception)
            {
                streamWriter.Close();
            }
        }
    }
}
