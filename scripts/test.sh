#!/usr/bin/env bash
./mpm.exe install
./mpm.exe build --input "src/tag-util" --output "test/TagUtil.cs"
./mpm.exe build --input "src/text-size-util" --output "test/TextSizeUtil.cs"
./mpm.exe build --input "src/tag-service" --output "test/TagService.cs"
./mpm.exe build --input "src/step-service" --output "test/StepService.cs"
./mpm.exe build --input "src/signal" --output "test/Signal.cs"
./mpm.exe build --input "src/quad-tree" --output "test/QuadTree.cs"
./mpm.exe build --input "src/player-util" --output "test/PlayerUtil.cs"
./mpm.exe build --input "src/option" --output "test/Option.cs"
./mpm.exe build --input "src/maid" --output "test/Maid.cs"
./mpm.exe build --input "src/coordinate-service" --output "test/CoordinateService.cs"
./mpm.exe build --input "src/task-util" --output "test/TagUtil.cs"