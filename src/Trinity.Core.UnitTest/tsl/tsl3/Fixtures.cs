using System;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Storage;
using Xunit;

namespace tsl3
{
    public class TestServerImpl : TestServerBase
    {
        [Result] public int SynWithRspResult { get; private set; } = 0;
        public override Task TestSynWithRspHandlerAsync(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRspResult = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
            return Task.CompletedTask;
        }

        [Result] public int SynResult { get; private set; } = 0;
        public override Task TestSynHandlerAsync(ReqReader request)
        {
            SynResult = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
            return Task.CompletedTask;
        }

        public ManualResetEventSlim AsynDone { get; private set; } = new ManualResetEventSlim();
        [Result] public int AsynResult { get; private set; } = 0;
        public override Task TestAsynHandlerAsync(ReqReader request)
        {
            AsynResult = Utils.CalcForAsyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
            AsynDone.Set();
            return Task.CompletedTask;
        }

        [Result] public int SynWithRsp1Result { get; private set; } = 0;
        public override Task TestSynWithRsp1HandlerAsync(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRsp1Result = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
            return Task.CompletedTask;
        }

        [Result] public int Syn1Result { get; private set; } = 0;
        public override Task TestSyn1HandlerAsync(ReqReader request)
        {
            Syn1Result = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
            return Task.CompletedTask;
        }

        public ManualResetEventSlim Asyn1Done { get; private set; } = new ManualResetEventSlim();
        [Result] public int Asyn1Result { get; private set; } = 0;
        public override Task TestAsyn1HandlerAsync(ReqReader request)
        {
            Asyn1Result = Utils.CalcForAsyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
            Asyn1Done.Set();
            return Task.CompletedTask;
        }

        public void ResetCounts()
        {
            SynResult = AsynResult = SynWithRspResult = 0;
            Syn1Result = Asyn1Result = SynWithRsp1Result = 0;
            AsynDone.Reset();
            Asyn1Done.Reset();
        }
    }

    public class TrinityServerFixture : IDisposable
    {
        public TrinityServerFixture()
        {
            Server = new TestServerImpl();
            Server.Start();
        }

        public TestServerImpl Server { get; private set; }

        public void Dispose()
        {
            Server.Stop();
        }
    }
}
