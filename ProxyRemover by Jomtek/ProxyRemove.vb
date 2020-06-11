Imports dnlib.DotNet
Imports dnlib.DotNet.Emit
Module ProxyRemove
    Private opcodesDict As New Dictionary(Of String, OpCode)

    Private Function IsTypeAnalyzable(moduleType As TypeDef)
        Return moduleType.HasMethods And Not moduleType.Namespace.Contains(".My")
    End Function

    Private Function IsMethodProxy(ByVal method As MethodDef, ByRef methodInstructions As IList(Of Instruction))
        Return method.HasReturnType And Not method.HasParams And methodInstructions.Count = 2
    End Function

    Private Function IsProxyTypeAnalyzable(proxyType As TypeSig)
        Dim analyzableProxyTypes = New String() {
            "System.String",
            "System.Int32",
            "System.Boolean"
        }

        Return analyzableProxyTypes.Contains(proxyType.ToString)
    End Function

    Private Function CanGetBody(method As MethodDef)
        Return method.Body IsNot Nothing
    End Function

    Private Function AnalyzeMethods(ByRef methods As IList(Of MethodDef),
                                    ByRef failedMethodAccessCount As Integer) As Dictionary(Of MDToken, Object)

        Dim proxyDict As New Dictionary(Of MDToken, Object)

        For Each method In methods
            If CanGetBody(method) Then
                Dim methodInstructions As IList(Of Instruction) = method.Body.Instructions

                If IsMethodProxy(method, methodInstructions) And IsProxyTypeAnalyzable(method.ReturnType) Then
                    Dim methodReturnValue As Object = methodInstructions(0).Operand

                    If method.ReturnType.ToString = "System.Boolean" Then
                        methodReturnValue = methodInstructions(0).OpCode Is OpCodes.Ldc_I4_1
                    End If

                    proxyDict(method.MDToken) = methodReturnValue
                End If
            Else
                failedMethodAccessCount += 1
            End If
        Next

        Return proxyDict
    End Function

    Private Function RemoveProxies(ByRef methods As IList(Of MethodDef),
                                   ByRef proxyFunctions As Dictionary(Of MDToken, Object)) As MethodDef()

        Dim removedMethods As New List(Of MethodDef)

        For Each method In methods
            If CanGetBody(method) Then
                Dim methodInstructions As IList(Of Instruction) =
                    method.Body.Instructions

                Dim counter As Integer = 0
                For i = 0 To methodInstructions.Count - 1
                    Dim instruction As Instruction = methodInstructions(counter)
                    If Not (instruction.OpCode Is OpCodes.Call) Then counter += 1 : Continue For

                    Try
                        Dim calledFunc As MethodDef = instruction.Operand
                        If proxyFunctions.ContainsKey(calledFunc.MDToken) Then
                            Dim proxyValue As Object = proxyFunctions(calledFunc.MDToken)
                            Dim proxyValueType As String = proxyValue.GetType.ToString

                            If methodInstructions(counter - 1).OpCode Is OpCodes.Ldarg_0 Then ' Small check
                                methodInstructions.RemoveAt(counter - 1)
                                counter -= 1
                            End If

                            removedMethods.Add(calledFunc)
                            proxyFunctions.Remove(calledFunc.MDToken)

                            instruction.OpCode = opcodesDict(proxyValueType)

                            If proxyValueType = "System.Boolean" Then
                                If Not proxyValue Then instruction.OpCode = OpCodes.Ldc_I4_0
                            Else
                                instruction.Operand = proxyValue
                            End If
                        End If
                    Catch ex As InvalidCastException
                        ' TODO : manage exception
                    End Try

                    counter += 1
                Next
            End If
        Next

        Return removedMethods.ToArray
    End Function

    Public Function RemoveProxiesFromTypes(moduleTypes As IEnumerable(Of TypeDef), cycles As Integer) As Integer
        Dim globalProxiesList As New Dictionary(Of MDToken, Object)
        Dim totalRemovedMethods As Integer = 0

        opcodesDict.Clear()
        opcodesDict.Add("System.String", OpCodes.Ldstr)
        opcodesDict.Add("System.Int32", OpCodes.Ldc_I4)
        opcodesDict.Add("System.Boolean", OpCodes.Ldc_I4_1)

        For i = 1 To cycles
            ' Analyze types
            For Each moduleType In moduleTypes
                If IsTypeAnalyzable(moduleType) Then
                    If moduleType.DeclaringType IsNot Nothing Then
                        If Not IsTypeAnalyzable(moduleType.DeclaringType) Then Continue For
                    End If

                    Dim failedMethodAccessCount As Integer = 0
                    Dim moduleProxies As Dictionary(Of MDToken, Object) =
                        AnalyzeMethods(moduleType.Methods, failedMethodAccessCount)

                    globalProxiesList =
                        globalProxiesList.Union(moduleProxies).ToDictionary(Function(p) p.Key, Function(p) p.Value)
                End If
            Next

            ' Remove proxies
            For Each moduleType In moduleTypes
                If IsTypeAnalyzable(moduleType) Then
                    If moduleType.DeclaringType IsNot Nothing Then
                        If Not IsTypeAnalyzable(moduleType.DeclaringType) Then Continue For
                    End If

                    Dim removedMethods As MethodDef() =
                        RemoveProxies(moduleType.Methods, globalProxiesList)

                    totalRemovedMethods += removedMethods.Count + globalProxiesList.Count

                    For Each method In removedMethods ' Remove used proxies
                        moduleType.Methods.Remove(method)
                    Next

                    For x = 0 To moduleType.Methods.Count - 1 ' Remove unused proxies
                        Dim method As MethodDef = moduleType.Methods(x)
                        If globalProxiesList.ContainsKey(method.MDToken) Then
                            moduleType.Methods.RemoveAt(x)
                        End If
                    Next
                End If
            Next
        Next

        Return totalRemovedMethods
    End Function
End Module
