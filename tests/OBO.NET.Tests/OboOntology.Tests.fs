﻿namespace OBO.NET.Tests

open OBO.NET

open Expecto

open System
open System.IO


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
                    Name = "testTerm4",
                    Xrefs = [DBXref.ofString "check:1"]
                )
            let testTerm5 =
                OboTerm.Create(
                    "id:6",
                    Name = "testTerm5",
                    Synonyms = [TermSynonym.parseSynonym None 0 "\"testTerm1\" EXACT []"; TermSynonym.parseSynonym None 1 "\"testTerm2\" BROAD []"; TermSynonym.parseSynonym None 2 "\"testTerm0\" NARROW []"]
                )
            let testTerm6 =
                OboTerm.Create(
                    "check:1",
                    Name = "checkTerm1"
                )

            let testFile1Path = Path.Combine(__SOURCE_DIRECTORY__, "References", "CorrectHeaderTags.obo")
            let testFile2Path = Path.Combine(__SOURCE_DIRECTORY__, "References", "IncorrectHeaderTags.obo")
            let testFile3Path = Path.Combine(__SOURCE_DIRECTORY__, "References", "DuplicateHeaderTags.obo")
            let testFile1 = try OboOntology.fromFile false testFile1Path |> Some with _ -> None
            let testFile2 = try OboOntology.fromFile false testFile2Path |> Some with _ -> None
            let testFile3 = try OboOntology.fromFile false testFile3Path |> Some with _ -> None

            testList "fromFile" [
                testCase "can read files" <| fun _ ->
                    Expect.isSome testFile1 $"Could not read testFile1: {testFile1Path}"
                    Expect.isSome testFile2 $"Could not read testFile2: {testFile2Path}"
                    Expect.isSome testFile3 $"Could not read testFile3: {testFile3Path}"
                testCase "reads correct headers correctly" <| fun _ ->
                    let formatVersionActual                                 = Option.map (fun o -> o.FormatVersion)                                 testFile1
                    let dataVersionActual                                   = Option.map (fun o -> o.DataVersion)                                   testFile1 |> Option.flatten
                    let ontologyActual                                      = Option.map (fun o -> o.Ontology)                                      testFile1 |> Option.flatten
                    let dateActual                                          = Option.map (fun o -> o.Date)                                          testFile1 |> Option.flatten
                    let savedByActual                                       = Option.map (fun o -> o.SavedBy)                                       testFile1 |> Option.flatten
                    let autoGeneratedByActual                               = Option.map (fun o -> o.AutoGeneratedBy)                               testFile1 |> Option.flatten
                    let subsetdefsActual                                    = Option.map (fun o -> o.Subsetdefs)                                    testFile1
                    let importsActual                                       = Option.map (fun o -> o.Imports)                                       testFile1
                    let synonymtypedefsActual                               = Option.map (fun o -> o.Synonymtypedefs)                               testFile1
                    let idSpacesActual                                      = Option.map (fun o -> o.Idspaces)                                      testFile1
                    let defaultRelationshipIdPrefixActual                   = Option.map (fun o -> o.DefaultRelationshipIdPrefix)                   testFile1 |> Option.flatten
                    let idMappingsActual                                    = Option.map (fun o -> o.IdMappings)                                    testFile1
                    let remarksActual                                       = Option.map (fun o -> o.Remarks)                                       testFile1
                    let treatXrefsAsEquivalentsActual                       = Option.map (fun o -> o.TreatXrefsAsEquivalents)                       testFile1
                    let treatXrefsAsGenusDifferentiasActual                 = Option.map (fun o -> o.TreatXrefsAsGenusDifferentias)                 testFile1
                    let treatXrefsAsRelationshipsActual                     = Option.map (fun o -> o.TreatXrefsAsRelationships)                     testFile1
                    let treatXrefsAsIsAsActual                              = Option.map (fun o -> o.TreatXrefsAsIsAs)                              testFile1
                    let relaxUniqueIdentifierAssumptionForNamespacesActual  = Option.map (fun o -> o.RelaxUniqueIdentifierAssumptionForNamespaces)  testFile1
                    let relaxUniqueLabelAssumptionForNamespacesActual       = Option.map (fun o -> o.RelaxUniqueLabelAssumptionForNamespaces)       testFile1
                    let formatVersionExpected                               = "0.0.1" |> Some
                    let dataVersionExpected                                 = "0.0.1" |> Some
                    let ontologyExpected                                    = "CL" |> Some
                    let dateExpected                                        = DateTime(1970, 1, 1, 0, 0, 0) |> Some
                    let savedByExpected                                     = "Oliver Maus" |> Some
                    let autoGeneratedByExpected                             = "TalkGPT" |> Some
                    let subsetdefsExpected                                  = ["GO_SLIM \"GO Slim\""; "GO_BASIC \"GO Basic\""] |> Some
                    let importsExpected                                     = ["http://purl.obolibrary.org/obo/go.owl"; "http://purl.obolibrary.org/obo/cl.owl"] |> Some
                    let synonymtypedefsExpected                             = ["UK_SPELLING \"British spelling\" EXACT"; "US_SPELLING \"American spelling\" EXACT"] |> Some
                    let idspacesExpected                                    = ["GO urn:lsid:bioontology.org:GO: \"gene ontology terms\""; "GO urn:lsid:bioontology.org:GO: \"gene ontology types\""] |> Some
                    let defaultRelationshipIdPrefixExpected                 = "OBO_REL" |> Some
                    let idMappingsExpected                                  = ["part_of OBO_REL:part_of"; "has_a OBO_REL:has_a"] |> Some
                    let remarksExpected                                     = ["test1"; "test2"] |> Some
                    let treatXrefsAsEquivalentExpected                      = ["CL"; "GO"] |> Some
                    let treatXrefsAsGenusDifferentiaExpected                = ["CL part_of NCBITaxon:7955"; "CL part_of NCBITaxon:7956"] |> Some
                    let treatXrefsAsRelationshipExpected                    = ["MA homologous_to"; "MA analogous_to"] |> Some
                    let treatXrefsAsIsAExpected                             = ["CL"; "GO"] |> Some
                    let relaxUniqueIdentifierAssumptionForNamespaceExpected = ["my_combined_ontology"; "my_combined_ontology2"] |> Some
                    let relaxUniqueLabelAssumptionForNamespaceExpected      = ["my_combined_ontology"; "my_combined_ontology2"] |> Some
                    Expect.equal formatVersionActual formatVersionExpected "format-version is not identical"
                    Expect.equal dataVersionActual dataVersionExpected "data-version is not identical"
                    Expect.equal ontologyActual ontologyExpected "ontology is not identical"
                    Expect.equal dateActual dateExpected "date is not identical"
                    Expect.equal savedByActual savedByExpected "saved-by is not identical"
                    Expect.equal autoGeneratedByActual autoGeneratedByExpected "auto-generated-by is not identical"
                    Expect.equal subsetdefsActual subsetdefsExpected "subsetdefs is not identical"
                    Expect.equal importsActual importsExpected "imports are not identical"
                    Expect.equal synonymtypedefsActual synonymtypedefsExpected "synonymtypedefs are not identical"
                    Expect.equal idSpacesActual idspacesExpected "idspaces are not identical"
                    Expect.equal defaultRelationshipIdPrefixActual defaultRelationshipIdPrefixExpected "default-relationship-id-prefix is not identical"
                    Expect.equal idMappingsActual idMappingsExpected "id-mappings are not identical"
                    Expect.equal remarksActual remarksExpected "remarks are not identical"
                    Expect.equal treatXrefsAsEquivalentsActual treatXrefsAsEquivalentExpected "treat-xrefs-as-equivalents are not identical"
                    Expect.equal treatXrefsAsGenusDifferentiasActual treatXrefsAsGenusDifferentiaExpected "treat-xrefs-as-genus-differentia are not identical"
                    Expect.equal treatXrefsAsRelationshipsActual treatXrefsAsRelationshipExpected "treat-xrefs-as-relationships are not identical"
                    Expect.equal treatXrefsAsIsAsActual treatXrefsAsIsAExpected "treat-xrefs-as-is-a are not identical"
                    Expect.equal relaxUniqueIdentifierAssumptionForNamespacesActual relaxUniqueIdentifierAssumptionForNamespaceExpected "relax-unique-identifier-assumption-for-namespaces are not identical"
                    Expect.equal relaxUniqueLabelAssumptionForNamespacesActual relaxUniqueLabelAssumptionForNamespaceExpected "relax-unique-label-assumption-for-namespaces are not identical"
                testCase "reads incorrect headers correctly" <| fun _ ->
                    Expect.isNone (Option.map (fun o -> o.Date) testFile2 |> Option.flatten) "Date should be missing but was still parsed"
                testCase "reads Terms correctly" <| fun _ ->
                    let termsExpected = List.init 2 (fun i -> OboTerm.Create $"Test:000{i + 1}") |> Some
                    Expect.equal (Option.map (fun o -> o.Terms) testFile1) termsExpected "Terms did not match"
                testCase "reads Typedefs correctly" <| fun _ ->
                    let typedefsExpected = List.init 2 (fun i -> OboTypeDef.Create($"Test:000{i + 3}", "", "")) |> Some
                    Expect.equal (Option.map (fun o -> o.TypeDefs) testFile1) typedefsExpected "Terms did not match"
            ]

            let testOntology = OboOntology.Create([testTerm1; testTerm2; testTerm3; testTerm4; testTerm5], [], "", TreatXrefsAsEquivalents = ["check"])
            let testOntology2 = OboOntology.Create([testTerm6], [], "")

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
                        Empty testTerm5
                    ]
                    Expect.sequenceEqual actual expected "is not equal"
            ]

            testList "GetSynonyms" [
                testCase "returns correct synonymous terms" <| fun _ ->
                    let actual = testOntology.GetSynonyms testTerm5
                    let expected = seq {Exact, testTerm5, testTerm1; Broad, testTerm5, testTerm2}
                    Expect.sequenceEqual actual expected "is not equal"
            ]

            testList "TryGetSynonyms" [
                testCase "returns correct synonymous terms" <| fun _ ->
                    let actual = testOntology.TryGetSynonyms testTerm5
                    let expected = seq {Exact, testTerm5, Some testTerm1; Broad, testTerm5, Some testTerm2; Narrow, testTerm5, None}
                    Expect.sequenceEqual actual expected "is not equal"
            ]

            testList "AreTermsEquivalent" [
                testCase "checks equivalence correctly" <| fun _ ->
                    Expect.isTrue (testOntology.AreTermsEquivalent(testTerm4, testTerm6)) "is not equal"
            ]

            testList "ReturnAllEquivalentTerms" [
                testCase "returns correct terms" <| fun _ ->
                    let actual = testOntology.ReturnAllEquivalentTerms(testOntology2)
                    Expect.sequenceEqual actual (seq {testTerm4}) "is not equal"
            ]
        ]