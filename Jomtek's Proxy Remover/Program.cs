using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jomtek_s_Proxy_Remover
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                string modulePath = "";
                int proxyDepth = 0;

                try
                {
                    modulePath = args[0];
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Error : no module selected... press ENTER to exit.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                Console.Write("Proxy depth : ");
                int.TryParse(Console.ReadLine(), out proxyDepth);

                if (proxyDepth < 1)
                {
                    Console.WriteLine("Invalid proxy depth (should be greater than 1)");
                    Thread.Sleep(700);
                    continue;
                }

                Console.Clear();

                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                ModuleDefMD module = ModuleDefMD.Load(modulePath, modCtx);

                var myProxyRemover = new ProxyRemover(module, modulePath);
                int removedProxies = myProxyRemover.RemoveProxiesFromTypes(module.Types, proxyDepth);

                if (removedProxies > 0) Console.WriteLine();
                Console.WriteLine(removedProxies + " proxies removed");
                Console.WriteLine();

                if (removedProxies > 0)
                {
                    Console.WriteLine("Saving module...");
                    myProxyRemover.SaveModule();
                }


                Console.WriteLine("Done.");
                Console.ReadLine();
                break;
            }
        }
    }
}
