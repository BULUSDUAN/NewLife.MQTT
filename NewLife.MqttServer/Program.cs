﻿using System;
using NewLife.Agent;
using NewLife.Caching;
using NewLife.Log;
using NewLife.MQTT;
using NewLife.Remoting;

namespace NewLife.CacheServer
{
    class Program
    {
        static void Main(String[] args) => new MyService().Main();

        class MyService : AgentServiceBase<MyService>
        {
            public MyService()
            {
                ServiceName = "MqttServer";
                DisplayName = "MQTT服务器";
            }

            private MqttServer _Server;
            protected override void StartWork(String reason)
            {
                // 配置
                var set = Setting.Current;

                // 服务器
                var svr = new MqttServer(set.Port)
                {
                    //Encoder = new BinaryEncoder(),
                    Log = XTrace.Log,
                    ShowError = true,
                };

                if (set.Debug) svr.EncoderLog = XTrace.Log;

                // 网络层日志
                var ns = svr.EnsureCreate() as Net.NetServer;
                ns.SessionLog = XTrace.Log;
                ns.SocketLog = XTrace.Log;
#if DEBUG
                ns.LogSend = true;
                ns.LogReceive = true;
#endif

                // 统计日志
                svr.StatPeriod = 10;

                //// 缓存提供者
                //var mc = new MemoryCache();
                //if (set.Expire > 0) mc.Expire = set.Expire;

                //// 注册RPC服务
                //var svc = new CacheService { Cache = mc };
                //svr.Register(svc, null);

                //svr.Start();

                //_Server = svr;

                base.StartWork(reason);
            }

            protected override void StopWork(String reason)
            {
                _Server.TryDispose();
                _Server = null;

                base.StopWork(reason);
            }
        }
    }
}