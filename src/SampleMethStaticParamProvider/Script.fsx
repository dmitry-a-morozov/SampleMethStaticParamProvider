#r @"..\..\bin\SampleMethStaticParamProvider.dll"

let x = ExampleTypeProvider.ExampleType()

let v1 = x.ExampleMethWithStaticParam<1>(1) 
let v2 = x.ExampleMethWithStaticParam<2>(1,2) 
let v3 = x.ExampleMethWithStaticParam<3>(1,2,3) 

let v1' = x.ExampleMethWithStaticParam<1, Fail = true>(1) 
