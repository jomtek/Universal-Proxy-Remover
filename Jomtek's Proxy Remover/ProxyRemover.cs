using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jomtek_s_Proxy_Remover
{
    class ProxyRemover
    {
        private ModuleDef module;
        private string modulePath;
        private Dictionary<string, OpCode> opcodesDict;

        public ProxyRemover(ModuleDef module, string modulePath)
        {
            this.module = module;
            this.modulePath = modulePath;

            opcodesDict = new Dictionary<string, OpCode>();
            opcodesDict.Add("System.String", OpCodes.Ldstr);
            opcodesDict.Add("System.Int32", OpCodes.Ldc_I4);
            opcodesDict.Add("System.SByte", OpCodes.Ldc_I4_S);
            opcodesDict.Add("System.Boolean", OpCodes.Ldc_I4_1);
        }

        private bool IsTypeAnalyzable(TypeDef moduleType) =>
            moduleType.HasMethods && !moduleType.Namespace.Contains(".My");

        private bool IsProxyTypeAnalyzable(string proxyType) {
            return new string[] { "System.String", "System.Int32", "System.SByte", "System.Boolean" }.Contains(proxyType);
        }

        private bool IsMethodProxy(MethodDef method, IList<Instruction> instructions) =>
            method.HasReturnType && !method.HasParams() && instructions.Count == 2; 
    
        private Dictionary<MDToken, object> AnalyzeMethods(IList<MethodDef> methods)
        {
            var proxyDict = new Dictionary<MDToken, object>();
            
            foreach (MethodDef method in methods)
            {
                if (method.Body != null)
                {
                    IList<Instruction> methodInstructions = method.Body.Instructions;

                    if (IsMethodProxy(method, methodInstructions) && IsProxyTypeAnalyzable(method.ReturnType.ToString()))
                    {
                        object methodReturnValue = methodInstructions[0].Operand; // from the ld instruction, just before the ret
                        OpCode ldOpcode = methodInstructions[0].OpCode;

                        if (method.ReturnType.ToString() == "System.Boolean")
                            methodReturnValue = ldOpcode == OpCodes.Ldc_I4_1;

                        proxyDict[method.MDToken] = methodReturnValue;
                    }
                }
            }

            return proxyDict;
        }

        private MethodDef[] RemoveProxies(IList<MethodDef> methods, Dictionary<MDToken, object> proxies)
        {
            var removedMethods = new List<MethodDef>();

            foreach (MethodDef method in methods)
            {
                if (method.Body != null)
                {
                    int counter = 0;
                    for (int i = 0; i < method.Body.Instructions.Count - 1; i++)
                    {
                        Instruction instruction = method.Body.Instructions[counter];

                        if (instruction.OpCode == OpCodes.Call && counter > 0)
                        {
                            try
                            {
                                MethodDef calledFunc = (MethodDef)instruction.Operand;
                                if (proxies.ContainsKey(calledFunc.MDToken))
                                {
                                    object resolvedValue = proxies[calledFunc.MDToken];
                                    string resolvedValueType = resolvedValue.GetType().ToString();

                                    if (IsProxyTypeAnalyzable(resolvedValueType))
                                    {
                                        if (method.Body.Instructions[counter - 1].OpCode == OpCodes.Ldarg_0)
                                        {
                                            method.Body.Instructions.RemoveAt(counter - 1);
                                            counter--;
                                        }

                                        removedMethods.Add(calledFunc);
                                        proxies.Remove(calledFunc.MDToken);

                                        instruction.OpCode = opcodesDict[resolvedValueType];

                                        if (resolvedValueType == "System.Boolean")
                                            if (!(bool)resolvedValue) instruction.OpCode = OpCodes.Ldc_I4_0;
                                            else
                                                instruction.Operand = resolvedValue;

                                        Console.WriteLine("Removed proxy " + calledFunc.MDToken.ToString());
                                    }
                                }
                            } catch (InvalidCastException) {}
                        }

                        counter++;
                    }
                }
            }

            return removedMethods.ToArray();
        }

        private void RemoveUnusedProxies(TypeDef moduleType, Dictionary<MDToken, object> globalProxiesList)
        {
            int counter = 0;
            for (int x = 0; x < moduleType.Methods.Count; x++) // Remove unused proxies
            {
                MethodDef method = moduleType.Methods[counter];

                if (globalProxiesList.ContainsKey(method.MDToken))
                {
                    method.Body.Instructions.Clear();
                    moduleType.Methods.RemoveAt(counter);
                    globalProxiesList.Remove(method.MDToken);
                    counter--;
                }

                counter++;
            }
        }
        public int RemoveProxiesFromTypes(IEnumerable<TypeDef> moduleTypes, int cycles)
        {
            var globalProxiesList = new Dictionary<MDToken, object>();
            int totalRemovedMethods = 0;

            for (int i = 0; i < cycles; i++)
            {
                int removedMethodsThisCycle = 0;

                // Analyze types
                foreach (TypeDef moduleType in moduleTypes)
                {
                    if (IsTypeAnalyzable(moduleType))
                    {
                        if (moduleType.DeclaringType != null && !IsTypeAnalyzable(moduleType.DeclaringType)) continue;

                        Dictionary<MDToken, object> foundProxies =
                            AnalyzeMethods(moduleType.Methods);

                        globalProxiesList = globalProxiesList.Union(foundProxies).ToDictionary(x => x.Key, x => x.Value);
                    }
                }

                // Remove proxies
                foreach (TypeDef moduleType in moduleTypes)
                {
                    if (IsTypeAnalyzable(moduleType))
                    {
                        if (moduleType.DeclaringType != null && !IsTypeAnalyzable(moduleType.DeclaringType)) continue;

                        MethodDef[] removedMethods = RemoveProxies(moduleType.Methods, globalProxiesList);
                        removedMethodsThisCycle += removedMethods.Count();

                        foreach (MethodDef method in removedMethods) // Remove used proxies
                            moduleType.Methods.Remove(method);
                    }
                }

                // Remove unused proxies
                foreach (TypeDef moduleType in moduleTypes)
                {
                    RemoveUnusedProxies(moduleType, globalProxiesList);
                }

                totalRemovedMethods += removedMethodsThisCycle;

                if (removedMethodsThisCycle == 0)
                    break;
            }

            return totalRemovedMethods;
        }

        public void SaveModule()
        {
            string savePath = Path.ChangeExtension(modulePath, null);

            var moduleOptions = new ModuleWriterOptions(module);
            moduleOptions.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
            moduleOptions.MetadataLogger = DummyLogger.NoThrowInstance;

            module.Write(savePath + "_fixed" + Path.GetExtension(modulePath), moduleOptions);
        }
    }
}
