#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Testing

let project = "InfluxDB.WriteOnly"
let summary = "Write-only client for InfluxDB"

let newVersion = getBuildParam "newVersion"

let testAssemblies = "tests/**/bin/Release/*Tests*.dll"

let isRelease = newVersion <> ""

let changeLogFile = "CHANGELOG.md"

Target "AssemblyInfo" (fun _ ->
    let changeLog =
        changeLogFile
        |> ChangeLogHelper.LoadChangeLog

    "src/InfluxDB.WriteOnly/InfluxDB.WriteOnly.csproj"
    |> RegexReplaceInFileWithEncoding @"\<VersionPrefix\>.*\</VersionPrefix>"
                                      (sprintf "<VersionPrefix>%O</VersionPrefix>" changeLog.LatestEntry.SemVer)
                                      System.Text.Encoding.UTF8
)

Target "Clean" (fun _ ->
    CleanDirs [ "bin" ]
)

Target "Restore" (fun _ ->
    !! "src/**/*.??proj"
    ++ "tests/**/*.??proj"
    |> Seq.iter (fun d ->
        DotNetCli.Restore (fun p ->
            { p with
                Project = d }))
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
    DotNetCli.Pack
        (fun p ->
            { p with
                WorkingDir = "src/InfluxDB.WriteOnly";
                OutputPath = "../../bin";
                AdditionalArgs = [ "--include-symbols" ] })
)

Target "PublishNuGet" (fun _ ->
    Paket.Push(fun p ->
        { p with
            WorkingDir = "bin" })
)

Target "RunTests" (fun _ ->
    DotNetCli.Test
        (fun p ->
            { p  with
                WorkingDir = "tests/InfluxDB.WriteOnly.Tests" })
)

Target "UpdateAppVeyorVersion" (fun _ ->
    let changeLog = changeLogFile |> ChangeLogHelper.LoadChangeLog
    "appveyor.yml"
    |> RegexReplaceInFileWithEncoding @"version: .*"
                                      (sprintf "version: %O.{build}" changeLog.LatestEntry.SemVer)
                                      System.Text.Encoding.UTF8
)

Target "Release" DoNothing

Target "All" DoNothing

Target "BuildPackage" DoNothing

"Clean"
=?> ("Promote Unreleased to new version", isRelease)
=?> ("AssemblyInfo", isRelease)
==> "Restore"
==> "Build"
==> "CopyBinaries"
==> "RunTests"
==> "All"

"All"
==> "NuGet"
==> "BuildPackage"

"BuildPackage"
==> "PublishNuGet"
==> "UpdateAppVeyorVersion"
==> "Release"

RunTargetOrDefault "All"