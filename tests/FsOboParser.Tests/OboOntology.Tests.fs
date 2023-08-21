﻿namespace FsOboParser.Tests

open Expecto
open FsOboParser


module OboOntologyTests =

    [<Tests>]
    let oboOntologyTest =
        testList "OboOntology" [
            let testTerm1 = 
                OboTerm.Create(
                    "id:1", 
                    Name = "testTerm1", 
                    Relationships = ["related_to id:2"; "unrelated_to id:3"; "antirelated_to id:4"]
                )
            let testTerm2 = 
                OboTerm.Create(
                    "id:2", 
                    Name = "testTerm2", 
                    Relationships = ["related_to id:1"]
                )
            let testTerm3 = 
                OboTerm.Create(
                    "id:3", 
                    Name = "testTerm3", 
                    Relationships = ["unrelated_to id:1"],
                    IsA = ["id:1"; "id:2"]
                )
            let testTerm4 =
                OboTerm.Create(
                    "id:5",
                    Name = "testTerm4"
                )
            let testOntology = OboOntology.create [testTerm1; testTerm2; testTerm3; testTerm4] []
            testList "GetRelatedTerms" [
                testCase "returns correct related terms" <| fun _ ->
                    let actual = testOntology.GetRelatedTerms(testTerm1)
                    let expected = [testTerm1, "related_to", Some testTerm2; testTerm1, "unrelated_to", Some testTerm3; testTerm1, "antirelated_to", None]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "GetIsAs" [
                testCase "returns correct related terms" <| fun _ ->
                    let actual = testOntology.GetIsAs testTerm3
                    let expected = [testTerm3, Some testTerm1; testTerm3, Some testTerm2]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
            testList "GetRelations" [
                testCase "returns correct TermRelations" <| fun _ ->
                    let actual = testOntology.GetRelations()
                    let expected = [
                        Target ("related_to", testTerm1, testTerm2)
                        Target ("unrelated_to", testTerm1, testTerm3)
                        TargetMissing ("antirelated_to", testTerm1)
                        Target ("related_to", testTerm2, testTerm1)
                        Target ("unrelated_to", testTerm3, testTerm1)
                        Target ("is_a", testTerm3, testTerm1)
                        Target ("is_a", testTerm3, testTerm2)
                        Empty testTerm4
                    ]
                    Expect.sequenceEqual actual expected "is not equal"
            ]
        ]