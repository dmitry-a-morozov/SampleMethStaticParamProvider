
#r @"bin\SampleMethStaticParamProvider.dll"


open ExampleTypeProvider

let x = ExampleType()

x.ExampleMethWithStaticParam<1>(1) |> printfn "%d"
x.ExampleMethWithStaticParam<2>(1,2) |> printfn "%d"
x.ExampleMethWithStaticParam<3>(1,2,3) |> printfn "%d"

let dmitry = ExampleType.CreateBag<"Name,Age">(Name = "Dmitry", Age = 40) 
dmitry.Name <- "Alexei"
dmitry.Name, dmitry.Age
let don = ExampleType.CreateBag<"Name,Age,Title">(Name = "Don", Age = 21, Title = "Principal Researcher")
don.Name, don.Age, don.Title 
