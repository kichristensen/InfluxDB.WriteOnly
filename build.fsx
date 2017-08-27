#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

let project = "InfluxDB.WriteOnly"
let summary = "Write-only client for InfluxDB"

let newVersion = getBuildParam "newVersion"

let isRelease = newVersion <> ""

let changeLogFile = "CHANGELOG.md"

let changeLog = changeLogFile |> ChangeLogHelper.LoadChangeLog

Target "AssemblyInfo" (fun _ ->
    let changeLog =
        changeLogFile
        |> ChangeLogHelper.LoadChangeLog

    let getAssemblyInfoAttributes projectName =
        [ Attribute.Title (projectName)
          Attribute.Product project
          Attribute.Description summary
          Attribute.FileVersion changeLog.LatestEntry.AssemblyVersion
          Attribute.Version ((string changeLog.LatestEntry.SemVer.Major) + ".0.0")
          Attribute.InformationalVersion changeLog.LatestEntry.AssemblyVersion ]

    let getProjectDetails projectPath =
        let projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath)
        ( projectPath, 
          projectName,
          System.IO.Path.GetDirectoryName(projectPath),
          (getAssemblyInfoAttributes projectName)
        )

    !! "src/**/*.??proj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, projectName, folderName, attributes) ->
        match projFileName with
        | Fsproj -> CreateFSharpAssemblyInfo (folderName @@ "AssemblyInfo.fs") attributes
        | Csproj -> CreateCSharpAssemblyInfo ((folderName @@ "Properties") @@ "AssemblyInfo.cs") attributes
        | Vbproj -> CreateVisualBasicAssemblyInfo ((folderName @@ "My Project") @@ "AssemblyInfo.vb") attributes
        | _ -> failwithf "Unsupported proj file: %s" projFileName
        )
)

Target "Clean" (fun _ ->
    CleanDirs [ "bin" ]
)

Target "Build" (fun _ ->
    !! "InfluxDB.WriteOnly.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

Target "CopyBinaries" (fun _ ->
    !! "src/**/*.??proj"
    |>  Seq.map (fun f -> ((System.IO.Path.GetDirectoryName f) @@ "bin/Release", "bin" @@ (System.IO.Path.GetFileNameWithoutExtension f)))
    |>  Seq.iter (fun (fromDir, toDir) -> CopyDir toDir fromDir (fun _ -> true))
)

Target "Promote Unreleased to new version" (fun _ ->
    if not isRelease then failwith "New version must be specified"
    
    changeLogFile
    |> ChangeLogHelper.LoadChangeLog
    |> ChangeLogHelper.PromoteUnreleased newVersion
    |> ChangeLogHelper.SaveChangeLog changeLogFile
    |> ignore
)

Target "Help" (fun _ ->
    listTargets()
)

Target "NuGet" (fun _ ->
    let changeLog = changeLogFile |> ChangeLogHelper.LoadChangeLog
    Paket.Pack(fun p ->
        { p with
            OutputPath = "bin"
            Version = changeLog.LatestEntry.NuGetVersion
            Symbols = true })
)

Target "PublishNuGet" (fun _ ->
    Paket.Push(fun p ->
        { p with
            WorkingDir = "bin" })
)

Target "Release" DoNothing

Target "All" DoNothing

"Clean"
=?> ("Promote Unreleased to new version", isRelease)
=?> ("AssemblyInfo", isRelease)
==> "Build"
==> "CopyBinaries"
==> "All"

"All"
==> "NuGet"
==> "BuildPackage"

"BuildPackage"
==> "PublishNuGet"
==> "Release"

RunTargetOrDefault "All"