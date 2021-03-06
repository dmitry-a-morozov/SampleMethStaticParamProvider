#r @"..\..\bin\SampleMethStaticParamProvider.dll"

type ExampleType = ExampleTypeProvider.ExampleType

let x = ExampleType()

let v1 = x.ExampleMethWithStaticParam<1>(1) 
let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 

let bag = ExampleType.CreateBag<"Name,Grand Slams">("Roger Federer", 17)
bag.Name
bag.``Grand Slams``