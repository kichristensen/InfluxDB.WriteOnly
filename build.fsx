#r @"packages/build/FAKE/tools/FakeLib.dll"
open Fake

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

Target "All" DoNothing

"Clean"
==> "Build"
==> "CopyBinaries"
==> "All"

RunTargetOrDefault "All"