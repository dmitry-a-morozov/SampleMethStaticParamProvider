//From: https://skillsmatter.com/skillscasts/5138-creating-type-providers-hands-on-with-michael-newton
//End time: 1:00:00
module FirstProvider.TP

open ProviderImplementation.ProvidedTypes //provides APIs
open Microsoft.FSharp.Core.CompilerServices //for code quotations
open System.Reflection
open System.Collections.Generic
open Microsoft.FSharp.Quotations

[<TypeProvider>]
type public MyTypeProvider() as this = 
    inherit TypeProviderForNamespaces()
    let asm = Assembly.GetExecutingAssembly()
    let ns = "ExampleTypeProvider"
    let newType = ProvidedTypeDefinition(asm, ns, "ExampleType", Some typeof<obj>)
  
    let helloWorld = ProvidedProperty("Hello", typeof<string>, IsStatic = true, GetterCode = (fun _ -> <@@ 333  @@>))
    let cons = ProvidedConstructor([], InvokeCode = fun _ -> <@@ 3 :> obj @@>)
    let paramCons = 
        ProvidedConstructor
            ([ ProvidedParameter("state", typeof<int>) ], 
            InvokeCode = fun argsx -> <@@ (%%(argsx.[0]) : int) :> obj @@>)
    let exampleProperty = 
        ProvidedProperty
            ("State", typeof<int>, IsStatic = false, 
            GetterCode = (fun argsy -> <@@ (%%argsy.[0] :> obj) :?> int @@>))

    let exampleMeth = 
        ProvidedMethod
            ("ExampleMeth", [ ProvidedParameter("x", typeof<int>) ], typeof<string>, IsStaticMethod = false, 
            InvokeCode = fun [ self; prefix ] -> <@@ (%%prefix : int) + (%%self :> obj :?> int) @@>)


    let staticParams = [ProvidedStaticParameter("Count", typeof<int>)]
    let exampleMethWithStaticParams =  
        let m = ProvidedMethod("ExampleMethWithStaticParam", [ ], typeof<int>, IsStaticMethod = false)
        m.DefineStaticParameters(staticParams, (fun nm args ->
            let arg = args.[0] :?> int
            let parms = [ for i in 1 .. arg -> ProvidedParameter("arg" + string i, typeof<int>)]
            let m2 = 
                ProvidedMethod(nm, parms, typeof<int>, IsStaticMethod = false,
                                InvokeCode = fun args -> <@@ arg @@>)
            newType.AddMember m2
            m2))
        m

//static params provided return type
    let createBag = 
        let staticParams = [ProvidedStaticParameter("Properties", typeof<string>)]
        let m = ProvidedMethod("CreateBag", [], typeof<obj>, IsStaticMethod = true)
        m.DefineStaticParameters(staticParams, (fun methodName args ->
            let props = [| for x in (unbox<string> args.[0]).Split(',') -> x.Trim() |]

            failwith "Test. Haha!"

            let resultTypeName = sprintf "%s_Result" methodName
            let resultType = ProvidedTypeDefinition(resultTypeName, Some typeof<obj>, HideObjectMethods = true)
            for name in props do
                let prop = ProvidedProperty(name, typeof<obj>)
                prop.GetterCode <- fun args -> <@@ (%%Expr.Coerce(args.[0], typeof<Dictionary<string, obj>>): Dictionary<string, obj>).[name] @@>
                prop.SetterCode <- fun args -> <@@ (%%Expr.Coerce(args.[0], typeof<Dictionary<string, obj>>): Dictionary<string, obj>).[name] <- %%args.[1] @@>
                resultType.AddMember prop
            newType.AddMember resultType
            let params' = [ for name in props -> ProvidedParameter(name, typeof<obj>)]
            let impl = ProvidedMethod(methodName, params', resultType, IsStaticMethod = true)
            impl.InvokeCode <- fun args -> 
                <@@ 
                    let values: obj[] = %%Expr.NewArray(typeof<obj>, args)
                    let data = (props, values) ||> Array.zip |> dict
                    Dictionary<string, obj>(data)
                @@>
            newType.AddMember impl
            impl
        ))
        m

    do 
        newType.AddMember helloWorld
        newType.AddMember cons //Note: you can also add subtypes with AddMember
        newType.AddMember paramCons
        newType.AddMember exampleProperty
        newType.AddMember exampleMeth
        newType.AddMember exampleMethWithStaticParams
        newType.AddMember createBag
  
    do this.AddNamespace(ns, [ newType ])

[<TypeProviderAssembly>]
do ()
