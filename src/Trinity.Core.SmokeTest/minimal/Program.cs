using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Trinity.Network;
using Trinity.Diagnostics;
using System.Linq;

namespace minimal
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Debug;
            TrinityConfig.StorageRoot  = Environment.CurrentDirectory;
            Global.LocalStorage.ResetStorage();
            TrinityServer server = new TrinityServer();
            server.Start();
            server.Stop();
            Task.WhenAll(
                Enumerable.Range(0, 1000)
                          .Select(i => Global.LocalStorage.SaveCellAsync(i, new byte[i])))
                .Wait();
            Global.LocalStorage.SaveStorage();
            Global.LocalStorage.LoadStorage();

            for(int i = 0; i<1000; ++i)
            {
                var result = Global.LocalStorage.LoadCellAsync(i).Result;
                byte[] cell_content = result.CellBuff;
                Assert(TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                Assert(i == cell_content.Length);
            }

            // let's also assert that there are no other cells in the storage...
            for(int i=1001; i < 10000; ++i)
            {
                Assert(!Global.LocalStorage.Contains(i));
            }

            Global.Uninitialize();
            Environment.Exit(0);
        }

        static void Assert(bool x)
        {
            if (!x)
            {
                Log.WriteLine("Assertion failed, stack trace: {0}", Environment.StackTrace);
                throw new Exception("Assertion failed");
            }
        }
    }
}
