using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Async
{
    class Program
    {

        private SomeClass _someClass;        

        public Program()
        {
            this._someClass = new SomeClass();
        }
        static void Main(string[] args)
        {

            M();

        }

        public static void M ()
        {
            var a = new Program().Method("");
            Console.WriteLine("{0}    {1}", a.Data, a.Error);

        }

        public SendData Method(string data)
        {
            return  MethodAcync(data).GetAwaiter().GetResult();
        }

        public async Task<SendData>  MethodAcync(string data)
        {         
            string error = null;//_someClass.QueryMethodAsync(data)
            var result = await _someClass.QueryMethodAsync(data).ContinueWith(innerTask =>
            {
                if (innerTask.Status == TaskStatus.Faulted)
                {
                    foreach (var err in innerTask.Exception.InnerExceptions)
                    {
                        error += err;
                    }
                    return new SendData { Data = false, Error = error };
                }

                // sendData = innerTask.Result;
                // if(innerTask.Status ==TaskStatus.RanToCompletion)
                return innerTask.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result;
        }

    }
}
