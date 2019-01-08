using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Async
{
    public class SomeClass
    {
        public SemaphoreSlim mutex = new SemaphoreSlim(1, 2);
        string error = null;

        public Task MethodAcync(string data)
        {
            var tsc = new TaskCompletionSource<object>();            

            Task.Run(() =>
            {

                try
                {
                   
               // throw new Exception();


                    switch (data)
                    {
                        case "1":
                            Console.WriteLine("{0}", data);
                            break;
                        case "2":
                            throw new Exception();
                         // Console.WriteLine("{0}", data);
                         // break;
                        case "3":
                            Console.WriteLine("{0}", data);
                            break;
                        default:
                            break;
                    }
                    tsc.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tsc.TrySetException(ex);
                }
            });
            return tsc.Task;
        }

        public async Task<SendData> QueryMethodAsync(string data)
        {
            SendData sendDada = null; string error = null;
            await mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                await this.MethodAcync(data).ContinueWith(innerTask =>
                {
                  //  throw null;
                    if (innerTask.Status == TaskStatus.Faulted)
                    {
                        foreach (var err in innerTask.Exception.InnerExceptions)
                        {
                            error += err;
                        }
                        sendDada = new SendData { Data = false, Error = error };
                    }
                    if (innerTask.Status == TaskStatus.RanToCompletion)
                        sendDada = new SendData { Data = true, Error = "good" };

                }, TaskContinuationOptions.PreferFairness | TaskContinuationOptions.ExecuteSynchronously).ConfigureAwait(false);
            }
            finally
            {
                mutex.Release();
            }
            return sendDada;
            //return await Task.FromResult( new SendData());
        }
    }

    public class SendData
    {
        public bool Data { get; set; }
        public string Error { get; set; }
    }
}
