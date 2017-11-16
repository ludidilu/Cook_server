using System;
using System.Threading;
using Cook_lib;
using Connection;
using System.Diagnostics;

namespace FinalWar_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection.Log.Init(Console.WriteLine);

            Cook_lib.Log.Init(Console.WriteLine);

            ResourceLoad();

            Server<PlayerUnit> server = new Server<PlayerUnit>();

            server.Start("0.0.0.0", ConfigDictionary.Instance.port, 100, 12000);

            Stopwatch watch = new Stopwatch();

            int tickTime = 1000 / CookConst.TICK_NUM_PER_SECOND;

            while (true)
            {
                watch.Reset();

                server.Update();

                BattleManager.Instance.Update();

                watch.Stop();

                int time = tickTime - (int)watch.ElapsedMilliseconds;

                Thread.Sleep(time);
            }
        }

        private static void ResourceLoad()
        {
            ConfigDictionary.Instance.LoadLocalConfig("local.xml");

            StaticData.path = ConfigDictionary.Instance.table_path;

            StaticData.Load<DishSDS>("dish");

            StaticData.Load<ResultSDS>("result");

            Cook_server.Init(StaticData.GetDic<DishSDS>(), StaticData.GetDic<ResultSDS>());
        }
    }
}
