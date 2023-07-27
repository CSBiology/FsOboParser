#I "src/FsOboParser/bin/Debug/netstandard2.0"
#r "FsOboParser.dll"

#r "nuget: IsaDotNet"

open FsOboParser

open System.IO


let testPath = Path.Combine(__SOURCE_DIRECTORY__,  "./../../nfdi4plants/arc-validate/ErrorClassOntology.obo")

OboEntries.fromFile true testPath
let testOntology = OboOntology.fromFile true testPath

let testTerms = [
    OboTerm.Create("test:000", Name = "test0")
    OboTerm.Create("test:001", Name = "test1a", IsA = ["test:000"])
    OboTerm.Create("test:002", Name = "test2", IsA = ["test:001"])
    OboTerm.Create("test:003", Name = "test1b", IsA = ["test:000"])
]

let testOntology = OboOntology.create testTerms []

testOntology.GetChildOntologyAnnotations(testTerms.Head.Id)
testOntology.GetChildOntologyAnnotations(testTerms.Head.Id, Depth = 1)
testOntology.GetChildOntologyAnnotations(testTerms.Head.Id, Depth = 2)

//let fileLines = File.ReadAllLines testPath

//OboTerm.fromLines true ((fileLines |> Seq.ofArray).GetEnumerator()) 0

//OboOntology.toFile "myOboOntology.obo" testOboOntology

//Path.Combine("myOboOntology.obo") |> FileInfo

let myOboTerm = OboTerm.Create("TO:00000000", Name = "testTerm", CreatedBy = "myself")

//let myOboTerm = 
//    OboTerm.create 
//        "TO:00000000" 
//        (Some "testTerm") 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        None 
//        (Some "myself") 
//        None

//OboTypeDef.Create

let myOntology = OboOntology.create [myOboTerm] []

//OboOntology.toFile "myOboOntology.obo" myOntology

let termOfInterest = testOntology.Terms[5]

let isAs = testOntology.GetParentOntologyAnnotations(termOfInterest.Id)
// output is an ISADotNet.OntologyAnnotation list

let isAsTerms = isAs |> List.map (fun oa -> testOntology.GetTerm(oa.TermAccessionString.ToString()))
// output is an OboTerm list